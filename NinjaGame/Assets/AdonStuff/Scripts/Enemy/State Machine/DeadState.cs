using UnityEngine;

/// <summary>
/// The state where the enemy has ceased to exist
/// </summary>
public class DeadState : EnemyState
{
    /// <summary>
    /// The location of which the enemy will respawn once it comes back
    /// </summary>
    private Vector2 randomRespawnPoint;


    public DeadState(EnemyStateMachine stateMachine, Enemy enemy) 
        : base(stateMachine, enemy)
    {
    }


    // Again, nothing really needs to happen/be checked here because the enemy is dead
    public override void CheckForTransition()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Sets the respawn point for when it comes back
    /// </summary>
    public override void Enter()
    {
        this.randomRespawnPoint = this.enemy.GetRandomRespawnPoint();
    }

    /// <summary>
    /// Sets the spawn point for the enemy
    /// </summary>
    public override void Exit()
    {
        this.enemy.SetRespawnPoint(this.randomRespawnPoint);
    }

    // Nothing really needs to happen while the enemy is dead. 
    public override void Update()
    {
        return;
    }
}
