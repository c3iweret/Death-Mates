using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoystickScanner : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] private JoystickScannerDisplay joystickScannerDisplay;
    [SerializeField] private float scanInterval = 1f;

    [SerializeField] private GameObject[] playerObjects;
    [SerializeField] private GameObject[] lightObjects;

    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI debugJoystickText;

    private string[] joystickNames;
    private int joystickCount = 0;
    
    public static List<int> readyPlayerIDs;

    // start button control variables
    private bool controllersAssigned = false;

    private void Start()
    {
        readyPlayerIDs = new List<int>();

        StartCoroutine(ScanJoysticks());
    }

    IEnumerator ScanJoysticks()
    {
        // scan for joysticks at an interval
        while (true)
        {
            joystickCount = 0;
            joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length > 0)
            {
                // go through every joystick
                for (int i = 0; i < joystickNames.Length; i++)
                {
                    if (!string.IsNullOrEmpty(joystickNames[i]))
                    {
                        joystickCount++;
                    }
                }
            }

            joystickScannerDisplay.UpdateJoystickCount(joystickCount, readyPlayerIDs.Count);

            yield return new WaitForSeconds(scanInterval);
        }
    }

    private void Update()
    {
        if (!controllersAssigned)
        {
            // scan for each joysticks' start button input
            for (int i = 0; i < 2; i++)
            {
                if (Input.GetAxis("Start" + i) > 0f)
                {
                    // add the player ID to ready players
                    if (!readyPlayerIDs.Contains(i))
                    {
                        readyPlayerIDs.Add(i);

                        // activate play button if 2 or more players are ready
                        if (readyPlayerIDs.Count >= 2)
                        {
                            playButton.interactable = true;

                            debugJoystickText.gameObject.SetActive(false);
                            instructionText.text = "";
                        }

                        playerObjects[readyPlayerIDs.Count - 1].SetActive(true);
                        lightObjects[readyPlayerIDs.Count - 1].SetActive(true);
                    }
                }
            }
        }
    }
}
