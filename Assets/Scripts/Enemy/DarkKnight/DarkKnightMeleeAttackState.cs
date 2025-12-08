using UnityEngine;

public class DarkKnightMeleeAttackState : EnemyAttackState
{
    public DarkKnightMeleeAttackState(DarkKnight enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
}
