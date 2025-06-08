using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{

    public TMP_Text scoreTxt;
    public int score, numberOfBombs, bombGroup, pointValueOfEachBomb, numberOfBombsCollected, lives;
    private bool canTakeDamage = true;
    private float damageCooldown = 0.25f;
    public float bombDropSpeed, enemyMoveSpeed, enemyDropInterval;
    private bool canAddLife = true, allBombsDestroyed;
    public GameObject tile1, tile2, tile3, gameOverPanel, player;
    public enemyController enemyManager;
    private CameraShake camShake;
    public impactFlash impactFlashManager;
    private int lastLifeScoreCheckpoint = 0;
    private const int lifeScoreThreshold = 1000;
    int nextLifeScoreCheckpoint = 1000;
    public ParticleSystem explosionParticles;
    private Color flashRed = new Color(0.89f, 0.43f, 0.40f);
    public Image spaceBar;

    void Awake()
    {
        var enemyRef = GameObject.Find("enemy");
        if (enemyRef != null)
        {
            enemyManager = enemyRef.GetComponent<enemyController>();
        }

        var playerRef = GameObject.Find("Player");
        if (playerRef != null)
        {
            player = playerRef;
        }

        var cameraShakeRef = this.GetComponent<CameraShake>();
        if (cameraShakeRef != null)
        {
            camShake = cameraShakeRef;
        }
    }

    void Start()
    {
        bombGroup = 1;
        numberOfBombsCollected = 0;
        lives = 3;
        tile1 = GameObject.FindGameObjectWithTag("tileOne");
        tile2 = GameObject.FindGameObjectWithTag("tileTwo");
        tile3 = GameObject.FindGameObjectWithTag("tileThree");

        gameOverPanel.transform.Find("Popup/Btn_Restart").GetComponent<Button>().onClick.AddListener(Restart);
    }

    void Update()
    {
        scoreTxt.text = score.ToString();
        GroupStats();

        if (score >= nextLifeScoreCheckpoint)
        {
            nextLifeScoreCheckpoint += lifeScoreThreshold;
        }

        if (lives < 3 && score >= nextLifeScoreCheckpoint)
        {
            AddLives();
        }

        if (lives == 0 && allBombsDestroyed)
        {
            GameOver();
        }
    }

    private void GroupStats()
    {
        switch (bombGroup)
        {
            case 1:
                numberOfBombs = 10;
                pointValueOfEachBomb = 1;
                bombDropSpeed = 7f;
                enemyMoveSpeed = 10f;
                enemyDropInterval = 0.5f;
                break;
            case 2:
                numberOfBombs = 20;
                pointValueOfEachBomb = 2;
                bombDropSpeed = 7.5f;
                enemyMoveSpeed = 12f;
                enemyDropInterval = 0.4f;
                break;
            case 3:
                numberOfBombs = 30;
                pointValueOfEachBomb = 3;
                bombDropSpeed = 8f;
                enemyMoveSpeed = 14f;
                enemyDropInterval = 0.3f;
                break;
            case 4:
                numberOfBombs = 40;
                pointValueOfEachBomb = 4;
                bombDropSpeed = 8.5f;
                enemyMoveSpeed = 18f;
                enemyDropInterval = 0.2f;
                break;
            case 5:
                numberOfBombs = 50;
                pointValueOfEachBomb = 5;
                bombDropSpeed = 8f;
                enemyMoveSpeed = 22f;
                enemyDropInterval = 0.1f;
                break;
            case 6:
                numberOfBombs = 75;
                pointValueOfEachBomb = 6;
                bombDropSpeed = 9f;
                enemyMoveSpeed = 25f;
                enemyDropInterval = 0.08f;
                break;
            case 7:
                numberOfBombs = 100;
                pointValueOfEachBomb = 7;
                bombDropSpeed = 9.5f;
                enemyMoveSpeed = 30f;
                enemyDropInterval = 0.05f;
                break;
            case 8:
                numberOfBombs = 150;
                pointValueOfEachBomb = 8;
                bombDropSpeed = 10f;
                enemyMoveSpeed = 35f;
                enemyDropInterval = 0.05f;
                break;
            default:
                numberOfBombs = 10;
                pointValueOfEachBomb = 1;
                bombDropSpeed = 7f;
                enemyMoveSpeed = 13f;
                enemyDropInterval = 0.5f;
                break;
        }
    }

    private void GameOver()
    {
        enemyManager.canDrop = false;

        gameOverPanel.SetActive(true);
        Cursor.visible = true;

        if (PlayerPrefs.GetInt("Highscore") < score)
        {
            PlayerPrefs.SetInt("Highscore", score);
        }

        gameOverPanel.transform.Find("Popup/Text_Score/Text_Score_Total").GetComponent<TMP_Text>().text = score.ToString();
        gameOverPanel.transform.Find("Popup/Text_Highscore/Text_Highscore_Total").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();

    }

    private void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void AddLives()
    {
        if (lives < 3)
        {
            AudioSource oneUpSound = gameObject.GetComponent<AudioSource>();
            oneUpSound.Play();
        }

        lastLifeScoreCheckpoint += lifeScoreThreshold;

        lives++;

        if (lives == 3)
        {
            tile3.SetActive(true);
        }
        else if (lives == 2)
        {
            tile2.SetActive(true);
        }
    }

    public void subtractLives()
    {
        if (!canTakeDamage || lives <= 0) return;

        canTakeDamage = false;
        StartCoroutine(ResetDamageCooldown());

        lives--;

        camShake.Shake(0.2f, 0.1f);

        impactFlashManager.Flash(tile1.GetComponent<SpriteRenderer>(), 0.05f, flashRed);
        impactFlashManager.Flash(tile2.GetComponent<SpriteRenderer>(), 0.05f, flashRed);
        impactFlashManager.Flash(tile3.GetComponent<SpriteRenderer>(), 0.05f, flashRed);

        enemyManager.myCoroutine = null;
        enemyManager.playing = true;
        enemyManager.canDrop = false;
        enemyManager.liveTaken = true;

        numberOfBombsCollected = 0;

        if (bombGroup != 1)
        {
            bombGroup--;
        }

        if (lives == 2)
        {
            tile3.SetActive(false);
        }
        else if (lives == 1)
        {
            tile2.SetActive(false);
        }
        else if (lives == 0)
        {
            tile1.SetActive(false);
            enemyManager.gameOver = true;
        }

        StartCoroutine(DestroyAllBombs());
    }

    private IEnumerator ResetDamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }


    public IEnumerator DestroyAllBombs()
    {
        allBombsDestroyed = false;

        GameObject[] bombs;
        bombs = GameObject.FindGameObjectsWithTag("bomb");

        foreach (GameObject b in bombs)
        {
            b.GetComponent<CircleCollider2D>().enabled = false;
            b.GetComponent<bombController>().StopFalling();
        }

        foreach (GameObject bomb in bombs)
        {
            AudioClip mySound = Resources.Load<AudioClip>("Sound/explosion");
            player.GetComponent<AudioSource>().PlayOneShot(mySound, 0.5f);

            if (bomb != null)
            {
                camShake.Shake(0.2f, 0.1f);
                var ex = Instantiate(explosionParticles, bomb.transform.position, Quaternion.identity);
                ex.Play();

                Destroy(ex.gameObject, ex.main.duration + ex.main.startLifetime.constantMax);

            }

            Destroy(bomb.gameObject);

            yield return new WaitForSeconds(0.2f);
        }

        if (bombs.Length > 0)
        {
            if (!enemyManager.gameOver)
            {
                spaceBar.gameObject.SetActive(true);
            }

            enemyManager.liveTaken = false;
            enemyManager.playing = false;
            enemyManager.canDrop = true;
            allBombsDestroyed = true;
        }
    }
}
