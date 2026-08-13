using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    protected EnemyController myController;

    [SerializeField] private Vector2 attackCoolDownMinMax;
    [SerializeField] protected float TelegraphTime = 0.75f;

    private void Start()
    {
        myController = GetComponent<EnemyController>();
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(Random.Range(attackCoolDownMinMax.x,attackCoolDownMinMax.y));
        if (myController.currentState == EnemyController.EnemyStates.isTargeting)
        {
            print("Shoot");
            myController.currentState = EnemyController.EnemyStates.isAboutToAttack;
            StartCoroutine(PerformAttackRoutine());
        }
        StartCoroutine(AttackRoutine());
    }

    protected virtual IEnumerator PerformAttackRoutine()
    {
        myController.currentState = EnemyController.EnemyStates.isPatrolling;
        yield return null;
    }

}
