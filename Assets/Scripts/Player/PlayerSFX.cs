using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    private PlayerHealth health;
    private PlayerLevel level;

    [Header("Player SE Clips")]
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip magicSfx;
    [SerializeField] private AudioClip itemSfx;
    [SerializeField] private AudioClip hittedSfx; // 被弾
    [SerializeField] private AudioClip diedSfx; // ドガンみたいな音(カービィのやられたときの音みたいなやつ）
    [SerializeField] private AudioClip levelUpSfx;
    [SerializeField] private AudioClip dashSfx;
    [SerializeField] private AudioClip teleportSfx;
    [SerializeField] private AudioClip skillAttackSfx;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        if (!LogHelper.AssertNotNull(health, nameof(health), this))
            return;

        level = GetComponent<PlayerLevel>();
        if (!LogHelper.AssertNotNull(level, nameof(level), this))
            return;
    }

    private void OnEnable()
    {
        if (health == null)
            return;
        health.OnTakeDamage += PlayHitted;
        health.OnDied += PlayDied;

        if (level == null)
            return;
        level.OnLevelUp += HandlePlayLevelUp;

        ItemPickup.OnTakeItem += PlayItem;
    }

    private void OnDisable()
    {
        if (health == null)
            return;
        health.OnTakeDamage -= PlayHitted;
        health.OnDied -= PlayDied;


        ItemPickup.OnTakeItem += PlayItem;
    }


    public void PlayAttack()
    {
        AudioManager.Instance?.PlaySfx(attackSfx);
    }

    public void PlayMagic()
    {
        AudioManager.Instance?.PlaySfx(magicSfx);
    }

    public void PlayItem()
    {
        AudioManager.Instance?.PlaySfx(itemSfx);
    }

    public void PlayHitted()
    {
        AudioManager.Instance?.PlaySfx(hittedSfx);
    }

    public void PlayDied()
    {
        // 一旦ここでBGMも止めているが、責務外かも。
        AudioManager.Instance?.StopBgm();
        AudioManager.Instance?.PlaySfx(diedSfx);

    }

    public void PlayLevelUp()
    {
        AudioManager.Instance?.PlaySfx(levelUpSfx);
    }

    // OnLevelUpがintを渡すので、intを受け取るハンドラを用意するイメージ
    public void HandlePlayLevelUp(int _)
    {
        PlayLevelUp();
    }

    public void PlayDash()
    {
        AudioManager.Instance?.PlaySfx(dashSfx);
    }

    public void PlayTeleport()
    {
        AudioManager.Instance?.PlaySfx(teleportSfx);
    }

    public void PlaySkillAttack()
    {
        AudioManager.Instance?.PlaySfx(skillAttackSfx);
    }




}
