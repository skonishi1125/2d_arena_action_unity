using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    private Player player;
    private EntityStatus entityStatus;
    private EntityHealth entityHealth;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            entityStatus = player.GetComponent<EntityStatus>();
            entityHealth = player.GetComponent<EntityHealth>();
            entityHealth.OnHealthUpdate += UpdateHealthBar;
        }

    }

    private void UpdateHealthBar()
    {
        var currentHp = entityHealth.GetCurrentHp();
        var maxHp = entityStatus.GetMaxHp();

        healthText.text = currentHp + "/" + maxHp;
        healthSlider.value = currentHp / maxHp;
    }

}
