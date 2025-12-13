using UnityEngine;

// 遠距離攻撃の弾などを管理するクラス
// 敵味方に付与して、使いたい弾をPrefabでセットすればよい
public class EntityProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint; // 射撃口
    [SerializeField] private float defaultSpeed = 5f; // 指定がないときのspeed

    public void Spawn(Entity entity, ProjectileSpawnRequest req)
    {
        EntityProjectile proj = Instantiate(req.prefab, spawnPoint.position, spawnPoint.rotation);

        // 弾に各種パラメータの割当
        proj.SetPierce(req.pierceGround, req.pierceTargets);
        proj.SetDamageMultiplier(req.damage.damageMultiplier);
        if (req.damage.hasCustomKnockback)
            proj.SetKnockback(req.damage.knockbackPower, req.damage.knockbackDuration);
        else
            proj.ResetKnockback();

        proj.Fire(entity.facingDir, req.speedOverride ?? defaultSpeed, entity);
    }

}
