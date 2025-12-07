using UnityEngine;

public class UIEnemyMiniHealthBar : MonoBehaviour
{
    private Enemy enemy;
    private EnemyHealth enemyHealth;

    [SerializeField] private GameObject slider;
    private bool isDisplayedHealthBar;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        if (!LogHelper.AssertNotNull(enemy, nameof(enemy), this))
            return;

        enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (!LogHelper.AssertNotNull(enemyHealth, nameof(enemyHealth), this))
            return;

        if (!LogHelper.AssertNotNull(slider, nameof(slider), this))
            return;

        isDisplayedHealthBar = false;
        slider.SetActive(false);
    }

    private void OnEnable()
    {
        enemy.OnFlipped += PreventUIFlip;
        enemyHealth.OnTakeDamaged += DisplayHealthBar;
    }

    private void OnDisable()
    {
        enemy.OnFlipped -= PreventUIFlip;
        enemyHealth.OnTakeDamaged -= DisplayHealthBar;

    }

    private void PreventUIFlip()
    {
        // UIバーが反転して、体力の減りも反転してしまうのを防ぐ
        transform.rotation = Quaternion.identity;
    }

    private void DisplayHealthBar()
    {
        if (isDisplayedHealthBar)
            return;

        if (!LogHelper.AssertNotNull(slider, nameof(slider), this))
            return;

        slider.SetActive(true);
        isDisplayedHealthBar = true;
    }

}
