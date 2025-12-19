using UnityEngine;
using UnityEngine.EventSystems;

public class UIStatusMenu : MonoBehaviour
{

    private Player player;
    private PlayerLevel playerLevel;
    private PlayerSkillController playerSkill;
    private EntityStatus entityStatus;
    private PlayerSkillController skillController;

    [SerializeField] private GameObject rootPanel;
    [SerializeField] private StatusPanel statusPanel;
    [SerializeField] private SkillPanel skillPanel;
    [SerializeField] private DescriptionPanel descriptionPanel;

    [Header("Tip Showtime")]
    [SerializeField] private float tipsShowTime = 15f;

    // キーボード操作関連
    [SerializeField] private GameObject firstSelectedInMenu;
    private bool isOpen;

    private void Awake()
    {
        isOpen = false;
    }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(player, nameof(player), this))
            return;

        entityStatus = player.GetComponent<EntityStatus>();
        if (!LogHelper.AssertNotNull(entityStatus, nameof(entityStatus), this))
            return;

        playerSkill = player.GetComponent<PlayerSkillController>();
        if (!LogHelper.AssertNotNull(playerSkill, nameof(playerSkill), this))
            return;

        playerLevel = player.GetComponent<PlayerLevel>();
        if (!LogHelper.AssertNotNull(playerLevel, nameof(playerLevel), this))
            return;

        skillController = player.GetComponentInChildren<PlayerSkillController>();
        if (!LogHelper.AssertNotNull(skillController, nameof(skillController), this))
            return;

        // キーボード操作関連の準備
        player.input.UI.Disable();
        player.input.Player.Enable();

        // ステータスパネルなどの準備
        statusPanel.Init(entityStatus, playerLevel, playerSkill);
        skillPanel.Init(playerLevel);

        rootPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isOpen)
        {
            // 開く：Player側の StatusMenu（Esc 等）
            if (player.input.Player.StatusMenu.WasPressedThisFrame())
                Open();
        }
        else
        {
            // 閉じる：UI側の Cancel（Esc/X）
            if (player.input.UI.Cancel.WasPressedThisFrame())
                Close();
        }

        //if (player.input.Player.StatusMenu.WasPressedThisFrame())
        //{
        //    bool active = !rootPanel.activeSelf;
        //    rootPanel.SetActive(active);
        //    Time.timeScale = active ? 0f : 1f;

        //    if (active)
        //        descriptionPanel.ShowTip(tipsShowTime);
        //}
    }


    // 開閉時のTimeScale, Object表示可否などの調整、
    private void SetPanelStatus()
    {
        bool active = !rootPanel.activeSelf;
        rootPanel.SetActive(active);
        Time.timeScale = active ? 0f : 1f;

        if (active)
            descriptionPanel.ShowTip(tipsShowTime);
    }

    // メニューを開く際に、InputSetやEventSystem等を管理する処理
    private void Open()
    {
        isOpen = true;

        rootPanel.SetActive(true);
        Time.timeScale = 0f;
        descriptionPanel.ShowTip(tipsShowTime);

        // 戦闘入力を止めて、UI入力へ
        player.input.Player.Disable();
        player.input.UI.Enable();

        // 選択を作る（これが無いと矢印が効かないことが多い）
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedInMenu);
        }
    }

    private void Close()
    {
        isOpen = false;

        // 選択解除
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        rootPanel.SetActive(false);
        Time.timeScale = 1f;

        // UI入力を止めて、戦闘入力へ戻す
        player.input.UI.Disable();
        player.input.Player.Enable();
    }

}
