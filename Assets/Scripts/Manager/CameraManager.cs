using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private CinemachineImpulseSource impulse;
    [SerializeField] private float intensity = 2f;

    private void Awake()
    {
        Instance = this;
        impulse = GetComponent<CinemachineImpulseSource>();
    }

    // 死亡時、画面を揺らす処理
    public void DeathShake()
    {
        // 上下左右にぐらぐら揺らす
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 velocity = new Vector3(dir2D.x, dir2D.y, 0f) * intensity;
        impulse.GenerateImpulse(velocity);
    }

}
