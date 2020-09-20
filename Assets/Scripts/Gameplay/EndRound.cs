using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class EndRound : MonoBehaviour
{
    private int playerCount;

    public void RemoveActivePlayer(Player player)
    {
        GameManager.GetInstance().SetPlayerFinished(player);
    }

}
