using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    bool gameFinished = false;

    [SerializeField]
    Image darkBackground;

    [SerializeField]
    TMP_Text resultText;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HumansWin();
        else if (Input.GetMouseButtonDown(1))
            ZombiesWin();
    }

    public void HumansWin()
    {
        if (gameFinished) return;

        darkBackground.gameObject.SetActive(true);

        resultText.text = "HUMANS WIN";
        resultText.gameObject.SetActive(true);

        gameFinished = true;

        Time.timeScale = .2f;
    }

    public void ZombiesWin()
    {
        if (gameFinished) return;

        darkBackground.gameObject.SetActive(true);

        resultText.text = "ZOMBIES WIN";
        resultText.color = Color.red;
        resultText.gameObject.SetActive(true);

        gameFinished = true;

        Time.timeScale = .2f;
    }
}