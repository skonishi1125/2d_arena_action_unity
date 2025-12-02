using UnityEngine;

public abstract class EntityState
{
    // 子クラス全てで使用するため, protected
    protected StateMachine stateMachine;
    // Animator側で作るパラメータの名前
    // 例えばmoveがtrueだと、現在moveStateである状態を示すことになる。
    protected string animBoolName;

    // 敵味方共通のコンポーネントなど
    protected Rigidbody2D rb;
    protected Animator anim;

    // ダッシュなど、時間制限のあるStateで使用するタイマー
    protected float stateTimer;

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // 入口処理 状態に入ったときにやること
    public virtual void Enter()
    {
        // anim取得の流れ Playerの例
        // * Entity.csでanimをGetComponentInChildrenで取得。
        // * stateを作るとき、new PlayerJumpState(this, stateMachine, "jumpFall");という感じで渡す。
        // * PlayerState側のコンストラクタが anim = player.anim; としてセット。
        // * 本メソッドが、入口処理でjumpFallというanimパラメータにtrueをセットしている。
        // 
        // これで、敵stateを作るときも、敵stateコンストラクタでenemy.animとしてセットすれば使いまわせる。
        anim.SetBool(animBoolName, true);

    }

    // 状態中にやること
    // 論理演算系のUpdate。状態遷移、入力受付, anim切替, 条件式評価
    // フレームの最終決定
    public virtual void LogicUpdate()
    {
        stateTimer -= Time.deltaTime;
    }

    // 物理演算系(移動、ジャンプ)のUpdate()
    // rbに速度加算, addForce, 物理挙動などrg, coを調整
    public virtual void PhysicsUpdate()
    {
    }

    // 出口処理 状態が終わったとき、次のStateを呼ぶときにやること
    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }

}
