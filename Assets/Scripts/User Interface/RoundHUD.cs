using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundHUD : MonoBehaviour {

    public TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        scoreText.text = "ROUND " + GameManager.GetInstance().roundCount + "/" + GameManager.GetInstance().maxRoundCount;
    }
}
