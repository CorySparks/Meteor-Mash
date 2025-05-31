using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    AudioSource music;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            music = this.GetComponent<AudioSource>();
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

    }

    void Start()
    {
        if (!music.isPlaying)
        {
            music.Play();
        }
    }
}
