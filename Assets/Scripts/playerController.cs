using UnityEngine;

public class playerController : MonoBehaviour
{
    public gameController statManager;

    [SerializeField] private float leanMultiplier = 5f;
    [SerializeField] private float maxLean = 10f;
    [SerializeField] private float leanSmoothing = 10f;

    private float previousX;
    private float currentLean;

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
        Cursor.visible = false;
        previousX = transform.position.x;
    }

    void Update()
    {
        if (statManager.lives == 0) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 newPos = new Vector2(mousePosition.x, -2.9f);
        transform.position = newPos;

        float velocityX = (transform.position.x - previousX) / Time.deltaTime;
        float targetLean = Mathf.Clamp(-velocityX * leanMultiplier, -maxLean, maxLean);

        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSmoothing);
        transform.rotation = Quaternion.Euler(0f, 0f, currentLean);

        previousX = transform.position.x;
    }
}
