using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private ObjectiveHealth objectiveHealth;

    public static CameraManager Instance;
    private CinemachineImpulseSource impulse;
    [SerializeField] private float intensity = 2f;

    private void Awake()
    {
        Instance = this;
        impulse = GetComponent<CinemachineImpulseSource>();
    }

    // OnEnable,OnDisableで購読処理を行わない。
    // playerHealth = GameManager.Instance.Player.Health;
    // playerHealth.OnDied += HandleDeathShake; とできるが、
    // GameObject側はこちらのOnEnable実行時点ではPlayerを所持していない。
    // そのためこのメソッドを用意し、GameManagerの準備が出来次第割り当てる。
    public void BindPlayerHealth(PlayerHealth newPlayerHealth)
    {
        // 古い購読を解除
        if (playerHealth != null)
            playerHealth.OnDied -= HandleDeathShake;

        // 新しく設定
        playerHealth = newPlayerHealth;

        if (playerHealth != null)
            playerHealth.OnDied += HandleDeathShake;
    }

    public void BindObjectiveHealth(ObjectiveHealth newObjectiveHealth)
    {
        // 古い購読を解除
        if (objectiveHealth != null)
            objectiveHealth.OnDestroyed -= _ => HandleDeathShake();

        // 新しく設定
        objectiveHealth = newObjectiveHealth;

        if (objectiveHealth != null)
            objectiveHealth.OnDestroyed += _ => HandleDeathShake();
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
