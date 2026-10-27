using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyStates
    {
        isPatrolling,
        isTargeting,
        isAboutToAttack,
        isAttacking,
        isOnHit,
        isDying

    }

    public EnemyStates currentState = EnemyStates.isPatrolling;

    private EnemyMovement movementScript;
    private EnemyAttack attackScript;

    private Transform player;

    [SerializeField] private float verticalDetection = 3;
    [SerializeField] private float stunTime = 1;

    private Coroutine OnHitRoutine;
    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();
        attackScript = GetComponent<EnemyAttack>();
        player = FindAnyObjectByType<PlayerMovement>().transform;

    }

    void Update()
    {
        if (player == null) return;

        CheckStateChanges();

        ExecuteCurrentState();
    }

    private void CheckStateChanges()
    {
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);

        if (currentState == EnemyStates.isDying || currentState == EnemyStates.isOnHit) return;
        switch (currentState)
        {
            case EnemyStates.isPatrolling:
                if (heightDifference < verticalDetection)
                {
                    movementScript.TurnToPlayer();
                    currentState = EnemyStates.isTargeting;
                }
                break;
            case EnemyStates.isTargeting:
                if (heightDifference >= verticalDetection)
                {
                    movementScript.AlignEyesAndLegs();
                    currentState = EnemyStates.isPatrolling;
                }
                break;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case EnemyStates.isPatrolling:
                movementScript.Patrolling();
                break;

            case EnemyStates.isTargeting:
                movementScript.Targeting();
                break;
        }
    }

    public void OnHit()
    {
        if (currentState != EnemyStates.isDying)
        {
            currentState = EnemyStates.isOnHit;
            if (movementScript != null)
            {
                movementScript.StopMoving(); 
            }
            Debug.Log("Enemigo aturdido por el golpe.");
            if (OnHitRoutine != null) StopCoroutine(OnHitRoutine);
            OnHitRoutine = StartCoroutine(RecoverFromHit()); 
        }
    }

    IEnumerator RecoverFromHit()
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.color = Color.red;
        yield return new WaitForSeconds(stunTime);
        if (currentState != EnemyStates.isDying)
        {
            currentState = EnemyStates.isPatrolling;
            render.color = Color.gray;
        }
        
    }

    public void Die()
    {
        currentState = EnemyStates.isDying;
        GetComponent<SpriteRenderer>().color = Color.black;
        if (movementScript != null)
        {
            movementScript.StopMoving();
        }
        Debug.Log("Enemigo cambiando al estado isDying.");
    }
}
