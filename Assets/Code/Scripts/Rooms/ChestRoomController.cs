using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRoomController : MonoBehaviour
{
    [SerializeField] private List<SpawnableItem> spawnables = new List<SpawnableItem>();
    [SerializeField] private List<Chest> chests = new List<Chest>();

    private void Start()
    {
        foreach (Chest chest in chests)
        {
            chest.SetChest(spawnables);
        }
    }
}
