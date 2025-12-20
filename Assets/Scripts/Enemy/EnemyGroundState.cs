using UnityEngine;

// idle, moveの親state
public class EnemyGroundState : EnemyState
{
    public EnemyGroundState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        var hit = enemy.PlayerDetection();
        if (hit)
        {
            enemy.TryEnterBattleState(hit.transform);
        }

    }


}
