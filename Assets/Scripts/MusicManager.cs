using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    AudioSource music;

    void Awake()
    {
        DontDestroyOnLoad(this);
        music = this.GetComponent<AudioSource>();
    }

    void Start()
    {
        music.Play();
    }
}
