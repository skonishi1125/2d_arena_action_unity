using UnityEngine;

public class PlayerIdleState : PlayerState
{
    // 親のEntityStateにコンストラクタが存在するので、同様に定義する
    public PlayerIdleState(Player player, StateMachine stateMachine, string statename) : base(player, stateMachine, statename)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
