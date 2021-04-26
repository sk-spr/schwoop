/*
    Skye Sprung 04/2021
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    //if anyone is looking at this after the jam, please don't judge me
    //i swear i do acceptable work if i have time but this might turn out to be a complete mess

    public Vector2 initialPosition;
    public Transform arm;
    public float speedQuotient;
    public GameObject heart;
    public List<AudioClip> hitSounds;
    public GameObject loseSound;
    public powerup heldPowerup;
    public GameObject shotgunBlast;
    public AudioClip shotgunSound;
    public float powerupCooldown;
    public Renderer shield;
    public SpriteRenderer armRenderer;
    public Sprite armShotgun;
    public Sprite armForce;
    public Transform cooldownDisplay;
    private Vector2 currentPosition;
    private float currentArmRot;
    private List<GameObject> hearts;
    private AudioSource thisSource;
    private bool lost;
    public bool canTakeDamage;
    public float timeSinceDamage;
    public SceneManager sceneM;
    public int health;
    public Transform hand;
    public GameObject breakTextPrefab;
    private Shake s;
    private void Awake()
    {
        thisSource = this.GetComponent<AudioSource>();
    }
    void Start()
    {
        s = Camera.main.GetComponent<Shake>();
        this.transform.position = currentPosition = initialPosition;
        currentArmRot = 0;
        sceneM.Reset();
        canTakeDamage = true;
        timeSinceDamage = 0.5f;
        health = 3;

        hearts = new List<GameObject>();
        for(int i = -1; i < 2; i++)
        {
            hearts.Add(Instantiate(heart, new Vector2(i, 3.5f), Quaternion.identity));
        }
        sceneM.Reset();
        lost = false;
    }

    void FixedUpdate()
    {
        var controller = Gamepad.current;
        if (controller != null)
            HandleControllerInput(controller);
        else
            HandleKeyboardInput();
        if (currentPosition.x < -2.5 || currentPosition.x > 2.5)
        {
            currentPosition = initialPosition;
            handleObstacleHit();
            
        }
        if (health < 1 && !lost)
            GameOver();
        
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
            timeSinceDamage -= Time.deltaTime;
        }
        if (timeSinceDamage < 0)
        {
            canTakeDamage = true;
            shield.enabled = false;
        }
        else
        {
            canTakeDamage = false;
        }
        powerupCooldown -= Time.deltaTime;
        var scale = cooldownDisplay.localScale;
        scale.x = Mathf.Clamp(powerupCooldown/5, 0, 1);
        cooldownDisplay.localScale = scale;
    }
    private void HandleControllerInput(Gamepad c)
    {
        currentPosition.x += c.leftStick.ReadValue().x * speedQuotient;
        var rightstick = c.rightStick.ReadValue();
        if(rightstick.magnitude>0.1)
            currentArmRot =  360 -Angle(rightstick) + 90;
        if(c.bButton.isPressed)
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Single);
        if (c.rightTrigger.IsActuated(0.2f))
            HandlePowerupFire();
    }
    private void HandleKeyboardInput()
    {
        if (Keyboard.current.dKey.isPressed)
            currentPosition.x += speedQuotient*0.5f;
        if (Keyboard.current.aKey.isPressed)
            currentPosition.x -= speedQuotient*0.5f;
        if (Keyboard.current.escapeKey.isPressed)
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Single);
        if (Mouse.current.leftButton.isPressed)
            HandlePowerupFire();
        
        var pos = Camera.main.ScreenToWorldPoint(new Vector3(
            Mouse.current.position.ReadValue().x,
            Mouse.current.position.ReadValue().y,
            0));
        var thisp = this.transform.position;
        var vecToMouse = pos - thisp;
        var vecToMouse2d = new Vector2(vecToMouse.x, vecToMouse.y);
        currentArmRot = 360 - Angle(vecToMouse2d) + 90;
    }
    private void HandlePowerupFire()
    {
        if (powerupCooldown > 0)
            return;
        switch (heldPowerup)
        {
            
            case powerup.None:
                return;
            case powerup.Shotgun:
                s.shakeIntensity = 0.2f;
                s.shakeDuration = 0.1f;
                bool hitAnything = false;
                for(int i = 0; i < 5; i++)
                {
                    var direction = new Vector2(
                        Mathf.Cos(currentArmRot * Mathf.Deg2Rad + (5*i*Mathf.Deg2Rad)), 
                        Mathf.Sin(currentArmRot * Mathf.Deg2Rad + (5*i*Mathf.Deg2Rad)));
                    var hit = Physics2D.RaycastAll(hand.position, direction).ToList();
                    hit = hit.Where<RaycastHit2D>(
                        h =>h.collider != this.GetComponentInChildren<Collider2D>() ).ToList();
                    if(hit.Count > 0)
                    {
                        var obstacle = hit[0].collider.gameObject.GetComponent<Obstacle>();
                        if (obstacle && !obstacle.isBroken)
                        {
                            obstacle.breakSelf();
                            sceneM.otherObjects.Add(
                                Instantiate(breakTextPrefab, obstacle.transform.position, Quaternion.identity));
                            sceneM.keeper.score += 30;
                        }
                        hitAnything = true;
                    }
                }
                var blastrot = Quaternion.Euler(0, 0, currentArmRot);
                var blast = Instantiate(shotgunBlast, hand.position, blastrot);
                
                if (hitAnything)
                    PlayClip(shotgunSound);
                powerupCooldown = 2;
                sceneM.multiplier -= Mathf.Sin(currentArmRot) / 5;

                return;
            case powerup.ForceBlast:
                shield.enabled = true;
                timeSinceDamage = 1.5f;
                powerupCooldown = 5;
                return;
        }
    }
    private void GameOver()
    {
        lost = true;
        s.shakeDuration = 1f;
        Instantiate(loseSound);
        UnityEngine.SceneManagement.SceneManager.LoadScene(3,LoadSceneMode.Single);
    }
    public void HandleCollisions(Collider2D c)
    {
        Debug.Log("hit");
        var o = c.GetComponent<Obstacle>();
        if (canTakeDamage && o != null)
        {
            if (!o.isBroken)
            {
                handleObstacleHit();
                var scripts = c.gameObject.GetComponentsInChildren<Obstacle>();
                for(int i = 0; i < scripts.Length; i++)
                    scripts[i].breakSelf();
            }
            
        } 
        if(c.gameObject.name == "shotgunPickup")
        {
            //TODO: add pickup sound effect
            heldPowerup = powerup.Shotgun;
            armRenderer.sprite = armShotgun;
            Destroy(c.gameObject);
        } else if(c.gameObject.name == "forcePickup")
        {
            //TODO: add pickup sound effect
            heldPowerup = powerup.ForceBlast;
            armRenderer.sprite = armForce;
            Destroy(c.gameObject);
        }
    }
    private void handleObstacleHit()
    {
        PlayClip(hitSounds[Random.Range(0, hitSounds.Count)]);
        canTakeDamage = false;
        timeSinceDamage = 1;
        health--;
        s.shakeIntensity = 0.4f;
        s.shakeDuration = 0.1f;
        sceneM.multiplier = 1;
    }
    public void PlayClip(AudioClip clip)
    {
        thisSource.clip = clip;
        thisSource.Play();
    }
    private static float Angle(Vector2 v) =>
            (v.x>0) 
                ? 360 - (Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg * -1) 
                : Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
}
public enum powerup
{
    None,
    Shotgun,
    ForceBlast
}
