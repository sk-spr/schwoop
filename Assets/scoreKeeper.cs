using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreKeeper : MonoBehaviour
{
    public float score;
    private void Awake()
    {
        score = 0;
        DontDestroyOnLoad(this.gameObject);
    }
}
