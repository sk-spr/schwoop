using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private GameObject destParticles;
    public bool isBroken;
    private Renderer thisr;
    private void Awake()
    {
        isBroken = false;
        thisr = this.GetComponent<Renderer>();
        destParticles = Resources.Load<GameObject>($"particles/break{1}");
    }
    public void breakSelf()
    {
        thisr.enabled = false;
        isBroken = true;
        var particles = Instantiate(destParticles);
        particles.transform.position = this.transform.position;
        particles.transform.parent = this.transform;
    }
}
