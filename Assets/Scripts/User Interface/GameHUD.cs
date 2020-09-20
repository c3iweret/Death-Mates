using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{

    [SerializeField] private List<PlayerInfoDisplay> playerInfoDisplays;
    [SerializeField] private RoundHUD roundHUD;

    public void SetupHUD(List<Player> players)
    {
        int numberOfActivePlayers = players.Count;
        for (int i = 0; i < numberOfActivePlayers; i++)
        {
            if (players[i])
            {
                playerInfoDisplays[i].Initialize(players[i]);
                playerInfoDisplays[i].transform.SetParent(transform);
                playerInfoDisplays[i].gameObject.SetActive(true);
            }
        }
        roundHUD.gameObject.SetActive(true);
    }
}
