using System;
using System.Collections;
using UnityEngine;

public class enemyController : MonoBehaviour
{

    [SerializeField]
    private float enemyMoveSpeed, enemyDropInterval, randomPos, screenMinX, screenMaxX;

    public bool playing = false, liveTaken = false, gameOver = false, canDrop = true, isDropping = false;

    private float leanAmount = 25f, leanSpeed = 5f, bounceAmplitude = 0.1f, bounceFrequency = 2f, previousX;
    private float squashAmount = 0.85f, stretchDuration = 0.15f;
    private bool isSquashing = false;

    private Vector3 initialPosition;
    [SerializeField]
    private GameObject bomb;

    public gameController statManager;

    public Coroutine myCoroutine;

    void Awake()
    {
        var statRef = GameObject.Find("GameManager");
        if (statRef != null)
        {
            statManager = statRef.GetComponent<gameController>();
        }
    }

    void Start()
    {
        initialPosition = transform.position;
        previousX = transform.position.x;

        screenMinX = Camera.main.ScreenToWorldPoint(Vector2.zero).x + 1;
        screenMaxX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - 1;
        randomPos = UnityEngine.Random.Range(screenMinX + 1, screenMaxX - 1);
    }

    void Update()
    {
        Wobble();

        enemyMoveSpeed = statManager.enemyMoveSpeed;
        enemyDropInterval = statManager.enemyDropInterval;

        if (Input.GetKeyDown("space") && canDrop && !playing && !liveTaken && !isDropping)
        {
            statManager.spaceBar.gameObject.SetActive(false);
            playing = true;
        }

        if (playing && !statManager.lives.Equals(0))
        {
            MoveEnemy();
        }

        if (playing && myCoroutine == null && !isDropping)
        {
            myCoroutine = StartCoroutine(DropBomb());
        }
    }

    private void MoveEnemy()
    {
        if (!gameOver)
        {
            Vector3 targetPosition = new Vector3(randomPos, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyMoveSpeed * Time.deltaTime);
        }

        if (Mathf.Approximately(transform.position.x, randomPos))
        {
            randomPos = UnityEngine.Random.Range(screenMinX + 1, screenMaxX - 1);
        }
    }

    private IEnumerator DropBomb()
    {
        isDropping = true;
        canDrop = false;

        int num = statManager.numberOfBombs;

        for (int i = 0; i < num; i++)
        {
            if (!liveTaken && playing)
            {
                AudioSource shootSound = this.GetComponent<AudioSource>();
                float randomPitch = UnityEngine.Random.Range(1f, 1.5f);
                shootSound.pitch = randomPitch;
                shootSound.Play();

                Instantiate(bomb, transform.position, Quaternion.identity);
                StartCoroutine(DoSquashAndStretch());
                yield return new WaitForSeconds(enemyDropInterval);
            }
        }

        yield return new WaitForSeconds(0.1f);

        playing = false;
        liveTaken = false;
        canDrop = true;
        isDropping = false;
        myCoroutine = null;

        yield return new WaitForSeconds(0.5f);

        if (!gameOver)
        {
            statManager.spaceBar.gameObject.SetActive(true);
        }
    }

    private void Wobble()
    {
        float bounceOffset = Mathf.Sin(Time.time * bounceFrequency) * bounceAmplitude;
        transform.position = new Vector3(transform.position.x, initialPosition.y + bounceOffset, transform.position.z);

        float targetZRotation = 0f;

        if (playing)
        {
            float direction = randomPos - transform.position.x;

            if (Mathf.Abs(direction) > 0.05f)
            {
                targetZRotation = direction > 0 ? -leanAmount : leanAmount;
            }
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * leanSpeed);
    }

    private IEnumerator DoSquashAndStretch()
    {
        if (isSquashing) yield break;
        isSquashing = true;

        Vector3 originalScale = transform.localScale;
        Vector3 squashedScale = new Vector3(originalScale.x * 1.15f, originalScale.y * 0.75f, originalScale.z);

        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration / 2f)
        {
            float t = elapsed / (duration / 2f);
            t = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        Vector3 overshootScale = new Vector3(originalScale.x * 0.9f, originalScale.y * 1.1f, originalScale.z);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float wiggle = Mathf.Sin(t * Mathf.PI * 3f) * 0.1f; // jiggle sine wave

            Vector3 jiggleScale = Vector3.Lerp(overshootScale, originalScale, t);
            jiggleScale.y += wiggle;
            jiggleScale.x -= wiggle;

            transform.localScale = jiggleScale;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        isSquashing = false;
    }
}
