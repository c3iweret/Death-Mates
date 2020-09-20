using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerTrigger))]
public class CatapultBoost : MonoBehaviour
{
    [SerializeField] private float force = 60f;
    [SerializeField] private Transform boostPoint;
    [SerializeField] private CatapultArm arm;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip throwSound;

    private bool inUse = false;
    public int catapultPoints = 5;

    // call this method through the inspector with PlayerTrigger component
    public void ThrowPlayer(Player player)
    {
        if (!inUse && !arm.animating)
        {
            StartCoroutine(Boost(player));
        }
    }

    private IEnumerator Boost(Player player)
    {
        player.score += catapultPoints;
        inUse = true;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.position = boostPoint.position;
        player.controller.enabled = false;
        player.GetComponent<PlayerGravity>().enabled = false;

        yield return new WaitForSeconds(0.25f);

        // boost the speed of the player
        player.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * force / 2f + Vector3.right * force, ForceMode.Impulse);

        yield return new WaitForSeconds(0.1f);
  
        // enable player controls after small delay
        player.controller.enabled = true;
        player.GetComponent<PlayerGravity>().enabled = true;
        inUse = false;

        // play sound
        audioSource.PlayOneShot(throwSound);

        // animate the arm
        StartCoroutine(arm.PlayThrowAnimation());
    }
}
