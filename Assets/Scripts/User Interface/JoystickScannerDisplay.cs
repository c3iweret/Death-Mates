using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoystickScannerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joystickText;

    public void UpdateJoystickCount(int joystickCount, int readyPlayerCount)
    {
        joystickText.text = joystickCount.ToString() + " controllers detected\n";
        joystickText.text += readyPlayerCount.ToString() + " players ready";

        if (readyPlayerCount < 2)
        {
            joystickText.text += ", need " + (2 - readyPlayerCount) + " more";
        }
    }
}
