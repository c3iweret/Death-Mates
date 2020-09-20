using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public List<PickupSpawnpoint> pickupSpawnpoints;
    public Transform center;

    public void AddSpawnPoint(PickupSpawnpoint spawnPoint){
        pickupSpawnpoints.Add(spawnPoint);
    }
}
