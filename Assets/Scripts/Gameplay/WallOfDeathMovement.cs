using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WallOfDeathMovement : MonoBehaviour {
	private float creepSpeed = 0;
    [SerializeField] private float defaultCreepSpeed = 3f;
	[SerializeField] private float maxDistance = 30f;
    public Transform respawnTransform;

    private Vector3 startingPos;

	private void Start() {
		this.startingPos = this.transform.position;
                creepSpeed = defaultCreepSpeed;
	}

	private void Update () {
		List<Player> playerList = PlayerManager.GetInstance().players;
		if (playerList.Count == 0)
			return;

		float firstPlayerX = playerList.Max(p => p.transform.position.x);
		float newX = Mathf.Max(firstPlayerX - this.maxDistance, this.transform.position.x + creepSpeed * Time.deltaTime);
		this.transform.position = new Vector3(newX, this.transform.position.y, this.transform.position.z);
	}

	public void RestartPosition(){
		this.transform.position = this.startingPos;
                creepSpeed = defaultCreepSpeed;
	}
}
