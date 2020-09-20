using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerTrigger))]
public class PickupSpawnpoint : MonoBehaviour
{
    public Placeable pickupPrefab;
    [SerializeField] private GameObject modelObject;
    [SerializeField] private bool startEnabled = false;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AudioClip pickupAudioClip;

    void Start(){
        // Make sure the pickup is on a lane
        // and add this to the pickups array
        RaycastHit hit;
        Ray groundCheckRay = new Ray(transform.position, Vector3.down);
        bool isOverGround = Physics.Raycast(groundCheckRay, out hit, Mathf.Infinity, groundMask);

        Lane lane;
        if(isOverGround && (lane = hit.collider.GetComponent<Lane>()) != null){
            lane.AddSpawnPoint(this);
        }else{
            Debug.LogError("Pickup not placed on a lane: " + gameObject.name);
        }

        if(startEnabled){
            GeneratePickup();
        }else{
            modelObject.SetActive(false);
        }
    }

    public void GeneratePickup()
    {
        pickupPrefab = GameManager.GetInstance().placeables.GeneratePickup();
        modelObject.SetActive(true);
    }

    // this method call is assigned through inspector via PlayerTrigger component
    public void GivePickupToPlayer(Player player) {
        if(pickupPrefab == null){
            return;
        }

        if(player.placer.CurrentPlacable != null) {
            player.placer.CurrentPlacable.HideBlueprint();
            return;
        }
        // Only give points when a placable is actually picked up
        player.score++;
        player.placer.AssignPlaceable(this.pickupPrefab);

        this.pickupPrefab = null;
        this.modelObject.SetActive(false);

        AudioSource.PlayClipAtPoint(pickupAudioClip, transform.position);
    }
}
