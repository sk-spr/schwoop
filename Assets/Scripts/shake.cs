using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public float shakeDuration;
    public float shakeIntensity;
    public float dampingSpeed;
    Vector3 initialPosition;
    private void OnEnable()
    {
        initialPosition = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(shakeDuration > 0)
        {
            this.transform.localPosition = initialPosition + Random.insideUnitSphere * shakeIntensity;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0;
            this.transform.position = initialPosition;
        }
    }
    
}
