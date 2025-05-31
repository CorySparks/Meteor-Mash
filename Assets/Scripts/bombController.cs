using System;
using UnityEngine;

public class bombController : MonoBehaviour
{

    public gameController statManager;
    public GameObject player;
    private Rigidbody2D bombRB;
    public ParticleSystem explosionParticles;
    private CameraShake camShake;
    private bool stopMovement = false;

    [SerializeField]
    private float dropSpeed;

    void Awake()
    {
        bombRB = this.GetComponent<Rigidbody2D>();
        var statRef = GameObject.Find("GameManager");
        if (statRef != null)
        {
            statManager = statRef.GetComponent<gameController>();
            var cameraShakeRef = statManager.GetComponent<CameraShake>();
            if (cameraShakeRef != null)
            {
                camShake = cameraShakeRef;

            }
        }

        var playerRef = GameObject.Find("Player");
        if (playerRef != null)
        {
            player = playerRef;
        }
    }

    void Update()
    {
        dropSpeed = statManager.bombDropSpeed;

        if (!stopMovement)
        {
            bombRB.velocity = Vector3.down * dropSpeed;
        }
        else
        {
            bombRB.velocity = Vector3.zero;
        }

        if (statManager.numberOfBombsCollected >= statManager.numberOfBombs)
        {
            if (statManager.bombGroup < 8)
            {
                statManager.bombGroup++;
                statManager.numberOfBombsCollected = 0;
            }
        }
    }

    public void StopFalling()
    {
        stopMovement = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "tileOne" || col.gameObject.tag == "tileTwo" || col.gameObject.tag == "tileThree")
        {
            camShake.Shake(0.07f, 0.035f);

            statManager.numberOfBombsCollected++;
            statManager.score += statManager.pointValueOfEachBomb;

            AudioSource explosionSound = player.GetComponent<AudioSource>();
            float randomPitch = UnityEngine.Random.Range(1f, 1.5f);
            explosionSound.pitch = randomPitch;
            explosionSound.Play();

            var ex = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            ex.Play();

            Destroy(ex.gameObject, ex.main.duration + ex.main.startLifetime.constantMax);

            Destroy(gameObject);
        }

        if (col.gameObject.tag == "edge")
        {
            statManager.subtractLives();
            AudioClip mySound = Resources.Load<AudioClip>("Sound/loseLife");
            player.GetComponent<AudioSource>().PlayOneShot(mySound, 0.5f);
        }
    }
}
