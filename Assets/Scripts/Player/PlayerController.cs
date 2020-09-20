using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Placer))]

public class PlayerController : MonoBehaviour
{
    enum MovementState { STANDING, RUNNING, JUMPING, FALLING };

    private Player player;
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private Collider playerCollider;
    private Placer playerPlacer;

    // button input maps for this player
    private string laneSwitchAxis;
    private string jumpButton;
    private string actionButton;
    private string startButton;

    // internal movement variables for calculations
    private MovementState movementState = MovementState.STANDING;
    private bool isGrounded = false;
    private bool laneSwitchAxisInUse = false;
    private bool jumpAxisInUse = false;
    private bool actionAxisInUse = false;
    private float accumulatedVelocity = 0f;
    private int targetLaneIndex;
    private Vector3 velVec;

    // player movement settings
    private float maxVelocity;
    private bool isSlowed = false;
    [SerializeField] private bool canMove = true;
    [SerializeField] private int startingLaneIndex = 0;
    [SerializeField] private float defaultMaxVelocity = 5f;
    [SerializeField] private float maxLaneSwitchVelocity = 3f;
    [SerializeField] private float jumpForce = 0f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> jumpSounds;
    [Range(0, 1)] private float switchAxisThreshold = 0.01f;
    [SerializeField] private Animator animator;
    [SerializeField] private float deceleration = 2f;

    public Lane TargetLane{
        get{
            return LaneManager.GetInstance().Lanes[targetLaneIndex];
        }
    }

    void OnDisable () 
    {
        // This makes sure that the player can't jump right after
        // getting thrown off a booster
        isGrounded = false;

        animator.SetFloat("Velocity", 0f);
    }

    private void Start ()
    {
        player = GetComponent<Player>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = transform;
        playerCollider = GetComponent<Collider>();
        playerPlacer = GetComponent<Placer>();

        playerRigidbody.useGravity = false;

        maxVelocity = defaultMaxVelocity;
        ResetTargetLane();

        AssignAxes();
    }

    private void Update()
    {   
        if (GameManager.GetInstance().gameState == GameManager.GameState.INPROGRESS)
        {
            HandlePlacementBehavior();
        }
    }

    private void FixedUpdate ()
    {
        if (GameManager.GetInstance().gameState == GameManager.GameState.INPROGRESS)
        {
            animator.SetFloat("Velocity", playerRigidbody.velocity.x / maxVelocity);
            
            velVec = playerRigidbody.velocity;
            HandlePauseBehavior();
            UpdateMovementState();
            HandleJumpBehavior();
            HandleLaneSwitchBehavior();
            HandleMovementBehavior();
            playerRigidbody.velocity = velVec;
        }
    }

    private void AssignAxes()
    {
        laneSwitchAxis = "DPad" + player.playerID.ToString();
        jumpButton = "Jump" + player.playerID.ToString();
        actionButton = "Action" + player.playerID.ToString();
        startButton = "Start" + player.playerID.ToString();
    }

    private void UpdateMovementState()
    {
        // raycast to check if player has landed on the ground
        isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, playerCollider.bounds.extents.y + 0.1f, groundMask);

