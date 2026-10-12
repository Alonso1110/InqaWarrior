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
    private bool gotHit;
    public bool wannaAttack { get; private set; }
    public bool wannaJump { get; private set;}

    [Header("GameFeel")]
    [SerializeField] private float HurtSkipingSeconds = 0.25f;

    private PlayerMovement movementScript;
    private PlayerAttack attackScript;
    private SpriteRenderer renderer;

    void Awake()
    {
        movementScript = GetComponent<PlayerMovement>();
        attackScript = GetComponent<PlayerAttack>();
        renderer = GetComponentInChildren<SpriteRenderer>();

        wannaAttack = false;
        gotHit = false;
        wannaJump = false;
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
            attackScript.InterruptAttack();
            SwitchToState(PlayerStates.Hurt);
            return; 
        }

        switch (currentState)
        {
            case PlayerStates.Idle:
                if (!isGrounded)
                {
                    bodyAnim.Play("ToFall");
                    bottomAnim.Play("None");
                }
                else if (wannaAttack)
                {
                    SwitchToState(PlayerStates.IdleAttacking);
                }
                else if (moveInputDirX != 0)
                {
                    bodyAnim.Play("IdleToRun");
                    bottomAnim.Play("None");
                    currentState = PlayerStates.Running;
                }
                else if (wannaJump) SwitchToState(PlayerStates.Jumping);
                break;
            case PlayerStates.Running:
                if (!isGrounded)
                {
                    bodyAnim.Play("ToFall");
                    bottomAnim.Play("None");
                }
                else if (wannaAttack)
                {
                    SwitchToState(PlayerStates.RunAttacking);
                }
                else if (moveInputDirX == 0)
                {
                    bodyAnim.Play("RunToIdle");
                    bottomAnim.Play("None");
                }
                else if (wannaJump) SwitchToState(PlayerStates.Jumping);
                break;
            case PlayerStates.Jumping:
                if (!movementScript.stillRaising())
                {
                    bodyAnim.Play("ToFall");
                    bottomAnim.Play("None");
                    if (isGrounded)
                    {
                        if (movementScript.completelyStop)
                        {
                            bodyAnim.Play("FallToIdle");
                            bottomAnim.Play("None", 0, 0);
                            currentState = PlayerStates.Idle;
                        }
                        else
                        {
                            bodyAnim.Play("FallToRun");
                            bottomAnim.Play("None", 0, 0);
                            currentState = PlayerStates.Running;
                        }
                    }
                } else if (wannaAttack)
                {
                    SwitchToState(PlayerStates.FallAttacking); 
                }
                break;
            case PlayerStates.Falling:
                if (isGrounded)
                {
                    if (movementScript.completelyStop)
                    {
                        bodyAnim.Play("FallToIdle");
                        bottomAnim.Play("None",0,0);
                        currentState = PlayerStates.Idle;
                    }
                    else
                    {
                        bodyAnim.Play("FallToRun");
                        bottomAnim.Play("None",0,0);
                        currentState = PlayerStates.Running;
                    }
                } else if (wannaAttack)
                {
                    SwitchToState(PlayerStates.FallAttacking); 
                }
                break;
            case PlayerStates.Hurt:
                break;
            case PlayerStates.IdleAttacking:
                if (!isGrounded)
                {
                    bottomAnim.Play("Fall");
                    currentState = PlayerStates.FallAttacking;
                }
                else if (moveInputDirX != 0)
                {
                    bottomAnim.Play("Run");
                    currentState = PlayerStates.RunAttacking;
                }
                break;
            case PlayerStates.RunAttacking:
                if (!isGrounded)
                {
                    bottomAnim.Play("Fall");
                    currentState = PlayerStates.FallAttacking;
                }
                else if (movementScript.completelyStop)
                {
                    bottomAnim.Play("Attack1");
                    currentState = PlayerStates.IdleAttacking;
                }
                break;
            case PlayerStates.FallAttacking:
                if (isGrounded)
                {
                    if (movementScript.completelyStop)
                    {
                        bottomAnim.Play("Attack1");
                        currentState = PlayerStates.IdleAttacking;
                    }
                    else
                    {
                        bottomAnim.Play("Run");
                        currentState = PlayerStates.RunAttacking;
                    }
                }
                break;
            default:
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
            default:
                break;
        }
    }

    private void ExecuteFixedCurrentState()
    {
        switch (currentState)
        {
            case PlayerStates.IdleAttacking:
            case PlayerStates.Idle:
                movementScript.StopAllMovement();
                break;
            case PlayerStates.RunAttacking:
            case PlayerStates.Running:
                movementScript.MovementX(moveInputDirX);
                break;
            case PlayerStates.Jumping:
            case PlayerStates.FallAttacking:
            case PlayerStates.Falling:
                movementScript.MovementX(moveInputDirX);
                break;
            default:
                break;
        }
    }

    public void SwitchToState(PlayerStates newState)
    {
        switch (newState)
        {
            case PlayerStates.Idle:
                bodyAnim.Play("Idle");
                bottomAnim.Play("None");
                break;
            case PlayerStates.Running:
                bodyAnim.Play("Run");
                bottomAnim.Play("Run");
                break;
            case PlayerStates.Jumping:
                bodyAnim.Play("Jump");
                bottomAnim.Play("Fall");

                movementScript.Jump();

                wannaJump = false;
                break;
            case PlayerStates.Falling:
                bodyAnim.Play("Fall");
                bottomAnim.Play("Fall");
                break;
            case PlayerStates.Hurt:
                gotHit = false;

                PlayerStats.Instance.ReduceHPandCheckVitals(dmgTaken);

                renderer.color = Color.red;
                GameFeelManager.Instance.FreezeFrame(HurtSkipingSeconds);

                StartCoroutine(StunTime());
                break;
            case PlayerStates.IdleAttacking:
                bodyAnim.Play("Attack1");
                bottomAnim.Play("Attack1");
                smearAnim.Play("Smear1", 0, 0);
                break;
            case PlayerStates.RunAttacking:
                bodyAnim.Play("Attack1");
                bottomAnim.Play("Run");
                smearAnim.Play("Smear1", 0, 0);
                break;
            case PlayerStates.FallAttacking:
                bodyAnim.Play("Attack1");
                bottomAnim.Play("Fall");
                smearAnim.Play("Smear1", 0, 0);
                break;
            default:
                break;
        }


        currentState = newState;

    }

    public void StartMeleeAttack(bool combo)
    {
        if (!combo) attackScript.Attack(PlayerAttack.PlayerAttacks.Melee1);
        else attackScript.Attack(PlayerAttack.PlayerAttacks.Melee2);
    }

    public void EndMeleeAttack()
    {
        Debug.Log("Termino ataque");
        Debug.Log(attackScript.currentAttack == PlayerAttack.PlayerAttacks.Melee2);
        if (!wannaAttack || attackScript.currentAttack == PlayerAttack.PlayerAttacks.Melee2)
        {
            Debug.Log("Desactivo ataque");
            attackScript.currentAttack = PlayerAttack.PlayerAttacks.Melee1;
            switch (currentState)
            {
                case PlayerStates.IdleAttacking:
                    Debug.Log("Voy a Idle");
                    SwitchToState(PlayerStates.Idle);
                    break;
                case PlayerStates.RunAttacking:
                    SwitchToState(PlayerStates.Running);
                    break;
                case PlayerStates.FallAttacking:
                    SwitchToState(PlayerStates.Falling);
                    break;
                default:
                    break;
            }
        } else
        {
            switch (currentState)
            {
                case PlayerStates.IdleAttacking:
                    bodyAnim.Play("Attack2");
                    bottomAnim.Play("Attack2");
                    smearAnim.Play("Smear2", 0, 0);
                    break;
                case PlayerStates.RunAttacking:
                    bodyAnim.Play("Attack2");
                    bottomAnim.Play("Run");
                    smearAnim.Play("Smear2", 0, 0);
                    break;
                case PlayerStates.FallAttacking:
                    bodyAnim.Play("Attack2");
                    bottomAnim.Play("Fall");
                    smearAnim.Play("Smear2", 0, 0);
                    break;
                default:
                    break;
            }
        }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

}
