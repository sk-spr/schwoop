using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleCollisionMessage : MonoBehaviour
{
    public playerBehavior player;
    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponentInParent<playerBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.handleCollisions(collision);
    }
}