        if (isGrounded)
        {
            if (playerRigidbody.velocity.magnitude <= 0.1f)
            {
                movementState = MovementState.STANDING;
            }
            else
            {
                movementState = MovementState.RUNNING;
            }
        }
        else
        {
            if (playerRigidbody.velocity.y >= 0f)
            {
                movementState = MovementState.JUMPING;
            }
            else
            {
                movementState = MovementState.FALLING;
            }
        }
    }

    private void HandlePauseBehavior()
    {
        if (Input.GetAxis(startButton) > 0f)
        {
            PauseMenuManager.GetInstance().Pause();
        }
    }

    private void HandleMovementBehavior()
    {
        animator.SetBool("Grounded", isGrounded);
        
        // if the player isn't finished, move the player
        if (canMove)
        {
            // accumulate velocity
            accumulatedVelocity += Time.deltaTime * 5f;

            if (isGrounded)
            {
                accumulatedVelocity = Mathf.Clamp(accumulatedVelocity, 0f, maxVelocity);

                // ignore y velocity (falling, etc) and limit right movement velocity
                velVec.x = accumulatedVelocity;
            }
            else
            {
                if (playerRigidbody.velocity.x < maxVelocity)
                {
                    // ignore y velocity (falling, etc) and limit right movement velocity
                    velVec.x = accumulatedVelocity;
                }
            }
        }

        // Move towards target lane center
        float laneZ = LaneManager.GetInstance().Lanes[targetLaneIndex].center.position.z;
        float zVel = Mathf.Clamp((laneZ - transform.position.z) * maxLaneSwitchVelocity, -maxLaneSwitchVelocity, maxLaneSwitchVelocity);
        velVec.z = zVel;
    }

    private void HandleLaneSwitchBehavior()
    {
        if (Mathf.Abs(Input.GetAxis(laneSwitchAxis)) < switchAxisThreshold)
        {
            laneSwitchAxisInUse = false;
        }

        // check for lane switch up
        if (Input.GetAxis(laneSwitchAxis) > switchAxisThreshold && !laneSwitchAxisInUse)
        {
            laneSwitchAxisInUse = true;
            targetLaneIndex = Mathf.Min(targetLaneIndex + 1, LaneManager.GetInstance().Lanes.Count - 1);
        }

        // check for lane switch down
        if (Input.GetAxis(laneSwitchAxis) < -switchAxisThreshold && !laneSwitchAxisInUse)
        {
            laneSwitchAxisInUse = true;
            targetLaneIndex = Mathf.Max(targetLaneIndex - 1, 0);
        }
    }

    private void HandleJumpBehavior()
    {
        // check if jump axis can be used
        if (Input.GetAxis(jumpButton) > 0f && !jumpAxisInUse)
        {
            // ensure the player isn't falling, jumping or being boosted and is on ground
            if (movementState != MovementState.JUMPING && movementState != MovementState.FALLING && isGrounded)
            {
                animator.SetTrigger("Jump");
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

                // update movement state
                movementState = MovementState.JUMPING;
                isGrounded = false;

                // turn off jump axis input
                jumpAxisInUse = true;

                // play random jump sound
                audioSource.PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Count)]);
            }
        }

        // if the jump button is not pressed, reset use flag
        if (Input.GetAxis(jumpButton) == 0f)
        {
            jumpAxisInUse = false;
        }
    }

    private void HandlePlacementBehavior()
    {
        // display blueprint when action button is held
        if (Input.GetAxis(actionButton) > 0f && !actionAxisInUse)
        {
            actionAxisInUse = true;
            playerPlacer.ActivateBlueprint = true;
        }

        // when action button is let go, place the placeable
        if (actionAxisInUse && Input.GetAxis(actionButton) == 0f)
        {
            actionAxisInUse = false;
            playerPlacer.ActivateBlueprint = false;
            playerPlacer.Place();
        }
    }

    public IEnumerator SlowPlayer(float duration)
    {
        if (isSlowed)
            yield break;

        isSlowed = true;
        int ticks = 10;

        // change in velocity for decay
        float slowTimeDelta = duration / ticks;

        // set player velocity to something low and increase it until default is reached
        for (int i = 0; i < ticks; i++)
        {
            maxVelocity = defaultMaxVelocity / (ticks - i);

            yield return new WaitForSeconds(slowTimeDelta);
        }

        maxVelocity = defaultMaxVelocity;
        isSlowed = false;
    }

    public void ResetPlayerVelocity()
    {
        velVec = Vector3.zero;
        animator.SetFloat("Velocity", velVec.x);
        accumulatedVelocity = 0f;
        playerRigidbody.velocity = velVec;
    }

	public void StopPlayer()
    {
        StartCoroutine(BrakeCoroutine());
	}

    private IEnumerator BrakeCoroutine()
    {
        // add a little random variety to deceleration so that players don't stop movement inside of each other
        float decelerationOffset = Random.Range(0, 1);

        while (playerRigidbody.velocity.x > 0)
        {
            velVec = playerRigidbody.velocity;
            velVec.x = Mathf.Max(0, velVec.x - (deceleration + decelerationOffset) * Time.deltaTime);
            animator.SetFloat("Velocity", velVec.x);
            playerRigidbody.velocity = velVec;
            yield return null;
        }
    }

    public void ResetTargetLane()
    {
        targetLaneIndex = startingLaneIndex;
    }

}
