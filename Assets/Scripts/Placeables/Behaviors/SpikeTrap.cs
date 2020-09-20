using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerTrigger))]
public class SpikeTrap : MonoBehaviour
{
    public float slowTime = 1.5f;
    
    private List<Player> ignoredPlayers;
    public int spikePoints = 3;

    private void Start()
    {
        ignoredPlayers = new List<Player>();
    }

    // call this method through the inspector with PlayerTrigger component
    public void TrapPlayer(Player player)
    {
        SlowPlayer(player);
        player.ToggleInvincibility(false);
        player.colorer.SetGlowiness(0);
    }

    private void SlowPlayer(Player player)
    {

        if (!player.invincible)
        {
            StartCoroutine(player.PostDeathImmunity(2, player));
            player.playerDeaths++;
            player.score-=spikePoints;
            if (!ignoredPlayers.Contains(player))
            {
                ignoredPlayers.Add(player);

                // perform slow on player
                StartCoroutine(player.controller.SlowPlayer(slowTime));

                ignoredPlayers.Remove(player);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        Player player = col.gameObject.GetComponent<Player>();
        player.ToggleInvincibility(false);
    }

}
