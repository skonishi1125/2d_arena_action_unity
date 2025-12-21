using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [SerializeField] private int baseExp = 1;
    private int currentExp;
    public int Exp => currentExp;

    private void Awake()
    {
        currentExp = baseExp;
    }

    public void ApplyExpMultiplier(float multiplier)
    {
        // multiplier は 1.0 = 等倍
        currentExp = Mathf.Max(0, Mathf.RoundToInt(baseExp * multiplier));
    }
}
