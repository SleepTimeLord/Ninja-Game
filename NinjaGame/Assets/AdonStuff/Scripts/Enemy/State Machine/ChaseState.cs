using UnityEngine;

/// <summary>
/// The state where the Enemy is actively pursuing the player
/// </summary>
public class ChaseState : EnemyState
{
    /// <summary>
    /// Determines whether or not an enemy can get out of the state
    /// </summary>
    public override bool CanExit
    {
        get
        {
            return !this.enemy.IsInTransition;
        }
    }

    public ChaseState(Enemy enemy)
        : base(enemy)
    {
        
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
