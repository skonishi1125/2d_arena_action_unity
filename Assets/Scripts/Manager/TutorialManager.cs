using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private Player Player;

    private void Awake()
    {
        Player = FindFirstObjectByType<Player>();
        if (!LogHelper.AssertNotNull(Player, nameof(Player), this))
            return;
    }

    private void OnEnable()
    {
        Enemy.OnExpGained += AddExp;
        Player.Level.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        Enemy.OnExpGained -= AddExp;
        Player.Level.OnLevelUp -= HandleLevelUp;
    }

    private void AddExp(int exp)
    {
        if (Player == null)
        {
            Debug.LogWarning("Tutorial:AddExp(): Playerがnullです。");
            return;
        }
        Player.Level.AddExp(exp);
    }

    private void HandleLevelUp(int newLevel)
    {
        if (Player == null)
        {
            Debug.LogWarning("Tutorial:HandleLevelUp(): Playerがnullです。");
            return;
        }
        Player.Vfx.CreateOnLevelUpVfx(Player.transform);
    }

}
