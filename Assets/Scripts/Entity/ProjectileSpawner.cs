using UnityEngine;

// 遠距離攻撃の弾などを管理するクラス
// 敵味方に付与して、使いたい弾をPrefabでセットすればよい
public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform spawnPoint; // 射撃口

    [SerializeField] private float speed = 5f;

    public void Spawn()
    {
        Projectile proj = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        proj.Fire(spawnPoint.right, speed, GetComponentInParent<Entity>());
    }

}
