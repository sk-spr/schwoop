using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle : MonoBehaviour
{
    private GameObject destParticles;
    private Renderer thisr;
    private void Awake()
    {
        thisr = this.GetComponent<Renderer>();
        destParticles = Resources.Load<GameObject>($"particles/break{1}");
    }
    public void breakSelf()
    {
        thisr.enabled = false;
        var particles = Instantiate(destParticles);
        particles.transform.position = this.transform.position;
        particles.transform.parent = this.transform;
    }
}
