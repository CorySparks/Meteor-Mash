using UnityEngine;

public class MoveMainMenu : MonoBehaviour
{
    public GameObject gameObjectToMove;
    [SerializeField]
    private float randomPos, screenMinX, screenMaxX, moveSpeed = 22f, cordY;

    void Start()
    {
        gameObjectToMove = this.gameObject;
        screenMinX = Camera.main.ScreenToWorldPoint(Vector2.zero).x + 1;
        screenMaxX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - 1;
        randomPos = Random.Range(screenMinX + 1, screenMaxX - 1);
    }
    void Update()
    {
        MoveGameObject();
    }

    private void MoveGameObject()
    {
        {
            if (this.transform.position.x != randomPos)
            {
                this.transform.position = Vector2.MoveTowards(transform.position, new Vector2(randomPos, cordY), moveSpeed * Time.deltaTime);
            }
            else
            {
                randomPos = Random.Range(screenMinX + 1, screenMaxX - 1);
            }

        }
    }
}
