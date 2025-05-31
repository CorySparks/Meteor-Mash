using System.Collections;
using UnityEngine;

public class impactFlash : MonoBehaviour
{

    public void Flash(SpriteRenderer spriteRend, float duration, Color flashColor)
    {
        StartCoroutine(DoFlash(spriteRend, duration, flashColor));
    }
    private IEnumerator DoFlash(SpriteRenderer spriteRend, float duration, Color flashColor)
    {
        Color originalColor = spriteRend.color;

        for (int i = 0; i < 5; i++)
        {
            spriteRend.color = flashColor;
            yield return new WaitForSeconds(duration);

            spriteRend.color = originalColor;
            yield return new WaitForSeconds(duration);
        }
    }
}
