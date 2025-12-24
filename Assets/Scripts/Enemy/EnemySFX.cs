using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    private EnemyHealth health;

    [Header("Player SE Clips")]
    //[SerializeField] private AudioClip attackSfx;
    //[SerializeField] private AudioClip magicSfx;
    //[SerializeField] private AudioClip itemSfx;
    [SerializeField] private AudioClip hittedSfx; // 被弾

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        if (!LogHelper.AssertNotNull(health, nameof(health), this))
            return;
    }

    private void OnEnable()
    {
        if (health == null)
            return;
        health.OnTakeDamaged += PlayHitted;

        EnemyHealth.OnAnyEnemyDied += HandlerPlayHitted;

    }

    private void OnDisable()
    {
        if (health == null)
            return;
        health.OnTakeDamaged -= PlayHitted;

        EnemyHealth.OnAnyEnemyDied -= HandlerPlayHitted;

    }

    public void PlayHitted()
    {
        AudioManager.Instance?.PlaySfx(hittedSfx);
    }

    private void HandlerPlayHitted(EnemyHealth _)
    {
        PlayHitted();
    }
}
