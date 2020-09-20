using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStartCountdown : MonoBehaviour {
    [SerializeField] private float cameraTranslationSpeedUp = 5;
    [SerializeField] private float defaultTimer = 3f;
    private float timer;

	[SerializeField] private GameHUD gameHUD;
    [SerializeField] private RoundStartCountdownDisplay roundStartCountdownDisplay;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip countdownSound;
    [SerializeField] private AudioClip countdownFinalSound;

    // Use this for initialization
    void OnEnable () {
        GameManager.GetInstance().gameState = GameManager.GameState.PREPARING;
        gameHUD.gameObject.SetActive(false);
        
        timer = defaultTimer;

        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
		GameManager.GetInstance().cameraController.TranslationRateOfApproachPerFrame *= this.cameraTranslationSpeedUp;

        while (timer > 0)
        {
            roundStartCountdownDisplay.UpdateTimer(timer);

            // play sound
            if (timer == 1)
            {
                audioSource.PlayOneShot(countdownFinalSound);
            }
            else
            {
                audioSource.PlayOneShot(countdownSound);
            }

            yield return new WaitForSeconds(1f);

            timer--;
            roundStartCountdownDisplay.UpdateTimer(timer);
        }

		GameManager.GetInstance().cameraController.TranslationRateOfApproachPerFrame /= this.cameraTranslationSpeedUp;
		this.gameObject.SetActive(false);
        GameManager.GetInstance().gameState = GameManager.GameState.INPROGRESS;

        // Reset timer for subsequent rounds and update view
        timer = defaultTimer;
        roundStartCountdownDisplay.UpdateTimer(timer);
        gameHUD.gameObject.SetActive(true);
    }
}
