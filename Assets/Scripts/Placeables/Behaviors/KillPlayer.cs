using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour {
    public void Kill(Player player)
    {
        GameManager.GetInstance().CheckRoundStatus();
        player.playerDeaths++;

        PlayerManager.GetInstance().RespawnDeadPlayer(player);
    }
}
