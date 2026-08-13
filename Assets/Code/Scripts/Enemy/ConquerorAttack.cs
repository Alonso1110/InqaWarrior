using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConquerorAttack : EnemyAttack
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed;

    protected override IEnumerator PerformAttackRoutine()
    {
        EnemyMovement movement = GetComponent<EnemyMovement>();
        movement.StopMoving();
        GetComponent<SpriteRenderer>().color = Color.blue; //Telegraph animation

        yield return new WaitForSeconds(TelegraphTime); 

        if (projectilePrefab != null && firePoint != null)
        {
            GameObject x = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            Rigidbody2D rb = x.GetComponent<Rigidbody2D>();

            if (x.GetComponent<Rigidbody2D>() == null) yield break;

            bool goingRight = movement.playerOnRight;
            rb.velocity = new Vector2(projectileSpeed * (goingRight? 1:(-1)) , 0);

            Destroy(x,10);



        }
        else
        {
            Debug.LogWarning("Cuidado: Falta asignar el proyectil o el firePoint en el Inspector.");
        }

        movement.TurnToPlayer();
        GetComponent<SpriteRenderer>().color = Color.red;

        StartCoroutine(base.PerformAttackRoutine());
    }
}
