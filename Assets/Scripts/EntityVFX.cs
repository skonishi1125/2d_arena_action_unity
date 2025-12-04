using System.Collections;
using UnityEngine;

public class EntityVFX : MonoBehaviour
{

    private SpriteRenderer sr;

    [Header("Damage VFX")]
    [SerializeField] private Material onDamageMaterial; // 被弾時に白くなるmaterial
    [SerializeField] private float onDamageVfxDuration = .2f; // 演出時間
    private Material originalMaterial;
    private Coroutine onDamageVfxCo;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    // 被弾時走らせて、演出時間の間animatonのspriteを白くして、元に戻す。
    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCo != null)
            StopCoroutine(onDamageVfxCo);

        Debug.Log("co!");

        onDamageVfxCo = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }




}
