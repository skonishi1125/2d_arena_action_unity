using UnityEngine;

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

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        entityStatus = player.GetComponent<EntityStatus>();
        playerSkill = player.GetComponent<PlayerSkillController>();
        playerLevel = player.GetComponent<PlayerLevel>();
        skillController = player.GetComponentInChildren<PlayerSkillController>();
    }

    private void Start()
    {
        statusPanel.Init(entityStatus, playerLevel, playerSkill);
        skillPanel.Init(playerLevel);

        rootPanel.SetActive(false);
    }

    private void Update()
    {
        if (player.input.Player.StatusMenu.WasPressedThisFrame())
        {
            bool active = !rootPanel.activeSelf;
            rootPanel.SetActive(active);
            Time.timeScale = active ? 0f : 1f;

            if (active)
                descriptionPanel.ShowTip("TIP: テストメッセージ", 2f);
        }
    }

}
