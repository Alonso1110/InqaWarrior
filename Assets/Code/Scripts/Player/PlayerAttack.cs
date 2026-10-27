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
    [SerializeField] private float melee2DmgTime = 1;
    [SerializeField] private float comboDmgMultiplier = 1.25f;

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

    public void Attack(PlayerAttacks attackType)
    {
        currentAttack = attackType;
        InterruptAttack();

        if (currentAttack == PlayerAttacks.Melee1 || currentAttack == PlayerAttacks.Melee2)
            activeAttackRoutine = StartCoroutine(ExpandingMeleeAttackRoutine(attackType));
    }




    public void InterruptAttack()
    {
        if (activeAttackRoutine != null)
        {
            StopCoroutine(activeAttackRoutine);
            currentMeleeRadius = 0;
        }
    }

    private IEnumerator ExpandingMeleeAttackRoutine(PlayerAttacks attackType)
    {
        float timer = 0f;
        currentMeleeRadius = 0f;

        float attackDuration = (attackType == PlayerAttacks.Melee1) ? meleeDmgTime : melee2DmgTime;
        float dmgMultiplier = (attackType == PlayerAttacks.Melee2) ? comboDmgMultiplier : 1;

        HashSet<Collider2D> enemiesHit = new HashSet<Collider2D>();

        while (timer < attackDuration)
        {
            timer += Time.deltaTime;

            currentMeleeRadius = Mathf.Lerp(0f, maxMeleeRadius, timer / attackDuration);

            Debug.Log("Multi : " + dmgMultiplier + " | BaseDmg : " + PlayerStats.Instance.playerDamagePoints);
            MeleeDetect(enemiesHit, dmgMultiplier);

            yield return null;
        }

        currentMeleeRadius = 0f;

        playerController.EndMeleeAttack();
    }

    private void MeleeDetect(HashSet<Collider2D> alreadyHitEnemies, float damageMultiplier)
    {
        Debug.Log("MELEEEEE");
        Collider2D[] enemiesRange = Physics2D.OverlapCircleAll(meleeOrigin.position, currentMeleeRadius, enemyMask);
        foreach (Collider2D enemy in enemiesRange)
        {
            Debug.Log("MELEEEEE 2!");
            if (!alreadyHitEnemies.Contains(enemy))
            {
                alreadyHitEnemies.Add(enemy);

                EnemyStats stats = enemy.GetComponent<EnemyStats>();

                if (stats != null)
                {
                    int finalDamage = Mathf.RoundToInt(PlayerStats.Instance.playerDamagePoints * damageMultiplier);
                    stats.TakeDamage(finalDamage);
                } else
                {
                    IInteractable interactable = enemy.GetComponent<IInteractable>();
                    if (interactable != null) interactable.Interact();
                }
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
