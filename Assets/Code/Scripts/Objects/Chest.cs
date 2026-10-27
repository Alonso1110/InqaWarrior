using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnableItem
{
    public GameObject Prefab;

    public float chance;
}

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private List<SpawnableItem> spawnables = new List<SpawnableItem>();
    private float totalRawChances = 0;

    private bool chestClosed = true;

    private void Start()
    {
        SetChest(null);
    }

    public void SetChest(List<SpawnableItem> _spawnables)
    {
        if (_spawnables != null) spawnables = _spawnables;
        chestClosed = true;
        foreach (SpawnableItem item in spawnables)
        {
            totalRawChances += item.chance;
        }
        totalRawChances = totalRawChances * 4 / 3; // Le aumento un tercio para que 25% quede siempre vacio
        //OpenChest();
    }

    private void OpenChest()
    {
        if (!chestClosed) return;
        float acumulatedPercentage = 0;
        float chance = Random.value;
        Debug.Log(chance);

        for (int i = 0; i < spawnables.Count; i++)
        {
            float elementChance = spawnables[i].chance / totalRawChances;
            acumulatedPercentage += elementChance;
            if (chance < acumulatedPercentage)
            {
                //element won
                GameObject toSpawn = spawnables[i].Prefab;
                Instantiate(toSpawn,transform.position,Quaternion.identity);
                chestClosed = false;
                return;
            }

        }

        
    }

    public void Interact()
    {
        OpenChest();
        GetComponent<SpriteRenderer>().color = Color.black;
    }
}
