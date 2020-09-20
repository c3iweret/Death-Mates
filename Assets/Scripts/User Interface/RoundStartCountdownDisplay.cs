using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundStartCountdownDisplay : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI timerText;

    public void UpdateTimer(float timer)
    {
        timerText.text = timer.ToString("0");
    }
}
