using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using UnityEngine.InputSystem.LowLevel;

public class PlayerAnimationsUtil : MonoBehaviour
{
    private PlayerController playerController;
    
    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void ContinueToState(PlayerStates newState)
    {
        playerController.SwitchToState(newState);
    }
}
