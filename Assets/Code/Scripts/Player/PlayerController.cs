using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum PlayerStates
    {
        Idle,
        Running,
        Jumping,
        Falling,
        Attacking,
        Hurt,
        Dying
    }

    [Header("State Machine")]
    public PlayerStates currentState = PlayerStates.Idle;

    [Header("Input Data")]
    private float moveInputDirX;
    private int dmgTaken;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Verifications")]
    private bool wannaJump = false;
    private bool gotHit = false;

    [Header("GameFeel")]
    [SerializeField] private float HurtSkipingSeconds = 0.25f;

    private PlayerMovement movementScript;
    private SpriteRenderer renderer;

    void Awake()
    {
        movementScript = GetComponent<PlayerMovement>();
        renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void OnMoveIntent(InputAction.CallbackContext context)
    {
        moveInputDirX = context.ReadValue<Vector2>().x;
    }

    public void OnJumpIntent(InputAction.CallbackContext context)
    {
        // Le preguntamos a los músculos si estamos tocando el suelo
        if (context.started && isGrounded)
        {
            wannaJump = true;
        }

        // Le preguntamos a los músculos nuestra velocidad en Y
        if (context.canceled && currentState == PlayerStates.Jumping)
        {
            movementScript.CutJump();
        }
    }

    private bool CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        return isGrounded;
    }

    private void Update()
    {
        CheckGround();

        CheckStateChanges();

        ExecuteCurrentState();
    }

    private void FixedUpdate()
    {
        ExecuteFixedCurrentState();
    }

    private void CheckStateChanges()
    {
        if (gotHit)
        {
            currentState = PlayerStates.Hurt;
            gotHit = false;

            PlayerStats.Instance.ReduceHPandCheckVitals(dmgTaken);

            renderer.color = Color.red;
            GameFeelManager.Instance.FreezeFrame(HurtSkipingSeconds);

            return; 
        }

        switch (currentState)
        {
            case PlayerStates.Idle:
                if (!isGrounded) currentState = PlayerStates.Falling;
                else if (moveInputDirX != 0) currentState = PlayerStates.Running;
                else if (wannaJump) SwitchToJumpingState();
                break;
            case PlayerStates.Running:
                if (!isGrounded) currentState = PlayerStates.Falling;
                else if (movementScript.completelyStop) currentState = PlayerStates.Idle;
                else if (wannaJump) SwitchToJumpingState();
                break;
            case PlayerStates.Jumping:
                if (!movementScript.stillRaising()) currentState = PlayerStates.Falling;
                break;
            case PlayerStates.Falling:
                if (CheckGround())
                {
                    if (movementScript.completelyStop) currentState = PlayerStates.Idle;
                    else currentState = PlayerStates.Running;
                }
                break;
            case PlayerStates.Hurt:
                StartCoroutine(StunTime());
                break;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case PlayerStates.Idle:
                wannaJump = false;
                break;
            case PlayerStates.Running:
                movementScript.TurningX(moveInputDirX);
                wannaJump = false;
                break;
            case PlayerStates.Jumping:
                break;
            case PlayerStates.Falling:
                break;
        }
    }
    private void ExecuteFixedCurrentState()
    {
        switch (currentState)
        {
            case PlayerStates.Idle:
                movementScript.StopAllMovement();
                break;
            case PlayerStates.Running:
                movementScript.MovementX(moveInputDirX);
                break;
            case PlayerStates.Jumping:
            case PlayerStates.Falling:
                movementScript.MovementX(moveInputDirX);
                break;
        }
    }

    private void SwitchToJumpingState()
    {
        currentState = PlayerStates.Jumping;
        movementScript.Jump();

        wannaJump = false;
    }

    public void TakeDamage(int dmg)
    {
        dmgTaken = dmg;
        gotHit = true;
    }

    IEnumerator StunTime()
    {
        yield return new WaitForSeconds(HurtSkipingSeconds/10);

        renderer.color = Color.white;
        currentState = PlayerStates.Idle;
    }
    
}
