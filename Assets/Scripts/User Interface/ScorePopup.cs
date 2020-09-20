using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopup : MonoBehaviour {

    [SerializeField] float floatSpeed = 1f;

    // Update is called once per frame
    void Update () {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}
