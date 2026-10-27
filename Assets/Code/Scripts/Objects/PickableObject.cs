using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{

    [SerializeField] private PlayerStats.Stats stat;
    [SerializeField] private int amount = 1;

    [SerializeField] 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats.Instance.AddByStat(stat,amount);

            Destroy(gameObject);
        }
    }
}
