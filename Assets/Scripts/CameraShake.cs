using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 originalPosition;

    void Awake()
    {
        mainCam = Camera.main;
        if (mainCam != null)
        {
            originalPosition = mainCam.transform.position;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        if (mainCam != null)
        {
            StartCoroutine(DoShake(duration, magnitude));
        }
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            mainCam.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.position = originalPosition;
    }
}
