using UnityEngine;

/// <summary>
/// A state within an enemy
/// </summary>
public abstract class EnemyState
{
    /// <summary>
    /// A reference to the state machine the state is connected to
    /// </summary>
    protected EnemyStateMachine stateMachine;

    /// <summary>
    /// The enemy that the state is representing
    /// </summary>
    protected Enemy enemy;


    /// <summary>
    /// Initializes the state machine, at the minimum
    /// </summary>
    /// <param name="stateMachine">a reference to the state machine the state will be 
    /// connected to</param>
    /// <param name="enemy">the enemy the state is doing all of this for</param>
    protected EnemyState(EnemyStateMachine stateMachine, Enemy enemy)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
    }


    /// <summary>
    /// The actions that should take place once the state is entered
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// The actions to occur every frame following the Enter method
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// The actions to occur before the state exits into another
    /// </summary>
    public abstract void Exit();

    /// <summary>
    /// The requirements for the Exit method to be called by the enemy
    /// </summary>
    public abstract void CheckForTransition();
}
