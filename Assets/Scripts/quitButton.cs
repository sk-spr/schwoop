using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(SpriteRenderer))]

public class quitButton : MonoBehaviour
{
    public Sprite PressedSprite;
    private Camera cam;
    private SpriteRenderer r;
    private Sprite idleSprite;
    public bool currentlyPressed = false;
    public List<simpleButton> otherButtons;
    public GameObject bleeper;
    private void Awake()
    {
        if (bleeper == null)
            bleeper = Resources.Load<GameObject>("beeper");
    }
    void Start()
    {
        cam = Camera.main;
        r = this.GetComponent<SpriteRenderer>();
        idleSprite = r.sprite;
        otherButtons = GameObject.FindObjectsOfType<simpleButton>().ToList();
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
            foreach(var btn in otherButtons)
                if (btn.currentlyPressed)
                {
                    avail = false;
                }
            
            if(avail)down();
        }
        if (!Mouse.current.leftButton.isPressed && currentlyPressed)
        {
            currentlyPressed = false;
            up();
        }

    }
    public new void down()
    {
        currentlyPressed = true;
        r.sprite = PressedSprite;
        Debug.Log("hello");
        Instantiate(bleeper);
    }
    public new void up()
    {
        r.sprite = idleSprite;
        Application.Quit();
    }
}

