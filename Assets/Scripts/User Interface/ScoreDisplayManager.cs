using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class ScoreDisplayManager : MonoBehaviour
{
    private List<Player> players;
    public RectTransform[] scoreEntries;
    private Player winner;
    private int winnerScore = 0;
    private RectTransform winningEntry;
    public RectTransform scoreDisplay;
    private Color white = new Color(1, 1, 1, 1);


    public void PopulateScores(){
        players = PlayerManager.GetInstance().players;
        //scoreEntries = this.gameObject.GetComponents<RectTransform>();
        for (int i = 0; i < players.Count; i++){

            //get leading player & score
            if (players[i].score > winnerScore)
            {
                winnerScore = players[i].score;
                winner = players[i];
                winningEntry = scoreEntries[i];
            }

            scoreEntries[i].gameObject.SetActive(true);
            scoreEntries[i].transform.Find("PlayerHeader").GetComponent<TextMeshProUGUI>().color = players[i].playerColor;
            scoreEntries[i].transform.Find("ScoreHeader").GetComponent<TextMeshProUGUI>().color = players[i].playerColor;
            scoreEntries[i].transform.Find("ScoreHeader").GetComponent<TextMeshProUGUI>().text = players[i].score.ToString();
        }
    }

    public void PopulateFinalScores(){
        PopulateScores();
        winningEntry.transform.Find("Crown").gameObject.SetActive(true);
        InvokeRepeating("Blink", 0, 0.4f);
    }

    public void Blink()
    {
        StartCoroutine(BlinkInterval(0.2f, winningEntry.transform.Find("PlayerHeader").GetComponent<TextMeshProUGUI>(),
                                    winningEntry.transform.Find("ScoreHeader").GetComponent<TextMeshProUGUI>(), winner));
    }

    private IEnumerator BlinkInterval(float secs, TextMeshProUGUI headerText, TextMeshProUGUI scoreText, Player player)
    {
        headerText.color = white;
        scoreText.color = white;
        yield return new WaitForSeconds(secs);
        headerText.color = player.playerColor;
        scoreText.color = player.playerColor;
    }

    public void CancelScoreBlink(){
        CancelInvoke("Blink");
        winningEntry.transform.Find("Crown").gameObject.SetActive(false);
    }

}
