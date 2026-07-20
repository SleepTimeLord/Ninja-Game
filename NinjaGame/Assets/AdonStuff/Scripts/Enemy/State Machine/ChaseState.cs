using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The state where the Enemy is actively pursuing the player
/// </summary>
public class ChaseState : EnemyState
{
    /// <summary>
    /// How fast the player is moving in pixels per secons
    /// </summary>
    private const float MovementSpeed = 10f;
    
    /// <summary>
    /// The time before the enemy recalculates a new path to the player in seconds
    /// </summary>
    private const float RepathTime = 0.5f;

    /// <summary>
    /// The distance the enemy has to be to switch to the punch in pixels
    /// </summary>
    private const float PunchingDistance = 0.45f;

    /// <summary>
    /// The platform the player is on
    /// </summary>
    private Platform playerPlatform;

    /// <summary>
    /// The exact position the player is at
    /// </summary>
    private Vector2 playerPosition;

    /// <summary>
    /// The time until the enemy repaths to the player
    /// </summary>
    private float repathTimer;



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

    public ChaseState(Enemy enemy, EnemyStateMachine stateMachine)
        : base(enemy, stateMachine)
    {
        
    }

    /// <summary>
    /// Starts targeting the player
    /// </summary>
    public override void Enter()
    {
        Debug.Log("entering chase state");
        this.playerPlatform = this.enemy.PlayerPlatform;
        this.playerPosition = this.enemy.PlayerPosition;

        // If the player is close enough already, frick it we ball
        if (IsCloseEnough())
        {
            this.stateMachine.TryChangeState(this.stateMachine.PunchState);
            return;
        }
        else
        {
            this.enemy.StartMovementTo(this.playerPlatform, this.playerPosition);
            this.repathTimer = RepathTime;
        }
    }

    /// <summary>
    /// Clears the path before the next state
    /// </summary>
    public override void Exit()
    {
        Debug.Log("exiting chase state");
        this.enemy.ClearPath();
    }

    public override void Update()
    {
        // Before everything, check whether or not we gotta even be in this state
        if (this.enemy.defaultState is EnemyManager.GlobalState.WANDER)
        {
            this.enemy.StateMachine.TryChangeState(this.enemy.StateMachine.WanderState);
        }

        this.playerPlatform = this.enemy.PlayerPlatform;
        this.playerPosition = this.enemy.PlayerPosition;

        // First, consider the timing for repathing
        if (!this.enemy.IsInTransition)
            repathTimer -= Time.deltaTime;

        if (repathTimer <= 0)
        {
            this.enemy.StartMovementTo(this.playerPlatform, this.playerPosition);
            repathTimer = RepathTime;
        }

        // Next, check for the distance the enemy is to the player and whether its worth changing
        if (IsCloseEnough())
        {
            this.stateMachine.TryChangeState(this.stateMachine.PunchState);
            return;
        }

        // After all is said and done, update the enemy's movement
        this.enemy.TickMovement(MovementSpeed);
    }

    /// <summary>
    /// Checks whether or not the enemy is close enough to the player to actually do anything
    /// </summary>
    /// <returns>whether or not the enemy is close enough to the player</returns>
    private bool IsCloseEnough()
    {
        /*
         * This is determined by whether the enemy is on the same platform as the player and if
         * the distance is close enough
         */
        return this.enemy.CurrentPlatform == this.enemy.PlayerPlatform && Vector2.Distance(
            this.enemy.transform.position, enemy.PlayerPosition) < PunchingDistance;
    }
}
