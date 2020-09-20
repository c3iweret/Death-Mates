using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LaneManager : MonoBehaviour
{
    private static LaneManager instance;

    [SerializeField] private int minimumPickupCount = 3;

    [SerializeField] private List<Lane> lanes;
    public List<Lane> Lanes{
        get{
            return lanes;
        }
    }

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
            Debug.LogError("There can only be one LaneManager instance");
        }
        else{
            instance = this;
        }
    }

    public static LaneManager GetInstance()
    {
        return instance;
    }

    public void SpawnPickups()
    {
        // Get all the pickups
        var spawns = new List<PickupSpawnpoint>();
        lanes.ForEach(l => spawns.AddRange(l.pickupSpawnpoints));

        // Count pickups that are already on the level
        int pickupCount = spawns.Count(s => s.pickupPrefab != null);
        
        // Spawn more pickups until minimum is reached
        spawns
            .Where(s => s.pickupPrefab == null)
            .OrderBy(s => Random.value)
            .Take(minimumPickupCount - pickupCount)
            .ToList()
            .ForEach(s => s.GeneratePickup());
    }
}
