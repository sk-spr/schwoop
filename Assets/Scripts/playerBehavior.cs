/*
    Scripts/playerBehavior.cs
    Skye Sprung 04/2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class playerBehavior : MonoBehaviour
{
    //if anyone is looking at this after the jam, please don't judge me
    //i swear i do acceptable work if i have time but this might turn out to be a complete mess

    public Vector2 initialPosition;
    public Transform arm;
    public float speedQuotient;
    public GameObject heart;
    public List<AudioClip> hitSounds;
    public GameObject loseSound;
    private Vector2 currentPosition;
    private float currentArmRot;
    private List<GameObject> hearts;
    private AudioSource thisSource;
    private bool lost;
    public bool canTakeDamage;
    public float timeSinceDamage;
    public SceneManager sceneM;
    public int health;
    private void Awake()
    {
        thisSource = this.GetComponent<AudioSource>();
    }
    void Start()
    {
        this.transform.position = currentPosition = initialPosition;
        currentArmRot = 0;
        sceneM.reset();
        canTakeDamage = true;
        timeSinceDamage = 0;
        health = 3;

        hearts = new List<GameObject>();
        for(int i = -1; i < 2; i++)
        {
            hearts.Add(Instantiate(heart, new Vector2(i, 3.5f), Quaternion.identity));
        }
        sceneM.reset();
        lost = false;
    }

    void FixedUpdate()
    {
        var controller = Gamepad.current;
        if (controller != null)
            handleControllerInput(controller);
        else
            handleKeyboardInput();
        if (currentPosition.x < -2.5 || currentPosition.x > 2.5)
        {
            currentPosition = initialPosition;
            handleCollisions(null);
            
        }
        if (health < 1 && !lost)
            gameOver();
        
    }
    private void Update()
    {
        this.transform.position = currentPosition;
        var temprot = arm.transform.localRotation.eulerAngles;
        temprot.z = currentArmRot;
        arm.transform.localRotation = Quaternion.Euler(temprot);
        if(hearts.Count > health)
        {
            Debug.Log("too high");
            if(health > 0)
            {
                while(hearts.Count > health)
                {
                    var last = hearts[hearts.Count - 1];
                    Destroy(last);
                    hearts.Remove(last);
                }
            }

        }
        if (!canTakeDamage)
        {
            timeSinceDamage += Time.deltaTime;
            if (timeSinceDamage > 0.5) canTakeDamage = true;
        }
    }
    private void handleControllerInput(Gamepad c)
    {
        currentPosition.x += c.leftStick.ReadValue().x * speedQuotient;
        var rightstick = c.rightStick.ReadValue();
        if(rightstick.magnitude>0.1)
            currentArmRot =  360 -Angle(rightstick) + 90;
    }
    private void handleKeyboardInput()
    {
        if (Keyboard.current.dKey.isPressed)
            currentPosition.x += speedQuotient*0.5f;
        if (Keyboard.current.aKey.isPressed)
            currentPosition.x -= speedQuotient*0.5f;

        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0));
        var thisp = this.transform.position;
        var vecToMouse = pos - thisp;
        var vecToMouse2d = new Vector2(vecToMouse.x, vecToMouse.y);
        currentArmRot = 360 - Angle(vecToMouse2d) + 90;
    }
    private static float Angle(Vector2 v) =>
        (v.x>0) 
            ? 360 - (Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg * -1) 
            : Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
    private void gameOver()
    {
        lost = true;
        Instantiate(loseSound);
        UnityEngine.SceneManagement.SceneManager.LoadScene(3,LoadSceneMode.Single);
    }
    public void handleCollisions(Collider2D c)
    {
        Debug.Log("hit");
        if (canTakeDamage)
        {
            playClip(hitSounds[Random.Range(0,hitSounds.Count)]);
            canTakeDamage = false;
            timeSinceDamage = 0;
            health--;
            Camera.main.GetComponent<shake>().shakeDuration = 0.1f;
            sceneM.multiplier = 1;
            var scripts = c.gameObject.GetComponentsInChildren<obstacle>();
            for(int i = 0; i < scripts.Length; i++)
                scripts[i].breakSelf();
        }

    }
    public void playClip(AudioClip clip)
    {
        thisSource.clip = clip;
        thisSource.Play();
    }

}
