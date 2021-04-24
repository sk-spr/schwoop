/*
    Scripts/playerBehavior.cs
    Skye Sprung 04/2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerBehavior : MonoBehaviour
{
    //if anyone is looking at this after the jam, please don't judge me
    //i swear i do acceptable work if i have time but this might turn out to be a complete mess

    public Vector2 initialPosition;
    public Transform arm;
    
    private Vector2 currentPosition;
    private float currentArmRot;

    public SceneManager sceneM;
    void Start()
    {
        this.transform.position = currentPosition = initialPosition;
        currentArmRot = 0;
        sceneM.reset();
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
            sceneM.reset();
            Start();
        }
    }
    private void Update()
    {
        this.transform.position = currentPosition;
        var temprot = arm.transform.localRotation.eulerAngles;
        temprot.z = currentArmRot;
        arm.transform.localRotation = Quaternion.Euler(temprot);
    }
    private void handleControllerInput(Gamepad c)
    {
        currentPosition.x += c.leftStick.ReadValue().x * 0.1f;
        var rightstick = c.rightStick.ReadValue();
        if(rightstick.magnitude>0.1)
            currentArmRot =  360 -Angle(rightstick) + 90;
    }
    private void handleKeyboardInput()
    {
        if (Keyboard.current.dKey.isPressed)
            currentPosition.x += 0.1f;
        if (Keyboard.current.aKey.isPressed)
            currentPosition.x -= 0.1f;
    }
    private static float Angle(Vector2 v) =>
        (v.x>0) 
            ? 360 - (Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg * -1) 
            : Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
}
