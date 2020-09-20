using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider))]
public class Placeable : MonoBehaviour {

    [SerializeField] private Color blueprintColor = new Color(1f, 1f, 1f, 0.35f);

    [SerializeField] private Placeable placeableInstance;

    private BoxCollider boxColliderComponent;
	public BoxCollider BoxColliderComponent {
		get {
			if (this.boxColliderComponent == null)
				this.boxColliderComponent = this.GetComponent<BoxCollider>();
			return this.boxColliderComponent;
		}
	}

    [SerializeField] private Sprite icon;
	public Sprite Icon {
		get {
			return this.icon;
		}
	}

	[SerializeField] private float placementDistance = 5f;
	public float PlacementDistance {
		get {
			return this.placementDistance;
		}
	}

	[SerializeField] private float maxDropDist = 105f;
	public float MaxDropDistance {
		get {
			return this.maxDropDist;
		}
	}

	[SerializeField] private LayerMask blockerCollisionMask;
	public LayerMask BlockerCollisionMask {
		get {
			return this.blockerCollisionMask;
		}
	}

	[SerializeField] private GameObject blueprintPrefab;
	private GameObject blueprintInstance;

	private GameObject BlueprintInstance {
		get {
			if (this.blueprintPrefab == null)
				return null;

			if (this.blueprintInstance == null)
            {
				this.blueprintInstance = GameObject.Instantiate(this.blueprintPrefab);

                this.blueprintInstance.GetComponentsInChildren<MeshRenderer>()
                    .ToList()
                    .ForEach(r => r.material.color = blueprintColor);

                // instantiate a placeable instance as well
                placeableInstance = GameObject.Instantiate(this, Vector3.zero, Quaternion.identity);
                placeableInstance.gameObject.SetActive(false);
            }

            return this.blueprintInstance;
		}
	}

	public void ShowBlueprintAtWorldPosition(Vector3 pos) {
		if (this.BlueprintInstance == null)
			return;

		this.BlueprintInstance.SetActive(true);
		this.BlueprintInstance.transform.position = pos;
	}

	public void HideBlueprint() {
		if (this.BlueprintInstance == null)
			return;

		this.BlueprintInstance.SetActive(false);
	}

	public void PlaceAtWorldPosition(Vector3 pos) {
        // place the placeable
        placeableInstance.transform.position = pos;
        placeableInstance.gameObject.SetActive(true);
        
		this.RemoveBlueprint();
	}

	private void RemoveBlueprint() {
		if (this.BlueprintInstance == null)
			return;

		GameObject.Destroy(this.BlueprintInstance);
		this.blueprintInstance = null;
	}
}
