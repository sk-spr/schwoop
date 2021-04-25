using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class musicPlayer : MonoBehaviour
{
    private AudioSource source;
    private List<GameObject> other;
    private bool notFirst = false;

    private void Awake()
    {
        other = GameObject.FindGameObjectsWithTag("music").ToList();
        other.Remove(this.transform.gameObject);
        if (other.Count > 0)
            Destroy(this.transform.gameObject);
        DontDestroyOnLoad(this.transform.gameObject);
        source = this.GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (source.isPlaying) return;
        source.Play();
    }
    public void StopMusic()
    {
        source.Stop();
    }

}
