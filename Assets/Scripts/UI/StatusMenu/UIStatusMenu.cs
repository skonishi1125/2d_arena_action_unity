using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIStatusMenu : MonoBehaviour
{

    private Player player;
    private PlayerHealth playerHealth;
    private PlayerLevel playerLevel;
    private PlayerSkillController playerSkill;
    private PlayerTimedModifiers playerTimedModifiers;
    private EntityStatus entityStatus;
    private PlayerSkillController skillController;

    [SerializeField] private GameObject rootPanel;
    [SerializeField] private StatusPanel statusPanel;
    [SerializeField] private SkillPanel skillPanel;
    [SerializeField] private DescriptionPanel descriptionPanel;

    [Header("TipBar Setting")]
    [SerializeField] private GameObject tipRoot;
    [SerializeField] private Selectable tipCloseSelectable;
    [SerializeField] private float tipsShowTime = 15f;
    [SerializeField] private Selectable[] leftEdge;
    [SerializeField] private Selectable[] rightEdge;
    private bool _lastTipActive;

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

        playerHealth = player.Health;
        if (!LogHelper.AssertNotNull(playerHealth, nameof(playerHealth), this))
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

        playerTimedModifiers = player.GetComponent<PlayerTimedModifiers >();
        if (!LogHelper.AssertNotNull(playerTimedModifiers, nameof(playerTimedModifiers), this))
            return;

        skillController = player.GetComponentInChildren<PlayerSkillController>();
        if (!LogHelper.AssertNotNull(skillController, nameof(skillController), this))
            return;

        // キーボード操作関連の準備
        player.input.UI.Disable();
        player.input.Player.Enable();

        // ステータスパネルなどの準備
        statusPanel.Init(entityStatus, playerHealth, playerLevel, playerSkill, playerTimedModifiers);
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

        // チップ表示状態の変化を検知し、差し替える

        bool tipActive = IsTipActive();
        if (tipActive != _lastTipActive)
        {
            _lastTipActive = tipActive;
            RefreshDynamicNavigation(tipActive);
            FixSelectionIfTipHidden(tipActive);
        }
    }

    private bool IsTipActive()
    {
        return tipRoot != null && tipRoot.activeInHierarchy;
    }

    private void RefreshDynamicNavigation(bool hasTip)
    {
        int n = Mathf.Min(
            leftEdge != null ? leftEdge.Length : 0,
            rightEdge != null ? rightEdge.Length : 0
        );

        for (int i = 0; i < n; i++)
        {
            var left = leftEdge[i];
            var right = rightEdge[i];
            if (left == null || right == null) continue;

            // 左端：←
            {
                var nav = left.navigation;
                nav.mode = Navigation.Mode.Explicit;
                nav.selectOnLeft = hasTip ? tipCloseSelectable : right;
                left.navigation = nav;
            }

            // 右端：→
            {
                var nav = right.navigation;
                nav.mode = Navigation.Mode.Explicit;
                nav.selectOnRight = hasTip ? tipCloseSelectable : left;
                right.navigation = nav;
            }
        }
    }

    private void FixSelectionIfTipHidden(bool hasTip)
    {
        if (hasTip) return;

        var es = EventSystem.current;
        if (es == null) return;

        var cur = es.currentSelectedGameObject;
        if (cur == null || cur.activeInHierarchy) return;

        // 逃がし先（好みで firstSelectedInMenu でもOK）
        if (firstSelectedInMenu != null)
            es.SetSelectedGameObject(firstSelectedInMenu);
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

        _lastTipActive = IsTipActive();
        RefreshDynamicNavigation(_lastTipActive);

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
