using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    private Objective objective;

    private Player player;
    private EntityStatus entityStatus;
    private PlayerHealth playerHealth;
    private PlayerLevel playerLevel;
    [SerializeField] private PlayerLevelTable levelTable;

    [Header("Objective Health Bar")]
    [SerializeField] private RectTransform objectiveHealthRect;
    [SerializeField] private Slider objectiveHealthSlider;
    [SerializeField] private TextMeshProUGUI objectiveHealthText;

    [Header("Player Health Bar")]
    [SerializeField] private RectTransform playerHealthRect;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TextMeshProUGUI playerHealthText;

    [Header("EXP Bar")]
    [SerializeField] private RectTransform expRect;
    [SerializeField] private Slider expSlider;

    [Header("Skill Slots")]
    [SerializeField] private SkillSlotWidget zSlot;
    [SerializeField] private SkillSlotWidget dSlot;
    [SerializeField] private SkillSlotWidget vSlot;

    private void Start()
    {
        // ========== objective ========== 
        // FindAnyObjectByTypeで取ってみる
        // (高速らしいが, 呼び出しごとに同じinstanceとは限らない)
        objective = FindAnyObjectByType<Objective>();
        if (!LogHelper.AssertNotNull(objective, nameof(objective), this))
            return;

        objective.Health.OnHealthUpdate += UpdateObjectiveHealthBar;

        //  ========== player ========== 
        player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(player, nameof(player), this))
            return;

        entityStatus = player.GetComponent<EntityStatus>();
        if (!LogHelper.AssertNotNull(entityStatus, nameof(entityStatus), this))
            return;

        playerHealth = player.GetComponent<PlayerHealth>();
        if (!LogHelper.AssertNotNull(playerHealth, nameof(playerHealth), this))
            return;

        playerLevel = player.GetComponent<PlayerLevel>();
        if (!LogHelper.AssertNotNull(playerLevel, nameof(playerLevel), this))
            return;

        // ========= 購読 ========== 
        // Health Actionイベントに、体力バー更新の購読
        playerHealth.OnHealthUpdate += UpdatePlayerHealthBar;

        // Level Actionイベントに、体力バー更新の購読
        playerLevel.OnLevelUp += HandlePlayerLevelUp;

        // Level 経験値返還時のイベント
        playerLevel.OnExpChanged += HandleExpChanged;

        // ========= 初期化 ========== 
        // 画面のSAMPLEなどのテキストを、Awake時点の設定にする
        UpdateObjectiveHealthBar();
        UpdatePlayerHealthBar();
        InitSkillSlots();
    }

    private void OnDestroy()
    {
        if (objective != null)
            objective.Health.OnHealthUpdate -= UpdateObjectiveHealthBar;

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

    private void UpdateObjectiveHealthBar()
    {
        float currentHp = objective.Health.GetCurrentHp();
        float maxHp = objective.Health.GetMaxHp();

        objectiveHealthText.text = currentHp + "/" + maxHp;
        objectiveHealthSlider.value = currentHp / maxHp;

    }


    private void UpdatePlayerHealthBar()
    {
        float currentHp = playerHealth.GetCurrentHp();
        float maxHp = entityStatus.GetMaxHp();

        playerHealthText.text = currentHp + "/" + maxHp;
        playerHealthSlider.value = currentHp / maxHp;
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
