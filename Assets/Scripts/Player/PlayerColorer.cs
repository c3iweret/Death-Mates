using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerColorer : MonoBehaviour {
    private Player player;
    [SerializeField] private List<Renderer> renderers;

    void Start(){
        player = GetComponent<Player>();

        renderers.ForEach(r => r.material.SetColor("_Color", player.playerColor));
    }

    public void SetGlowiness(float glowiness)
    {
        renderers.ForEach(r => r.material.SetFloat("_Glowiness", glowiness));
    }

}
