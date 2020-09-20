using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour {

    private Rigidbody playerRigidbody;
    [SerializeField] private float gravityScale = 1f;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // apply the force of gravity
        playerRigidbody.AddForce(Vector3.up * -9.81f * gravityScale, ForceMode.Acceleration);
    }
}
