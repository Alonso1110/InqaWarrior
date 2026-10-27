using UnityEngine;
using UnityEngine.SceneManagement;

public class GateController : MonoBehaviour
{
    private string targetSceneName;
    private BoxCollider2D myCollider;
    private ParticleSystem myParticles;
    private bool resetRoomsWithoutFight = false;

    [SerializeField] private Sprite[] blahParticles;
    [SerializeField] private Sprite[] musicParticles;
    [SerializeField] private Sprite[] emptyParticles;
    [SerializeField] private float emissionRateOverTime;

    private void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        myParticles = GetComponentInChildren<ParticleSystem>();
        myCollider.isTrigger = false;
        var emision = myParticles.emission;
        emision.rateOverTime = 0;
    }
    public void SetGate(string _targetSceneName, GeneralRoomController.SceneType sceneType)
    {
        myCollider.isTrigger = true;
        targetSceneName = _targetSceneName;
        switch (sceneType)
        {
            case GeneralRoomController.SceneType.Combat:
                resetRoomsWithoutFight = true;
                for (int i = 0; i < blahParticles.Length; i++)
                {
                    myParticles.textureSheetAnimation.AddSprite(blahParticles[i]);
                }
                break;
            case GeneralRoomController.SceneType.Shop:
                for (int i = 0; i < musicParticles.Length; i++)
                {
                    myParticles.textureSheetAnimation.AddSprite(musicParticles[i]);
                }
                break;
            case GeneralRoomController.SceneType.Treasure:
                for (int i = 0; i < emptyParticles.Length; i++)
                {
                    myParticles.textureSheetAnimation.AddSprite(emptyParticles[i]);
                }
                break;
            default: break;
        }
        var emision = myParticles.emission;
        emision.rateOverTime = emissionRateOverTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (resetRoomsWithoutFight) PlayerStats.Instance.ResetRoomsWithoutFight();
            else PlayerStats.Instance.OneMoreRoomWithoutFight();

            Debug.Log("GO in the Gate");
            SceneManager.LoadScene(targetSceneName);
        }
    }


}
