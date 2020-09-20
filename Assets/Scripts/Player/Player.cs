using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Placer))]
[RequireComponent(typeof(ScorePopupSpawner))]
public class Player : MonoBehaviour
{
    public enum PlayerState { ALIVE, DEAD, FINISHED };

    public int playerID;

    private int _score = 0;
    public int score {
        get {
            return _score;
        }
        set{
            SetPoints(value);
        }
    }

    [SerializeField] private GameObject scorePopupPrefab;
    public int startingDeaths = 0;
    public int playerDeaths;
    public float respawnTime = 3f;
    public Color playerColor;
    public bool invincible = false;
    public Component[] renderers;
    public Coroutine invincibilityCoroutine;

    private PlayerState _playerState = PlayerState.ALIVE;
    public PlayerState playerState{
        get{
            return _playerState;
        }
        set{
            switch(value){
                case PlayerState.FINISHED:
                    controller.enabled = false;
                    controller.StopPlayer();
                    break;
                case PlayerState.ALIVE:
                    renderers = this.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mesh in renderers)
                        mesh.enabled = true;
                    controller.enabled = true;
                    break;
            }
            _playerState = value;
        }
    }

    public PlayerController controller;
    public Placer placer;
    public PlayerColorer colorer;
    private ScorePopupSpawner popupSpawner;

    void Start(){
        popupSpawner = GetComponent<ScorePopupSpawner>();
        colorer = GetComponent<PlayerColorer>();
    }

    public void CreatePlayer(int id, int deaths)
    {
        playerID = id;
        playerDeaths = deaths;
        playerState = PlayerState.ALIVE;
    }
    
    // Sets player score to new value and displays gained/lost points on the screen
    private void SetPoints(int points)
    {
        int delta = points - _score;
        StartCoroutine(popupSpawner.SpawnPopup(delta));
        _score = Mathf.Max(points, 0);
    }

    public void ToggleInvincibility(bool isInvincible)
    {
        invincible = isInvincible;
    }

    public IEnumerator ToggleInvincibilityWithDelayCoroutine(bool isInvincible, float delay)
    {
        if (!isInvincible)
        {
            yield return new WaitForSeconds(delay);
        }
        ToggleInvincibility(isInvincible);
    }

    public void Blink()
    {
        StartCoroutine(blinkInterval(0.1f, this));
    }

    private IEnumerator blinkInterval(float secs, Player player)
    {
        renderers = player.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mesh in renderers)
            mesh.enabled = false;

        yield return new WaitForSeconds(secs);

        foreach (MeshRenderer mesh in renderers)
            mesh.enabled = true;
    }

    public IEnumerator PostDeathImmunity(float secs, Player player)
    {
        player.invincible = true;
        InvokeRepeating("Blink", 0, 0.4f);

        yield return new WaitForSeconds(secs);

        CancelInvoke("Blink");
        player.invincible = false;

    }
}
