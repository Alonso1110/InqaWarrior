using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFeelManager : MonoBehaviour
{
    public static GameFeelManager Instance { get; private set; }

    private bool isFrozen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Freeze Frame Logic
    public void FreezeFrame(float duration)
    {
        if (!isFrozen)
        {
            StartCoroutine(DoFreeze(duration));
        }
    }

    private IEnumerator DoFreeze(float duration)
    {
        isFrozen = true;
        Time.timeScale = 0f; // Pausa el tiempo

        yield return new WaitForSecondsRealtime(duration); // Espera en tiempo real

        Time.timeScale = 1f; // Reanuda el tiempo
        isFrozen = false;
    }

    #endregion

    #region Screen Shake Logic
    public void ScreenShake()
    {
        // Aquí añadiremos la lógica para agitar la cámara más adelante
        Debug.Log("¡La cámara está temblando!");
    }

    #endregion
}
