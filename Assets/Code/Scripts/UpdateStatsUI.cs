using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStatsUI : MonoBehaviour
{
    [SerializeField] private GameObject prefabHP;
    [SerializeField] private GameObject prefabDmg;
    [SerializeField] private GameObject prefabCash;

    [SerializeField] private Transform HPBar;
    [SerializeField] private Transform DmgBar;
    [SerializeField] private Transform CashBar;

    private void Start()
    {
        PlayerStats.Instance.OnHealthChange += UpdateHPInUI;
        PlayerStats.Instance.OnDamageChange += UpdateDmgInUI;
        PlayerStats.Instance.OnCashChange += UpdateCashInUI;
    }

    private void OnDisable()
    {
        PlayerStats.Instance.OnHealthChange -= UpdateHPInUI;
        PlayerStats.Instance.OnDamageChange -= UpdateDmgInUI;
        PlayerStats.Instance.OnCashChange -= UpdateCashInUI;
    }

    private void UpdateHPInUI(int newHP) => UpdateUI(newHP, 0);
    private void UpdateDmgInUI(int newDmg) => UpdateUI(newDmg, 1);
    private void UpdateCashInUI(int newCash) => UpdateUI(newCash, 2);

    private void UpdateUI(int newValue, int BarIndex)
    {
        Transform ValueBar;
        GameObject prefabValue;
        switch (BarIndex)
        {
            case 0:
                ValueBar = HPBar;
                prefabValue = prefabHP;
                break;
            case 1:
                ValueBar = DmgBar;
                prefabValue = prefabDmg;
                break;
            case 2:
                ValueBar = CashBar;
                prefabValue = prefabCash;
                break;
            default:
                return;
        }

        int actualValue = ValueBar.childCount;
        int dif = newValue - actualValue;

        if (dif > 0)
        {
            for (int i = 0; i < dif; i++)
                Instantiate(prefabValue, ValueBar);

        }
        else if (dif < 0)
        {
            for (int i = actualValue; i > newValue; i--)
                Destroy(ValueBar.transform.GetChild(i - 1).gameObject);
        }
    }
}
