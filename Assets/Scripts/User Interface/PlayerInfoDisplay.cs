using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoDisplay : MonoBehaviour {

    public TextMeshProUGUI text;
    public Player player;

    void Start()
    {
        Initialize(player);
    }

    private void Update()
    {
        UpdatePlayerDeaths();
    }

    public void Initialize(Player player)
    {
        this.player = player;
        this.GetComponent<TextMeshProUGUI>().color = player.playerColor;
    }

    void UpdatePlayerDeaths()
    {
        text.text = "Score: " + player.score;
    }
}
