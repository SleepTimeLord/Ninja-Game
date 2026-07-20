using UnityEngine;
using static EnemyManager;

/// <summary>
/// The state for when the enemy is punching
/// </summary>
public class PunchState : EnemyState
{
    /// <summary>
    /// A reference to the enemy's animation manager
    /// </summary>
    private EnemyAnimationManager animationManager;

    /// <summary>
    /// Whether or not the punch is finished
    /// </summary>
    private bool isPunchFinished;


    /// <summary>
    /// This state can only be exited when the punch animation is complete
    /// </summary>
    public override bool CanExit
    {
        get
        {
            return this.isPunchFinished;
        }
    }

    public PunchState(Enemy enemy, EnemyStateMachine stateMachine)
        : base(enemy, stateMachine)
    {
        
    }


    /// <summary>
    /// Sets up all the variables
    /// </summary>
    public override void Enter()
    {
        Debug.Log("Entering punch state");
        this.animationManager = this.enemy.AnimationManager;
        this.animationManager.PlayPunch();
    }

    /// <summary>
    /// Resets all the references that's repeatable
    /// </summary>
    public override void Exit()
    {
        this.isPunchFinished = false;
    }

    /// <summary>
    /// Checks for what state to change the enemy into following the punch
    /// </summary>
    public override void Update()
    {
        if (!this.isPunchFinished)
        {
            return;
        }

        switch (this.enemy.defaultState)
        {
            case GlobalState.WANDER:
                this.stateMachine.TryChangeState(this.stateMachine.WanderState);
                break;

            case GlobalState.CHASE:
                this.stateMachine.TryChangeState(this.stateMachine.ChaseState);
                break;
        }
    }
    
    /// <summary>
    /// The actions to occur once the animation is complete
    /// </summary>
    public void OnAnimationFinished()
    {
        this.isPunchFinished = true;
    }
}
