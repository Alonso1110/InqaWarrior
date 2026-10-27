using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerStats.Stats stat;
    [SerializeField] private int amount;
    [SerializeField] private int cost = 1;

    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = cost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (!PlayerStats.Instance.CheckCashToSpend(cost)) return;
        PlayerStats.Instance.AddByStat(stat, amount);
        Destroy(gameObject);
    }
}
