using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private PlayerManager playerManager;

    public enum GameState { PREPARING, INPROGRESS, FINISHED };
    public GameState _gameState = GameState.PREPARING;
    public GameState gameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            _gameState = value;
        }
    }

    private List<Player> playersFinished = new List<Player>();
    public CameraController cameraController;
    public WallOfDeathMovement wallOfDeath;

    public UnityEvent onGameEnd;
    public GameHUD gameHUD;
    public GameObject scoreDisplayCanvas;
    public ScoreDisplayManager scoreDisplayManager;
    public float scoreDisplayDelay = 2f;
    public int roundCount = 0;
    public int maxRoundCount = 5;

    [SerializeField] private RoundStartCountdown roundStartCountdown;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victorySound;

    public PlaceableData placeables;

    // Determines the speed of the WoD based on the number of player that have finished
    [SerializeField] float wallOfDeathCreepMultiplier = 2f;
    //[SerializeField] float roundStartCountdown = 5f;

    public static GameManager GetInstance()
    {
        return instance;
    }

    void Awake(){
        // Enforce singleton pattern
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerManager = PlayerManager.GetInstance();

        // start the game
        playerManager.SpawnPlayers(JoystickScanner.readyPlayerIDs);
        StartRound();
    }

    public void StartRound()
    {
        LaneManager.GetInstance().SpawnPickups();
        List<Player> players = playerManager.players;
        gameHUD.SetupHUD(players);
        RoundStartCountdownCoroutine();
        roundCount++;
        
        FindObjectOfType<RoundHUD>().UpdateText();
    }

    public void SetPlayerFinished(Player player)
    {
        player.playerState = Player.PlayerState.FINISHED;
        playersFinished.Add(player);

        CalculatePlayerScore(player);

        CheckRoundStatus();
    }
    
    public void CheckRoundStatus()
    {
        if(PlayerManager.GetInstance().players.All(
            p => p.playerState == Player.PlayerState.FINISHED)){

            //game over after five rounds
            if (roundCount < maxRoundCount){
                PostRoundCoroutine();
            }
            else{
                wallOfDeath.enabled = false;
                StartCoroutine(ScoreDisplayDelay());
            }
        }

        else
        {
            int numFinishedPlayers = PlayerManager.GetInstance().players
                                        .Count(p => p.playerState == Player.PlayerState.FINISHED);
        }
    }

    private void CalculatePlayerScore(Player player)
    {
        player.score += 50 / playersFinished.Count;
    }

    private void PostRoundCoroutine()
    {
        StartCoroutine(ScoreDisplayDelay());
        playersFinished = new List<Player>();

        // play victory sound
        audioSource.PlayOneShot(victorySound);

        playerManager.ResetPlaceables();
        LaneManager.GetInstance().SpawnPickups();
        GameManager.GetInstance().wallOfDeath.enabled = false;
    }

    private IEnumerator ScoreDisplayDelay()
    {
        scoreDisplayCanvas.gameObject.SetActive(true);
        if(roundCount == maxRoundCount){
            scoreDisplayManager.PopulateFinalScores();
        }

        else{
            scoreDisplayManager.PopulateScores();
        }

        yield return new WaitForSeconds(scoreDisplayDelay);
        while (!Input.GetButtonDown("DismissScore"))
        {
            yield return null;
        }

        if (roundCount == maxRoundCount){
            roundCount = 0;
            scoreDisplayManager.CancelScoreBlink();
            PlayerManager.GetInstance().ResetScores();
            scoreDisplayCanvas.gameObject.SetActive(false);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        else{
            roundCount++;
            scoreDisplayCanvas.gameObject.SetActive(false);
            PlayerManager.GetInstance().RespawnAllPlayers();
            wallOfDeath.enabled = true;
            wallOfDeath.RestartPosition();
            RoundStartCountdownCoroutine();

            FindObjectOfType<RoundHUD>().UpdateText();
        }

    }

	private void RoundStartCountdownCoroutine() {
		roundStartCountdown.gameObject.SetActive(true);
	}

}
