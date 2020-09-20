using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour {
	public const float RESPAWN_OFFSET_BEHIND_AVERAGE = 10f;

	private static PlayerManager instance;

	private GameObject playerPrefab;

	public List<Player> players;
	[SerializeField] private List<Transform> playerStartPositions;
	[SerializeField] private float wallOfDeathMinDistance = 10.0f;
    [SerializeField] private float cameraTrackRadius = 3f;

	public float respawnHeight = 5f;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
			Debug.LogError("There can only be one instance of PlayerManager");
		}
    }

	public static PlayerManager GetInstance() {
		return instance;
	}

	public void SpawnPlayers(List<int> readyPlayers)
    {
		List<Player> activePlayers = new List<Player>();

        // enable players that are active
		for (int i = 0; i < readyPlayers.Count; i++)
        {
			activePlayers.Add(players[i]);

			players[i].playerID = readyPlayers[i];
            players[i].gameObject.SetActive(true);
		}

		players = activePlayers;

		// add active players to camera tracking
		for (int i = 0; i < activePlayers.Count; i++)
        {
			GameManager.GetInstance().cameraController.TrackObject(activePlayers[i].gameObject, cameraTrackRadius);
		}

		// enable wall of death
		GameManager.GetInstance().wallOfDeath.gameObject.SetActive(true);
	}

	public void RespawnAllPlayers()
    {
		for(int i = 0; i < players.Count; i++)
        {
			players[i].transform.position = playerStartPositions[i].position;
			players[i].playerState = Player.PlayerState.ALIVE;
			players[i].playerDeaths = players[i].startingDeaths;
                        players[i].controller.ResetTargetLane();
            players[i].controller.ResetPlayerVelocity();
        }
	}

    public void ResetPlaceables(){
        for (int i = 0; i < players.Count; i++)
        {
            players[i].placer.ClearPlaceable();
        }
    }

    public void ResetScores(){
        for (int i = 0; i < players.Count; i++)
        {
            players[i].score = 0;
        }
    }

	public void RespawnDeadPlayer(Player player) {
        float averageX;
        float wodX = GameManager.GetInstance().wallOfDeath.respawnTransform.transform.position.x;
        IEnumerable<Player> otherPlayers = this.players.Where(p => p != player);
        if (otherPlayers.Count() > 0)
        {
                averageX = otherPlayers.Average(p => p.transform.position.x);
        } else
        {
                averageX = wodX;
        }

        // Ensure that the player is spawned in the middle of the lane that they were heading towards
        float laneZ = player.controller.TargetLane.center.position.z;

		player.transform.position = new Vector3(Mathf.Max(averageX - RESPAWN_OFFSET_BEHIND_AVERAGE, wodX), this.respawnHeight, laneZ);
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}   

        public Player GetLeader()
        {
            return players.Aggregate((p1, p2) => p1.transform.position.x > p2.transform.position.x ? p1 : p2);
        }
}
