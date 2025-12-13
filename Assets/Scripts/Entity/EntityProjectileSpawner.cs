using UnityEngine;

// 遠距離攻撃の弾などを管理するクラス
// 敵味方に付与して、使いたい弾をPrefabでセットすればよい
public class EntityProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint; // 射撃口
    [SerializeField] private float speed = 5f;

    public void Spawn(Entity entity, ProjectileSpawnRequest req)
    {
        EntityProjectile proj = Instantiate(req.prefab, spawnPoint.position, spawnPoint.rotation);

        // 弾インスタンスにダメージ, KB値の設定
        proj.SetDamageMultiplier(req.damage.damageMultiplier);
        if (req.damage.hasCustomKnockback)
            proj.SetKnockback(req.damage.knockbackPower, req.damage.knockbackDuration);
        else
            proj.ResetKnockback();

        proj.Fire(entity.facingDir, speed, entity);
    }

}
