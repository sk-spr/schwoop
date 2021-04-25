using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disposableBeeper : MonoBehaviour
{ 
    private AudioSource source;
    public float duration;
    private bool playing;
    private void Awake()
    {
        source = this.GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
        play();
    }
    public void play()
    {
        source.Play();
        playing = true;
    }
    private void Update()
    {
        if (playing)
        {
            duration -= Time.deltaTime;
        }
        if (duration < 0) Destroy(this.gameObject);
    }
}
