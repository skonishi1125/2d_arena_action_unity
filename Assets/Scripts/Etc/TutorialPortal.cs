using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPortal : MonoBehaviour
{
    [SerializeField] private string transferToScene;

    // BattleEasyへ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(transferToScene);
    }

}
