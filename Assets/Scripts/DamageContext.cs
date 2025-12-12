using UnityEngine;

// Combat, Projectileで発生するダメージをまとめた構造体
// この構造体をTakeDamage()等に渡すと、KB処理などを済ませてくれる
public struct DamageContext
{
    public Transform attacker; // ノックバック方向用
    public float damage;

    public bool hasCustomKnockback;
    public Vector2 knockbackPower;
    public float knockbackDuration;

    public float criticalRate; // 必要なら
    public object source;// 任意：ProjectileやCombat等を入れてデバッグ用
}
