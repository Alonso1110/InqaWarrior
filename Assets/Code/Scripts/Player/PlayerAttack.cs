using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private LayerMask enemyMask;

    [Header("Melee Attack")]
    [SerializeField] private Transform meleeOrigin;
    [SerializeField] private float meleeRadius;

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

    public void Attack()
    {

    }




    public void InterruptAttack()
    {
        StopCoroutine(activeAttackRoutine);
    }

    private void MeleeDetect()
    {
        Collider2D[] enemiesRange = Physics2D.OverlapCircleAll(meleeOrigin.position, meleeRadius, enemyMask);
        foreach (Collider2D enemy in enemiesRange)
        {
            //Hacer daño a los enemigos
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeOrigin.position, meleeRadius);
    }

}
