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

    }

    private void OnDisable()
    {
        Enemy.OnExpGained -= AddExp;
    }

    private void AddExp(int exp)
    {
        Debug.Log("tutorial add!");
        Player.Level.AddExp(exp);
    }

}
