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

        // Si estamos muertos o recibiendo daño, no cambiamos a patrullar ni atacar
        //if (currentState == EnemyStates.death || currentState == EnemyStates.onHit) return;
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
}
