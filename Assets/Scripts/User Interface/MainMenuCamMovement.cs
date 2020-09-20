using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCamMovement : MonoBehaviour
{
    [SerializeField] private float panSpeed = 1f;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private Image darkOverlay;

    private void Start()
    {
        StartCoroutine(DoCameraFade());
    }

    private void FixedUpdate()
    {
        // pan camera from left to right
        transform.position = new Vector3(transform.position.x + Time.deltaTime * panSpeed, transform.position.y, transform.position.z);
    }

    private IEnumerator DoCameraFade()
    {
        while (true)
        {
            transform.position = startPos.position;

            // fade in camera to half alpha
            while (darkOverlay.color.a > 0.5f)
            {
                darkOverlay.color = new Color(darkOverlay.color.r, darkOverlay.color.g, darkOverlay.color.b, darkOverlay.color.a - 0.01f);
                yield return new WaitForSeconds(0.01f);
            }

            // wait until camera reaches end position
            while (transform.position.x < endPos.position.x)
            {
                yield return new WaitForSeconds(0.25f);
            }

            // fade out camera but continue panning
            while (darkOverlay.color.a < 1f)
            {
                darkOverlay.color = new Color(darkOverlay.color.r, darkOverlay.color.g, darkOverlay.color.b, darkOverlay.color.a + 0.01f);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
