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

    [Header("Skill Slots")]
    [SerializeField] private SkillSlotWidget zSlot;
    [SerializeField] private SkillSlotWidget dSlot;
    [SerializeField] private SkillSlotWidget vSlot;

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

            // Level 経験値返還時のイベント
            playerLevel.OnExpChanged += HandleExpChanged;
        }

        UpdatePlayerHealthBar();

        InitSkillSlots();
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthUpdate -= UpdatePlayerHealthBar;

        }

        if (playerLevel != null)
        {
            playerLevel.OnLevelUp -= HandlePlayerLevelUp;
            playerLevel.OnExpChanged -= HandleExpChanged;
        }

    }



    private void UpdatePlayerHealthBar()
    {
        float currentHp = playerHealth.GetCurrentHp();
        float maxHp = entityStatus.GetMaxHp();

        //Debug.Log("currentHp: " + currentHp + " maxHp: " + maxHp);

        healthText.text = currentHp + "/" + maxHp;
        healthSlider.value = currentHp / maxHp;
    }

    // レベルが上がった時に体力バーの更新を行う
    // playerLevel.OnLevelUp には引数intの値が必要なので、
    // 引数を無視する記述を書いて登録する
    private void HandlePlayerLevelUp(int _)
    {
        UpdatePlayerHealthBar();
    }



    // EXPバー更新処理
    private void HandleExpChanged(int currentExp, int requiredExp)
    {
        if (requiredExp <= 0)
        {
            expSlider.value = 1f;
            return;
        }

        //Debug.Log("HandleExpChanged currentExp: " + currentExp + " requiredExp: " + requiredExp + " 割合: " + currentExp / requiredExp);

        expSlider.value = (float)currentExp / requiredExp;
    }

    // スキルスロット呼び出し
    private void InitSkillSlots()
    {
        var skill = player.Skill;
        zSlot?.Setup(skill);
        dSlot?.Setup(skill);
        vSlot?.Setup(skill);
    }

}
