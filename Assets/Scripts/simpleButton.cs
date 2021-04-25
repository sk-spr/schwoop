using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(SpriteRenderer))]
public class simpleButton : MonoBehaviour
{
    public Sprite PressedSprite;
    public int targetScene;
    private Camera cam;
    private SpriteRenderer r;
    private Sprite idleSprite;
    public bool currentlyPressed = false;
    public List<simpleButton> otherButtons;
    public List<quitButton> quitButtons;
    public GameObject bleeper;
    private void Awake()
    {
        if (bleeper == null)
            Debug.LogWarning("bleeper not set");
        bleeper = Resources.Load<GameObject>("beeper");
    }
    void Start()
    {
        cam = Camera.main;
        r = this.GetComponent<SpriteRenderer>();
        idleSprite = r.sprite;
        otherButtons = GameObject.FindObjectsOfType<simpleButton>().ToList();
        quitButtons = GameObject.FindObjectsOfType<quitButton>().ToList();
    }

    void Update()
    {
        var pos = cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 10));
        var p = this.transform.position;

        if (pos.x > r.bounds.min.x && pos.y > r.bounds.min.y && pos.x < r.bounds.max.x && pos.y < r.bounds.max.y && Mouse.current.leftButton.isPressed && !currentlyPressed)
        {
            var avail = true;
            //yes, this is a user set list of all other buttons in the scene
            //i am disgusted at myself too
            //TODO: refactor to make quitButton inherit from simpleButton
            foreach(var btn in otherButtons)
                if (btn.currentlyPressed)
                    avail = false;
            foreach (var btn in quitButtons)
                if (btn.currentlyPressed)
                    avail = false;
            if (avail)down();
        }
        if (!Mouse.current.leftButton.isPressed && currentlyPressed)
        {
            currentlyPressed = false;
            up();
        }

    }
    public void down()
    {
        currentlyPressed = true;
        r.sprite = PressedSprite;
        Debug.Log("hello");
        Instantiate(bleeper);
    }
    public void up()
    {
        r.sprite = idleSprite;
        UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
        Debug.Log("goodbye");
    }
}
