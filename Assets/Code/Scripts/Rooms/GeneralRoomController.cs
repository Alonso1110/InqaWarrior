using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralRoomController : MonoBehaviour
{
    public enum SceneType
    {
        Combat,
        Shop,
        Treasure
    }

    [SerializeField] private string[] CombatScenesNames;
    [SerializeField] private string ShopSceneName;
    [SerializeField] private string TreasureSceneName;

    [SerializeField] private GateController LeftGate;
    [SerializeField] private GateController RightGate;

    private int roomsWithoutFightLimit = 5;
   
    public void OpenTheGates()
    {
        if (PlayerStats.Instance.roomsWithoutFight < roomsWithoutFightLimit)
        {
            Debug.Log("Normal Room Generation");
            int randomRoomTypeLeft = Random.Range(0, 3);
            int randomRoomTypeRight = Random.Range(0, 3);
            string RoomSelected = CombatScenesNames[0];
            SceneType roomtype = SceneType.Combat;
            switch (randomRoomTypeLeft)
            {
                case 0:
                    RoomSelected = CombatScenesNames[Random.Range(0, CombatScenesNames.Length)];
                    roomtype = SceneType.Combat;
                    break;
                case 1:
                    RoomSelected = ShopSceneName;
                    roomtype = SceneType.Shop;
                    break;
                case 2:
                    RoomSelected = TreasureSceneName;
                    roomtype = SceneType.Treasure;
                    break;
                default: break;
            }
            LeftGate.SetGate(RoomSelected, roomtype);

            switch (randomRoomTypeRight)
            {
                case 0:
                    RoomSelected = CombatScenesNames[Random.Range(0, CombatScenesNames.Length)];
                    roomtype = SceneType.Combat;
                    break;
                case 1:
                    RoomSelected = ShopSceneName;
                    roomtype = SceneType.Shop;
                    break;
                case 2:
                    RoomSelected = TreasureSceneName;
                    roomtype = SceneType.Treasure;
                    break;
                default: break;
            }
            RightGate.SetGate(RoomSelected, roomtype);
        }
        else
        {
            Debug.Log("Fixed Room Generation");

        }
    }
}
