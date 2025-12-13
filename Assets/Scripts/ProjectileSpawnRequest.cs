using System;

[Serializable]
public struct ProjectileSpawnRequest
{
    public EntityProjectile prefab;          // どの弾を出すか
    public ProjectileDamageContext damage;   // 威力/KBなど（既存）
    public float? speedOverride;// スキル別, 速度設定
    public bool destroyOnGround;
    public bool destroyOnHit;
    //public int pierceCount; 
}

