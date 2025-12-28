using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Player player;
    private bool bound;

    private void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);

    }

    private void OnEnable()
    {
        Enemy.OnExpGained += AddExp;
        StartCoroutine(BindWhenReady());
    }

    private void OnDisable()
    {
        Enemy.OnExpGained -= AddExp;
        if (bound && player != null && player.Level != null)
            player.Level.OnLevelUp -= HandleLevelUp;

        bound = false;
    }

    private IEnumerator BindWhenReady()
    {
        if (bound) yield break;

        while (player == null)
            player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);

        while (player.Level == null)
            yield return null; // 1フレーム待つ

        player.Level.OnLevelUp += HandleLevelUp;
        bound = true;
    }

    private void AddExp(int exp)
    {
        if (player == null)
        {
            Debug.LogWarning("Tutorial:AddExp(): Playerがnullです。");
            return;
        }
        player.Level.AddExp(exp);
    }

    private void HandleLevelUp(int newLevel)
    {
        if (player == null)
        {
            Debug.LogWarning("Tutorial:HandleLevelUp(): Playerがnullです。");
            return;
        }
        player.Vfx.CreateOnLevelUpVfx(player.transform);
    }

}
