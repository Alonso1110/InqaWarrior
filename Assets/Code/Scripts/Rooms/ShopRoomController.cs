using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoomController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("CallTheGates", 1);
    }

    private void CallTheGates()
    {
        GetComponent<GeneralRoomController>().OpenTheGates();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
