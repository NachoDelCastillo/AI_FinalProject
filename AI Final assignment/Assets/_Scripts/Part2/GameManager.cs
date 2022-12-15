using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    static public GameManager GetInstance()
    { return instance; }

    private void Awake()
    {
        instance = this;

        Time.timeScale = 1;
    }

    bool gameFinished = false;

    [SerializeField]
    Image darkBackground;

    [SerializeField]
    TMP_Text resultText;

    public void HumansWin()
    {
        if (gameFinished) return;

        PrepareResults();

        resultText.text = "HUMANS WIN";
    }

    public void ZombiesWin()
    {
        if (gameFinished) return;

        PrepareResults();

        resultText.text = "ZOMBIES WIN";
        resultText.color = Color.red;
    }

    void PrepareResults()
    {
        if (gameFinished) return;

        if (darkBackground != null)
            darkBackground.gameObject.SetActive(true);

        if (resultText != null)
            resultText.gameObject.SetActive(true);

        gameFinished = true;

        Time.timeScale = .2f;
    }
}