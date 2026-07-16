using UnityEngine;

/// <summary>
/// The state where the Enemy is actively pursuing the player
/// </summary>
public class ChaseState : EnemyState
{
    public ChaseState(EnemyStateMachine stateMachine, Enemy enemy)
        : base(stateMachine, enemy)
    {
        
    }

    public override void CheckForTransition()
    {
        throw new System.NotImplementedException();
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}
