using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPortal : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private string transferToScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Playerが触れた時だけ
        if (((1 << collision.gameObject.layer) & whatIsPlayer) != 0)
            SceneManager.LoadScene(transferToScene);
    }

}
