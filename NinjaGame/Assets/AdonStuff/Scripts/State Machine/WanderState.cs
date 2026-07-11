using UnityEngine;

/// <summary>
/// The state where the enemy isn't aware of the Player
/// </summary>
public class WanderState : EnemyState
{
    public WanderState(EnemyStateMachine stateMachine) 
        : base(stateMachine)
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
