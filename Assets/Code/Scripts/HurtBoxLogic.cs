using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBoxLogic : MonoBehaviour
{
    private enum LogicOwners
    {
        Player,
        Enemy
    }

    [SerializeField] private LogicOwners logicOwner;

    private EnemyController enemyBrain;
    private PlayerController playerBrain;

    void Awake()
    {
        playerBrain = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerBrain != null) 
        {
            if (collision.CompareTag("EnemyBullet"))
            {
                playerBrain.TakeDamage(PlayerStats.Instance.suyo);
            }
        }
    }
}
