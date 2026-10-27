using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private EnemyController enemyController;

    private void Awake()
    {
        currentHealth = maxHealth;
        enemyController = GetComponent<EnemyController>();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " recibió " + damageAmount + " de daño. Salud restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            if (enemyController != null)
            {
                enemyController.OnHit();
            }
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");
        if (enemyController != null)
        {
            enemyController.Die();
        }
        // Opcional: Destruir el objeto después de un tiempo o animación
        // Destroy(gameObject, 1f); 
    }
}
