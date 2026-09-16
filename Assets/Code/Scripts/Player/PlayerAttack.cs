using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private LayerMask enemyMask;
    private PlayerController playerController;

    [Header("Melee Attack")]
    [SerializeField] private Transform meleeOrigin;
    [SerializeField] private float maxMeleeRadius = 2;
    [SerializeField] private float meleeDmgTime = 1;

    private float currentMeleeRadius = 0;

    public enum PlayerAttacks
    {
        Melee1,
        Melee2,
        Range,
        Potion,
        Sun

    }

    public PlayerAttacks currentAttack = PlayerAttacks.Melee1;

    private Coroutine activeAttackRoutine;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        currentMeleeRadius = 0;
    }

    public void Attack()
    {
        activeAttackRoutine = StartCoroutine(ExpandingMeleeAttackRoutine());
    }




    public void InterruptAttack()
    {
        if (activeAttackRoutine != null)
        {
            StopCoroutine(activeAttackRoutine);
            currentMeleeRadius = 0;
        }
    }

    private IEnumerator ExpandingMeleeAttackRoutine()
    {
        float timer = 0f;
        currentMeleeRadius = 0f;

        HashSet<Collider2D> enemiesHit = new HashSet<Collider2D>();

        while (timer < meleeDmgTime)
        {
            timer += Time.deltaTime;

            currentMeleeRadius = Mathf.Lerp(0f, maxMeleeRadius, timer / meleeDmgTime);

            MeleeDetect(enemiesHit);

            yield return null;
        }

        currentMeleeRadius = 0f;

        playerController.EndAttack();
    }

    private void MeleeDetect(HashSet<Collider2D> alreadyHitEnemies)
    {
        Collider2D[] enemiesRange = Physics2D.OverlapCircleAll(meleeOrigin.position, currentMeleeRadius, enemyMask);
        foreach (Collider2D enemy in enemiesRange)
        {
            if (!alreadyHitEnemies.Contains(enemy))
            {
                alreadyHitEnemies.Add(enemy); 

                // TODO: Aquí va tu lógica para hacer daño al enemigo.
                Debug.Log("¡Golpeado: " + enemy.name + "!");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(meleeOrigin.position, maxMeleeRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeOrigin.position, currentMeleeRadius);
    }

}
