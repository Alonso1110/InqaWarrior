using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [SerializeField] private int playerHealthPoints;
    [SerializeField] private int playerDamagePoints;
    [SerializeField] public int playerCash { get; private set; }

    [SerializeField] public int level { get; private set; }
    [SerializeField] public int suyo { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        } 
        else Destroy(gameObject);

        playerHealthPoints = 5;
        playerDamagePoints = 2;
        playerCash = 0;
        level = 1;
        suyo = 1;

    }
    public void AddHP(int amount)
    {
        playerHealthPoints += amount;
    }
    public void AddDmgPoints(int amount)
    {
        playerDamagePoints += amount;
    }
    public void AddCash(int amount)
    {
        playerCash += amount;
    }

    public bool ReduceHPandCheckVitals(int amount)
    {
        int newTotal = playerHealthPoints - amount;
        if (newTotal > 0)
        {
            playerHealthPoints = newTotal;
        }
        else
        {
            playerHealthPoints = 0;
            return false;
        }
        return true;
    }
    public void ReduceDmgPoints(int amount)
    {
        int newTotal = playerDamagePoints - amount;
        if (newTotal > 0)
        {
            playerDamagePoints = newTotal;
        }
        else
        {
            playerDamagePoints = 0;
        }
    }
    public bool CheckCashToSpend(int amount)
    {
        int newTotal = playerDamagePoints - amount;
        if (newTotal >= 0)
        {
            playerDamagePoints = newTotal;
            return true;
        }
        return false;

    }

}
