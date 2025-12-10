using UnityEngine;
using UnityEngine.InputSystem;

public class UIStatusMenu : MonoBehaviour
{
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private StatusPanel statusPanel;
    [SerializeField] private SkillPanel skillPanel;

    private Player player;
    private PlayerLevel playerLevel;
    private EntityStatus entityStatus;
    private PlayerSkillController skillController;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        entityStatus = player.GetComponent<EntityStatus>();
        playerLevel = player.GetComponent<PlayerLevel>();
        skillController = player.GetComponentInChildren<PlayerSkillController>();
    }

    private void Start()
    {
        //statusPanel.Init(entityStatus, playerLevel);
        //skillPanel.Init(skillController);

        rootPanel.SetActive(false);
    }

    private void Update()
    {
        if (player.input.Player.Menu.WasPressedThisFrame())
        {
            bool active = !rootPanel.activeSelf;
            rootPanel.SetActive(active);
            Time.timeScale = active ? 0f : 1f;
        }
    }

}
