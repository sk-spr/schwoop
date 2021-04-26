using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class fadingText : MonoBehaviour
{
    private SpriteRenderer r;
    private void Awake()
    {
        r = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var c = r.color;
        c.a -= 0.01f;
        r.color = c;
    }
}
