using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    private Player player;
    private EntityStatus entityStatus;
    private PlayerHealth playerHealth;
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
            playerHealth = player.GetComponent<PlayerHealth>();
            // EXP
            playerLevel = player.GetComponent<PlayerLevel>();


            // Health Actionイベントに、体力バー更新の購読
            playerHealth.OnHealthUpdate += UpdatePlayerHealthBar;

            // Level Actionイベントに、体力バー更新の購読
            playerLevel.OnLevelUp += HandlePlayerLevelUp;
        }
    }

    // レベルが上がった時に体力バーの更新を行う
    // playerLevel.OnLevelUp には引数intの値が必要なので、
    // 引数を無視する記述を書いて登録する
    private void HandlePlayerLevelUp(int _)
    {
        UpdatePlayerHealthBar();
    }

    private void UpdatePlayerHealthBar()
    {
        float currentHp = playerHealth.GetCurrentHp();
        float maxHp = entityStatus.GetMaxHp();

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
