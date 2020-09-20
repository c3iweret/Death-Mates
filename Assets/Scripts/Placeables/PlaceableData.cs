using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/PlaceableData", order = 1)]
public class PlaceableData : ScriptableObject {
    [SerializeField] private List<Placeable> placeables;

    public Placeable GeneratePickup(){
        return placeables[Random.Range(0, placeables.Count)];
    }
}
