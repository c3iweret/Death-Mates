using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
	private Camera cameraReference;
	public Camera CameraComponent {
		get {
			if (this.cameraReference != null)
				return this.cameraReference;
			return this.cameraReference = this.GetComponent<Camera>();
		}
	}

	[SerializeField] private GameObject[] initialTrackedObjects;
	[SerializeField] private float[] initialTrackedObjectRadii;

	[SerializeField] private float minFarPlane = 1000;

	[SerializeField] private float translationRateOfApproachPerFrame = 0.01f;
    [SerializeField] private Vector3 deadZoneRadius = Vector3.right * 4f;

	public float TranslationRateOfApproachPerFrame {
		get {
			return this.translationRateOfApproachPerFrame;
		}
		set {
			this.translationRateOfApproachPerFrame = value;
		}
	}

	[SerializeField] private float dollyRateOfApproachPerFrame = 0.01f;
	public float DollyRateOfApproachPerFrame {
		get {
			return this.dollyRateOfApproachPerFrame;
		}
		set {
			this.dollyRateOfApproachPerFrame = value;
		}
	}

	[SerializeField] private float dollyThresholdDeltaPercentage = 0.05f;
	private bool isDollying;

	private Dictionary<GameObject, float> trackedObjectRadiiTable = new Dictionary<GameObject, float>();

	public void TrackObject(GameObject go, float radius) {
		// Use to dynamically edit object tracking status

		this.trackedObjectRadiiTable[go] = radius;
	}

	public void UntrackObject(GameObject go) {
		// Use to dynamically untrack an object

		this.trackedObjectRadiiTable.Remove(go);
	}

	protected virtual void Start() {
		this.CameraComponent.nearClipPlane = 0.01f;
		this.CameraComponent.orthographic = false;

		for (int i = 0; i < this.initialTrackedObjects.Length; i++)
			this.TrackObject(this.initialTrackedObjects[i], i >= this.initialTrackedObjectRadii.Length ? 0 : this.initialTrackedObjectRadii[i]);
	}

	protected virtual void FixedUpdate() {
		this.RefreshTrackedObjectStatus();

		if (this.trackedObjectRadiiTable.Count == 0)
			return;

		this.UpdateCameraTracking();
		this.UpdateDolly();
		this.UpdateFarPlane();
	}

	private void RefreshTrackedObjectStatus() {
		// Automatically clears any destroyed objects from being tracked

		var deletedGameObjects = new HashSet<GameObject>();

		foreach (GameObject go in this.trackedObjectRadiiTable.Keys)
			if (go == null)
				deletedGameObjects.Add(go);

		foreach (GameObject go in deletedGameObjects)
			this.trackedObjectRadiiTable.Remove(go);
	}

	private void UpdateCameraTracking() {
		// Centers camera to the midpoint of all its tracked objects

		Vector3 screenUp = this.transform.up;
		Vector3 screenRight = this.transform.right;

		bool populated = false;
		Vector3 top, bottom, left, right;
		top = bottom = left = right = Vector3.zero;

		foreach (KeyValuePair<GameObject, float> entry in this.trackedObjectRadiiTable) {
			Vector3 trackedObjectPosition = entry.Key.transform.position;

			Vector3 projectedVertical = Vector3.Project(trackedObjectPosition, screenUp);
			Vector3 projectedHorizontal = Vector3.Project(trackedObjectPosition, screenRight);

			Vector3 projectedTop = projectedVertical + entry.Value * screenUp;
			Vector3 projectedBottom = projectedVertical - entry.Value * screenUp;
			Vector3 projectedLeft = projectedHorizontal - entry.Value * screenRight;
			Vector3 projectedRight = projectedHorizontal + entry.Value * screenRight + deadZoneRadius;

			if (!populated) {
				top = projectedTop;
				bottom = projectedBottom;
				left = projectedLeft;
				right = projectedRight;

				populated = true;
				continue;
			}

			// Gets the most extreme points in global space relative to the camera
			if (Vector3.Dot(projectedTop, screenUp) > Vector3.Dot(top, screenUp))
				top = projectedTop;
			if (Vector3.Dot(projectedBottom, screenUp) < Vector3.Dot(bottom, screenUp))
				bottom = projectedBottom;
			if (Vector3.Dot(projectedLeft, screenRight) < Vector3.Dot(left, screenRight))
				left = projectedLeft;
			if (Vector3.Dot(projectedRight, screenRight) > Vector3.Dot(right, screenRight))
				right = projectedRight;
		}

		// Sets camera position to the average of the most extreme points
		this.SmoothTranslation((top + bottom + left + right) * 0.5f);
	}

	private void SmoothTranslation(Vector3 worldTarget) {
		Vector3 relativeTarget = worldTarget - this.transform.position;
		if (relativeTarget.sqrMagnitude <= 1) {
			this.transform.position = worldTarget;
			return;
		}

		Vector3 cameraTranslation = Vector3.ProjectOnPlane(relativeTarget, this.transform.forward);
		this.transform.position += this.translationRateOfApproachPerFrame * cameraTranslation;
	}

	private void UpdateDolly() {
		// Zooms camera in/out to render the least amount possible, while keeping every tracked object in its FOV

		Vector3 cameraPosition = this.transform.position;
		Vector3 focalDirection = this.transform.forward;

		Vector3 screenUp = this.transform.up;
		Vector3 screenRight = this.transform.right;

		float verticalFOVCos, verticalFOVTan, horizontalFOVCos, horizontalFOVTan;
		this.GetFOVTrig(out verticalFOVCos, out verticalFOVTan, out horizontalFOVCos, out horizontalFOVTan);

		float requiredDolly = Mathf.NegativeInfinity;

		foreach (KeyValuePair<GameObject, float> kvp in this.trackedObjectRadiiTable) {
			Vector3 trackedObjectPosition = kvp.Key.transform.position;

			float projectedYCoordinate = Vector3.Dot(trackedObjectPosition - cameraPosition, screenUp);
			float projectedXCoordinate = Vector3.Dot(trackedObjectPosition - cameraPosition, screenRight);

			float objectDolly = -Vector3.Dot(trackedObjectPosition, focalDirection);

			// The point where the object's tracking sphere is tangent to the camera's FOV does not lie on the same camera depth as the object's position
			float verticalRadialExtension = kvp.Value / verticalFOVCos;
			float horizontalRadialExtension = kvp.Value / horizontalFOVCos;

			float topMostPoint = Mathf.Abs(projectedYCoordinate + verticalRadialExtension);
			float botMostPoint = Mathf.Abs(projectedYCoordinate - verticalRadialExtension);
			float leftMostPoint = Mathf.Abs(projectedXCoordinate - horizontalRadialExtension);
			float rightMostPoint = Mathf.Abs(projectedXCoordinate + horizontalRadialExtension);

			requiredDolly = Math.Max(topMostPoint / verticalFOVTan + objectDolly, requiredDolly);
			requiredDolly = Math.Max(botMostPoint / verticalFOVTan + objectDolly, requiredDolly);
			requiredDolly = Math.Max(leftMostPoint / horizontalFOVTan + objectDolly, requiredDolly);
			requiredDolly = Math.Max(rightMostPoint / horizontalFOVTan + objectDolly, requiredDolly);
		}

		float currentDolly = -Vector3.Dot(cameraPosition, focalDirection);
		this.SmoothDolly(cameraPosition + focalDirection * (currentDolly - requiredDolly));
	}

	private void SmoothDolly(Vector3 worldTarget) {
		if (worldTarget == this.transform.position)
			return;

		Vector3 relativeTarget = worldTarget - this.transform.position;
		Vector3 currentDolly = Vector3.Project(this.transform.position, relativeTarget);
		
		if (!this.isDollying && currentDolly.sqrMagnitude != 0 && relativeTarget.sqrMagnitude / currentDolly.sqrMagnitude < this.dollyThresholdDeltaPercentage * this.dollyThresholdDeltaPercentage)
			return;

		this.isDollying = true;
		if (relativeTarget.sqrMagnitude <= 0.01f) {
			this.transform.position = worldTarget;
			this.isDollying = false;
			return;
		}

		this.transform.position += this.dollyRateOfApproachPerFrame * relativeTarget;
	}

	private void UpdateFarPlane() {
		// Sets far plane to render to render as little as possible without cutting off the tracked objects

		Vector3 cameraPosition = this.transform.position;
		Vector3 focalDirection = this.transform.forward;

		float maxFarPlane = 0;
		foreach (KeyValuePair<GameObject, float> kvp in this.trackedObjectRadiiTable)
			maxFarPlane = Mathf.Max(Vector3.Dot(kvp.Key.transform.position - cameraPosition, focalDirection) + kvp.Value, maxFarPlane);
		this.CameraComponent.farClipPlane = Math.Max(maxFarPlane, this.minFarPlane);
	}

	private void GetFOVTrig(out float verticalFOVCos, out float verticalFOVTan, out float horizontalFOVCos, out float horizontalFOVTan) {
		float verticalFOVRads = this.CameraComponent.fieldOfView * Mathf.Deg2Rad * 0.5f;
		verticalFOVCos = Mathf.Cos(verticalFOVRads);
		verticalFOVTan = Mathf.Tan(verticalFOVRads);

		float horiziontalFOVRads = Mathf.Atan(verticalFOVTan * this.CameraComponent.aspect);
		horizontalFOVCos = Mathf.Cos(horiziontalFOVRads);
		horizontalFOVTan = Mathf.Tan(horiziontalFOVRads);
	}
}