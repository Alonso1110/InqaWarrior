using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    public enum Stats
    {
        HP,
        Dmg,
        Cash
    }

    [SerializeField] private int initialHP;
    [SerializeField] private int initialDmg;
    [SerializeField] private int initialCash;

    public int playerHealthPoints { get; private set; }
    public int playerDamagePoints { get; private set; }
    public int playerCash { get; private set; }

    public int level { get; private set; }
    public int suyo { get; private set; }
    public int roomsWithoutFight { get; private set; }

    [Header("Final Screen Specs")]
    [SerializeField] private int SpanishMurdered = 0;
    [SerializeField] private int GoldRecovered = 0;
    [SerializeField] private float TimeSpentTillVictory = 0f;

    public event Action<int> OnHealthChange;
    public event Action<int> OnDamageChange;
    public event Action<int> OnCashChange;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        } 
        else Destroy(gameObject);

        playerHealthPoints = initialHP;
        playerDamagePoints = initialDmg;
        playerCash = initialCash;
        level = 1;
        suyo = 1;

    }
    public void AddHP(int amount)
    {
        playerHealthPoints += amount;
        OnHealthChange?.Invoke(playerHealthPoints);
    }
    public void AddDmgPoints(int amount)
    {
        playerDamagePoints += amount;
        OnDamageChange?.Invoke(playerDamagePoints);
    }
    public void AddCash(int amount)
    {
        playerCash += amount;
        GoldRecovered += amount;
        OnCashChange?.Invoke(playerCash);
    }

    public bool ReduceHPandCheckVitals(int amount)
    {
        int newTotal = playerHealthPoints - amount;
        if (newTotal > 0)
        {
            playerHealthPoints = newTotal;
            OnHealthChange?.Invoke(playerHealthPoints);
        }
        else
        {
            playerHealthPoints = 0;
            OnHealthChange?.Invoke(playerHealthPoints);
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
        OnDamageChange?.Invoke(playerDamagePoints);
    }
    public bool CheckCashToSpend(int amount)
    {
        int newTotal = playerCash - amount;
        if (newTotal >= 0)
        {
            playerCash = newTotal;
            OnCashChange?.Invoke(playerCash);
            return true;
        }
        return false;

    }

    public void AddByStat(Stats modifiedStat, int amount)
    {
        switch (modifiedStat)
        {
            case Stats.HP: AddHP(amount); break;
            case Stats.Dmg: AddDmgPoints(amount); break;
            case Stats.Cash: AddCash(amount); break;
            default: break;
        }
    }

    public void ResetRoomsWithoutFight() => roomsWithoutFight = 0;
    public void OneMoreRoomWithoutFight() => roomsWithoutFight++;

    private void KillConfirmed()
    {
        SpanishMurdered++;
    }

}
