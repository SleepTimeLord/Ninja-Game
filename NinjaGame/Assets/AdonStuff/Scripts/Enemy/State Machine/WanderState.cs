using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The state where the enemy isn't aware of the Player
/// </summary>
public class WanderState : EnemyState
{
    /// <summary>
    /// The approximate time it takes, in seconds, before the enemy tries to find another place
    /// to wander to again
    /// </summary>
    private const float TimeBetweenPathCreationMedian = 3f;

    /// <summary>
    /// How fast an enemy should move while in this state
    /// </summary>
    private const float MovementSpeed = 5f;

    /// <summary>
    /// The base chance an enemy has to search 
    /// </summary>
    private const float BaseSearchChance = 0.55f;

    private GameObject currentTarget;

    /// <summary>
    /// How long the enemy needs to be idle for
    /// </summary>
    private float idleTime;

    /// <summary>
    /// Whether or not the enemy is currently staying still between wandering paths
    /// </summary>
    private bool isIdle;


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


    public WanderState(Enemy enemy, EnemyStateMachine stateMachine) 
        : base(enemy,stateMachine)
    {
    }


    /// <summary>
    /// Makes the enemy start to wander to a random place
    /// </summary>
    public override void Enter()
    {
        Debug.Log("entering wander state");
        this.idleTime = 0f;
        this.isIdle = false;

        KeyValuePair<Platform, Vector2> target = GetTarget();

        this.enemy.StartMovementTo(target.Key, target.Value);
    }

    /// <summary>
    /// Clears the path before the next state
    /// </summary>
    public override void Exit()
    {
        Debug.Log("exiting wander state");
        this.enemy.ClearPath();
    }

    public override void Update()
    {
        // Before everything, check whether or not we gotta even be in this state
        if (this.enemy.defaultState is EnemyManager.GlobalState.CHASE)
        {
            this.enemy.StateMachine.TryChangeState(this.enemy.StateMachine.ChaseState);
        }

        // Now, check whether or not the enemy actually needs to update its movement
        if (this.enemy.HasPath)
        {
            this.enemy.TickMovement(MovementSpeed);
            return;
        }

        /*
         * At this point, we know that the enemy isn't moving, so we need to check whether or not 
         * its idle or if it's at a trashcan
         */
        if (!this.isIdle)
        {
            if (this.currentTarget != null && 
                this.currentTarget.TryGetComponent<Trashcan>(out Trashcan trashcan))
            {
                this.enemy.StateMachine.TryChangeState(this.enemy.StateMachine.PunchState);
                return;
            }

            // If the target wasn't a trash can, then it means that the enemy needs to be idle
            StartIdle();
            return;
        }

        /* When the enemy is idle, we can begin decreasing the time until it's no 
         * longer idle and check it
         */
        this.idleTime -= Time.deltaTime;

        // Now, we can check whether or not we want to wander
        if (this.idleTime <= 0f)
        {
            this.isIdle = false;

            Debug.Log("Making him wander again");

            KeyValuePair<Platform, Vector2> newTarget = GetTarget();
            this.enemy.StartMovementTo(newTarget.Key, newTarget.Value);
        }
    }

    /// <summary>
    /// Gets a target in the form of its platform and its exact position
    /// </summary>
    /// <returns>a target's platform and specific position</returns>
    private KeyValuePair<Platform, Vector2> GetTarget()
    {
        KeyValuePair<Platform, Vector2> target;

        if (ShouldSearch())
        {
            // If the enemy should search, then that means the target is a trashcan
            Trashcan targetTrashcan = this.enemy.GetRandomTrashcan();

            target = new KeyValuePair<Platform, Vector2>(targetTrashcan.CurrentPlatform, 
                targetTrashcan.Position);
        }
        else
        {
            Platform targetPlatform = this.enemy.GetRandomPlatform();
            target = new KeyValuePair<Platform, Vector2>(targetPlatform,
                targetPlatform.GetValidPoint());
        }

        this.currentTarget = target.Key.gameObject;

        return target;
    }

    /// <summary>
    /// Should the enemy search based on the number of enemies on screen and the
    /// base search chance?
    /// </summary>
    /// <returns>whether or not the enemy is searching for something while wandering</returns>
    private bool ShouldSearch()
    {
        float searchChance = Mathf.Clamp01(BaseSearchChance * this.enemy.searchScale);

        if (Random.value <= searchChance)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// The actions to occur once the enemy has gone idle
    /// </summary>
    private void StartIdle()
    {
        Debug.Log("Going idle");
        this.isIdle = true;
        this.idleTime = Random.Range((TimeBetweenPathCreationMedian * 0.5f),
            TimeBetweenPathCreationMedian * 1.5f);
    }
}
