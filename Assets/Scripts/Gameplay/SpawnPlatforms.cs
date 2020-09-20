using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPlatforms : MonoBehaviour {

        [SerializeField] private GameObject platformPrefab;

	// Use this for initialization
	void Start () {
	    for(float x = -150f; x < 150f; x += 8.8f){
                Instantiate(platformPrefab, transform.position + Vector3.right * x, transform.rotation, transform);
            }
	}
}
