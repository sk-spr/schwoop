using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCollisionMessage : MonoBehaviour
{
    public PlayerBehavior player;
    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponentInParent<PlayerBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.HandleCollisions(collision);
    }
}
