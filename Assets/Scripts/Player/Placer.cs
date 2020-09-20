using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Placer : MonoBehaviour {
	[SerializeField] private Sprite defaultPlacableIcon;
	[SerializeField] private LayerMask groundLayerMask;

    [SerializeField] private Placeable currentPlacable;
    [SerializeField] private float distanceFromLeader = 5f;
    [SerializeField] private PlayerIndicator indicator;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buildSound;

	public Placeable CurrentPlacable {
		get { return currentPlacable; } 
        set { this.currentPlacable = value; }
	}

	public bool HasPlacable {
		get {
			return this.currentPlacable != null;
		}
	}

    public void AssignPlaceable(Placeable placeable)
    {
        currentPlacable = placeable;
        indicator.UpdatePlaceableIcon();
    }

    public void ClearPlaceable()
    {
        currentPlacable = null;
        indicator.UpdatePlaceableIcon();
    }

	public bool ActivateBlueprint { get; set; }
	
    public bool Place(){
		if (!this.HasPlacable)
			return false;

		Vector3? placeLocation = this.GetWorldPlacementPos();
		if (!placeLocation.HasValue)
			return false;

        // play build sound
        audioSource.PlayOneShot(buildSound);

        // place placeable in the world
		this.currentPlacable.PlaceAtWorldPosition(placeLocation.Value);
        ClearPlaceable();

        return true;
    }
    
    public Sprite GetPlaceableIcon(){
		return this.HasPlacable && this.currentPlacable.Icon != null ? this.currentPlacable.Icon : this.defaultPlacableIcon;
    }

	private void Update()
    {
		this.UpdatePlacableBlueprintIfNecessary();
	}

	private void UpdatePlacableBlueprintIfNecessary() {
		if (!this.HasPlacable)
			return;

		Vector3? placeLocation = this.GetWorldPlacementPos();
		if (!this.ActivateBlueprint || !placeLocation.HasValue) {
			this.currentPlacable.HideBlueprint();
			return;
		}

		this.currentPlacable.ShowBlueprintAtWorldPosition(placeLocation.Value);
	}

	private Vector3? GetWorldPlacementPos() {
		if (!this.HasPlacable)
			return null;

		RaycastHit hit;

                // Calculate placement position
                Vector3 leaderPos = PlayerManager.GetInstance().GetLeader().transform.position;
                Vector3 placementPos = new Vector3(leaderPos.x + distanceFromLeader, transform.position.y, transform.position.z);

		// Raycast towards ground
		bool overGround = Physics.Raycast(
				new Ray(placementPos,
				Vector3.down),
				out hit,
				this.currentPlacable.MaxDropDistance,
				this.groundLayerMask);

        // return if raycast didn't hit anything
        if (!overGround)
        {
            return null;
        }

		// Return if ground is not a lane
		Lane lane = hit.collider.GetComponent<Lane>();
		if (lane == null)
        {
            return null;
        }
        
		Vector3 placeablePos = new Vector3(hit.point.x, lane.transform.position.y + currentPlacable.BoxColliderComponent.size.y / 2, lane.center.transform.position.z);

		// Check for any traps or players at the placement position
		bool canPlace = !Physics.CheckBox(
				placeablePos + this.currentPlacable.BoxColliderComponent.center,
				this.currentPlacable.BoxColliderComponent.size / 2,
				Quaternion.identity,
				this.currentPlacable.BlockerCollisionMask);
		return canPlace ? placeablePos : (Vector3?)null;
	}
}
