using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    private Player player;
    private EntityStatus entityStatus;
    private EntityHealth entityHealth;
    private PlayerLevel playerLevel;
    [SerializeField] private PlayerLevelTable levelTable;

    [Header("Player Health Bar")]
    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("EXP Bar")]
    [SerializeField] private RectTransform expRect;
    [SerializeField] private Slider expSlider;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            // Health
            entityStatus = player.GetComponent<EntityStatus>();
            entityHealth = player.GetComponent<EntityHealth>();
            entityHealth.OnHealthUpdate += UpdateHealthBar;

            // EXP
            playerLevel = player.GetComponent<PlayerLevel>();
        }

    }

    private void UpdateHealthBar()
    {
        var currentHp = entityHealth.GetCurrentHp();
        var maxHp = entityStatus.GetMaxHp();

        healthText.text = currentHp + "/" + maxHp;
        healthSlider.value = currentHp / maxHp;
    }

    public void UpdateExpBar()
    {
        int currentExp = playerLevel.CurrentExp;
        int currentLevel = playerLevel.Level;
        int requiredExp = levelTable.GetLevelOfInfo(currentLevel).requiredExp;

        float ratio = (float)currentExp / requiredExp;

        Debug.Log("UpdateExpBar currentExp: " + currentExp + " requiredExp: " + requiredExp + " 割合: " + currentExp / requiredExp);


        expSlider.value = ratio;
    }

}
