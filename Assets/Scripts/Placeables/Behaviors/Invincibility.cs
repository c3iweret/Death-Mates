using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(PlayerTrigger))]
public class Invincibility : MonoBehaviour
{
    public float invincibleTime = 3f;
    public float initialPlayerColor;
    public float invincibilityAlpha = 0.5f;
    public int invincibilityPoints = 8;

	private const float FLASH_DURATION = 0.2f;

	private void Awake() {
		if (FLASH_DURATION > 1)
			throw new System.ArgumentOutOfRangeException("Flash duration must currently be less than 1 second");
	}

	public void ActivateInvincibleState(Player player)
    {   
        //change player state to invincible
        player.invincible = true;
        player.score += invincibilityPoints;
        StartCoroutine(InvinciblePeriod(invincibleTime, player));
    }

    IEnumerator InvinciblePeriod(float seconds, Player player)
    {
        player.invincible = true;
        player.colorer.SetGlowiness(1);

		yield return new WaitForSeconds(seconds - 1);

        // blink before turning off invincibility
		for (int i = 0; i < (int)(1 / FLASH_DURATION); i++)
        {
            player.colorer.SetGlowiness(0);
            yield return new WaitForSeconds(FLASH_DURATION / 2);

            player.colorer.SetGlowiness(1);
            yield return new WaitForSeconds(FLASH_DURATION / 2f);
        }

        player.invincible = false;
        player.colorer.SetGlowiness(0);
    }
}
