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
        IdleAttacking,
        RunAttacking,
        FallAttacking,
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

    [Header("Animations")]
    [SerializeField] private Animator bodyAnim;
    [SerializeField] private Animator bottomAnim;
    [SerializeField] private Animator capeAnim;
    [SerializeField] private Animator smearAnim;

    [Header("Verifications")]
    private bool wannaJump = false;
    private bool wannaAttack = false;
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

    public void OnAttackIntent(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            wannaAttack = true;
        }
        if (context.canceled)
        {
            wannaAttack = false;
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
            SwitchToState(PlayerStates.Hurt);
            return; 
        }

        switch (currentState)
        {
            case PlayerStates.Idle:
                if (!isGrounded) SwitchToState(PlayerStates.Falling);
                else if (wannaAttack) SwitchToState(PlayerStates.IdleAttacking);
                else if (moveInputDirX != 0) SwitchToState(PlayerStates.Running);
                else if (wannaJump) SwitchToState(PlayerStates.Jumping);
                break;
            case PlayerStates.Running:
                if (!isGrounded) SwitchToState(PlayerStates.Falling);
                else if (movementScript.completelyStop) SwitchToState(PlayerStates.Idle);
                else if (wannaJump) SwitchToState(PlayerStates.Jumping);
                break;
            case PlayerStates.Jumping:
                if (!movementScript.stillRaising()) SwitchToState(PlayerStates.Falling);
                break;
            case PlayerStates.Falling:
                if (CheckGround())
                {
                    if (movementScript.completelyStop) SwitchToState(PlayerStates.Idle);
                    else SwitchToState(PlayerStates.Running);
                }
                break;
            case PlayerStates.Hurt:
                break;
            case PlayerStates.IdleAttacking:
                if (!wannaAttack)
                {
                    SwitchToState(PlayerStates.Idle);
                    smearAnim.Play("smearIdle");

                }
                else if (moveInputDirX != 0) SwitchToState(PlayerStates.Running);
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

    private void SwitchToState(PlayerStates newState)
    {
        switch (newState)
        {
            case PlayerStates.Idle:
                bodyAnim.Play("idle");
                bottomAnim.Play("idle");
                break;
            case PlayerStates.Running:
                bodyAnim.Play("run");
                bottomAnim.Play("run");
                break;
            case PlayerStates.Jumping:
                bodyAnim.Play("jump");
                bottomAnim.Play("fall");

                movementScript.Jump();

                wannaJump = false;
                break;
            case PlayerStates.Falling:
                bodyAnim.Play("fall");
                bottomAnim.Play("fall");
                break;
            case PlayerStates.Hurt:
                gotHit = false;

                PlayerStats.Instance.ReduceHPandCheckVitals(dmgTaken);

                renderer.color = Color.red;
                GameFeelManager.Instance.FreezeFrame(HurtSkipingSeconds);

                StartCoroutine(StunTime());
                break;
            case PlayerStates.IdleAttacking:
                bodyAnim.Play("idleAttack1a");
                bottomAnim.Play("idleAttack1a");
                smearAnim.Play("smearAttack");
                break;
            default:
                break;
        }


        currentState = newState;

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
