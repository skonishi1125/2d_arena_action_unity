using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Player player;
    private PlayerLevel level;
    private PlayerVFX vfx;

    private void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        level = player.GetComponent<PlayerLevel>();
        vfx = player.GetComponent<PlayerVFX>();

    }

    private void OnEnable()
    {
        Enemy.OnExpGained += AddExp;
        level.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        Enemy.OnExpGained -= AddExp;
        level.OnLevelUp -= HandleLevelUp;
    }


    private void AddExp(int exp)
    {
        if (level == null)
        {
            Debug.LogWarning("Tutorial:AddExp(): levelがnullです。");
            return;
        }
        level.AddExp(exp);
    }

    private void HandleLevelUp(int newLevel)
    {
        if (player == null)
        {
            Debug.LogWarning("Tutorial:HandleLevelUp(): levelがnullです。");
            return;
        }
        vfx.CreateOnLevelUpVfx(player.transform);
    }

}
