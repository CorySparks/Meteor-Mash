using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Highscore : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("Highscore") > 0)
        {
            this.gameObject.GetComponent<TMP_Text>().text = $"Highscore: {PlayerPrefs.GetInt("Highscore")}";
        }
        else
        {
            this.gameObject.GetComponent<TMP_Text>().text = $"Highscore: {0}";
        }

    }
}
