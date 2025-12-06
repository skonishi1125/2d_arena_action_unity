using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private PlayerHealth playerHealth;

    public static CameraManager Instance;
    private CinemachineImpulseSource impulse;
    [SerializeField] private float intensity = 2f;

    private void Awake()
    {
        Instance = this;
        impulse = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            playerHealth = GameManager.Instance.Player.Health;
            if (playerHealth != null)
                playerHealth.OnDied += HandleDeathShake;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            playerHealth = GameManager.Instance.Player.Health;
            if (playerHealth != null)
                playerHealth.OnDied -= HandleDeathShake;
        }
    }

    // 死亡時、画面を揺らす処理
    private void DeathShake()
    {
        // 上下左右にぐらぐら揺らす
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 velocity = new Vector3(dir2D.x, dir2D.y, 0f) * intensity;
        impulse.GenerateImpulse(velocity);
    }

    private void HandleDeathShake()
    {
        DeathShake();
    }

}
