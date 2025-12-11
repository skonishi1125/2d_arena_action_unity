using UnityEngine;

// 遠距離攻撃の弾などを管理するクラス
// 敵味方に付与して、使いたい弾をPrefabでセットすればよい
public class EntityProjectileSpawner : MonoBehaviour
{
    [SerializeField] private EntityProjectile projectilePrefab;
    [SerializeField] private Transform spawnPoint; // 射撃口

    [SerializeField] private float speed = 5f;

    public void Spawn(Entity entity)
    {
        EntityProjectile proj = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        proj.Fire(entity.facingDir, speed, entity);
    }

}
