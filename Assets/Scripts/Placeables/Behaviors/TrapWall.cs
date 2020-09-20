using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrapWall : MonoBehaviour {

    private Collider collider;
    public int wallPoints = 2;

    void Start(){
        collider = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision col){
        // Check whether the other collider belongs to a player
        Player player = col.collider.GetComponent<Player>();
        if(player == null){
            return;
        }

        if (player.invincible){
            StartCoroutine(InvincibilityCoroutine(player));
        }

        else{
            player.score -= wallPoints;
            player.playerDeaths++;
            StartCoroutine(player.PostDeathImmunity(2, player));
        }
    }

    void OnCollisionExit(Collision col)
    {
        Player player = col.collider.GetComponent<Player>();
        StartCoroutine(player.ToggleInvincibilityWithDelayCoroutine(false, 0.3f));
        player.colorer.SetGlowiness(0);
    }

    IEnumerator InvincibilityCoroutine(Player player){
        Physics.IgnoreCollision(collider, player.GetComponent<Collider>());
        while(player.invincible){
            yield return null;
        }
        Physics.IgnoreCollision(collider, player.GetComponent<Collider>(), false);
    }

}
