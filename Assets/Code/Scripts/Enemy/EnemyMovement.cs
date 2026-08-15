using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class EnemyMovement : MonoBehaviour
{
    private EnemyController myController;

    [SerializeField] private float movSpeed;
    private Transform player;
    private Rigidbody2D rb;

    [Header("Ground Check")]
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private Vector2 checkOffset; // Guardará la distancia real del groundCheck
    [SerializeField] private LayerMask groundLayer;

    public bool playerOnRight { get; private set; }

    private float currentMoveDir = 1f;

    void Start()
    {
        //Get the references
        myController = GetComponent<EnemyController>();
        player = FindAnyObjectByType<PlayerMovement>().transform;
        rb = GetComponent<Rigidbody2D>();

        TurnToPlayer();

        StartCoroutine(TurnToPlayerRoutine());
    }

    public void StopMoving() => rb.velocity = Vector2.zero;

    public void TurnToPlayer()
    {
        //Target the player
        playerOnRight = player.position.x > transform.position.x;

        //Move to player
        currentMoveDir = playerOnRight ? 1 : -1;
        rb.velocity = new Vector2(currentMoveDir * movSpeed, rb.velocity.y);

        AlignEyesAndLegs();

    }

    public void AlignEyesAndLegs()
    {
        if (currentMoveDir != transform.localScale.x) Flip();
    }
    public void Patrolling()
    {
        if (CheckEndOfPlatform(currentMoveDir)) Turn(true);
    }

    public void Targeting()
    {
        if (CheckPlayerPosChange())
        {
            Flip();
        }

        if (CheckEndOfPlatform(currentMoveDir)) Turn(false);
    }

    private void Turn(bool andFlip)
    {
        if (andFlip) Flip();
        currentMoveDir *= -1f;
        rb.velocity = new Vector2(currentMoveDir * movSpeed, rb.velocity.y);
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
    }

    private bool CheckPlayerPosChange()
    {
        if (player != null)
        {
            bool actuallyOnRight = player.position.x > transform.position.x;

            if (actuallyOnRight != playerOnRight)
            {
                playerOnRight = actuallyOnRight;
                return true;
            }
        }

        return false;
    }

    IEnumerator TurnToPlayerRoutine()
    {
        while (true)
        {
            if (player != null)
            {
                if (myController.currentState == EnemyController.EnemyStates.isTargeting)
                {
                    if (currentMoveDir != transform.localScale.x)
                    {
                        Turn(false);
                        print("Turn on routine");
                    }
                }
            }

            float waitTime = (Random.Range(
                1f, 4f) + 2f) / 3f;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private bool CheckEndOfPlatform(float movementDirection)
    {
        if (movementDirection == 0) return false;

        Vector2 dynamicCheckPosition = new Vector2(
            transform.position.x + (checkOffset.x * movementDirection),
            transform.position.y + checkOffset.y);
        return !Physics2D.OverlapCircle(dynamicCheckPosition, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (rb == null) return;
        float movementDirection = Mathf.Sign(rb.velocity.x);
        Vector2 dynamicCheckPosition = new Vector2(
        transform.position.x + (checkOffset.x * currentMoveDir),
        transform.position.y + checkOffset.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(dynamicCheckPosition, checkRadius);
    }

}
