using System.Collections;
using TMPro;
using UnityEngine;

public class EntityVFX : MonoBehaviour
{

    private SpriteRenderer sr;

    [Header("Damage VFX")]
    [SerializeField] private Material onDamageMaterial; // 被弾時に白くなるmaterial
    [SerializeField] private float onDamageVfxDuration = .2f; // 演出時間
    private Material originalMaterial;
    private Coroutine onDamageVfxCo;

    // ダメージ数値
    [Header("Attack Damage Number Vfx")]
    [SerializeField] private GameObject damageNumberVfx;
    [SerializeField] private Color damageNumberVfxColor = Color.white;

    [Header("Attack CritDamage Number Vfx")]
    [SerializeField] private GameObject critDamageNumberVfx;
    [SerializeField] private Color critDamageNumberVfxColor = Color.white;

    // 斬撃(などのダメージ)
    [Header("Attack Hit VFX")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private Color hitVfxColor = Color.white;
    // 斬撃(などのダメージ) クリティカル
    [Header("Attack CritHit VFX")]
    [SerializeField] private GameObject critHitVfx;
    [SerializeField] private Color critHitColor = Color.white;

    // 射撃
    [Header("Projectile Hit VFX")]
    [SerializeField] private GameObject projectileHitVfx;
    [SerializeField] private Color projectileHitVfxColor = Color.white;

    // 射撃クリティカル
    [Header("Projectile CritHit VFX")]
    [SerializeField] private GameObject projectileCritHitVfx;
    [SerializeField] private Color projectileCritHitColor = Color.white;

    // Evasionでよけた時
    [Header("Miss Hit VFX")]
    [SerializeField] private GameObject missHitVfx;

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    public void CreateOnDamageNumberVfx(Transform target, float damage)
    {
        var go = Instantiate(damageNumberVfx, target.position, Quaternion.identity);
        var vfx = go.GetComponent<DamageNumberVfx>();
        vfx.Init(damage, damageNumberVfxColor);
    }

    public void CreateOnCritDamageNumberVfx(Transform target, float damage)
    {
        var go = Instantiate(critDamageNumberVfx, target.position, Quaternion.identity);
        var vfx = go.GetComponent<DamageNumberVfx>();
        vfx.Init(damage, critDamageNumberVfxColor);
    }

    // Player, Enemy 通常斬撃などのVFX
    public void CreateOnHitVfx(Transform target)
    {
        GameObject vfx = Instantiate(hitVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;
    }

    // クリティカル時
    public void CreateOnCritHitVfx(Transform target)
    {
        GameObject vfx = Instantiate(critHitVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = critHitColor;
    }

    // Miss!というようなvfx

    public void CreateOnMissHitVfx(Transform target)
    {
        GameObject vfx = Instantiate(missHitVfx, target.position, Quaternion.identity);
    }

    // Player, 敵 被弾時に白くなる演出
    // 被弾時走らせて、演出時間の間animatonのspriteを白くして、元に戻す。
    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCo != null)
            StopCoroutine(onDamageVfxCo);

        onDamageVfxCo = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }

    // 通常弾ヒット
    public void CreateOnProjectileHitVfx(Transform target)
    {
        GameObject vfx = Instantiate(projectileHitVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = projectileHitVfxColor;
    }

    public void CreateOnProjectileCritHitVfx(Transform target)
    {
        GameObject vfx = Instantiate(projectileCritHitVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = projectileCritHitColor;
    }




}
