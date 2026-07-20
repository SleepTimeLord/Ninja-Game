using NUnit.Framework;
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
    /// The time the enemy has left to being idle
    /// </summary>
    private float idleTimer;

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


    public WanderState(Enemy enemy) 
        : base(enemy)
    {
    }


    /// <summary>
    /// Makes the enemy start to wander to a random place
    /// </summary>
    public override void Enter()
    {
        this.idleTimer = 0f;
        this.isIdle = false;
        this.enemy.Wander(this.enemy.searchScale);
    }

    //
    public override void Exit()
    {
    }

    public override void Update()
    {
        // First, check whether or not the enemy actually needs to update its movement
        if (this.enemy.HasPath)
        {
            this.enemy.Tick(true, this.enemy.searchScale);
            return;
        }

        /*
         * At this point, we know that the enemy isn't moving, so we need to check whether or not 
         * its idle
         */
        if (!this.isIdle)
        {
            Debug.Log("Bro stopped and needs to have the timer started");
            this.isIdle = true;
            enemy.IsIdle = true;
            this.enemy.IsWalking = false;
            this.enemy.IsJumpingDropping = false;
            this.idleTimer = GetIdleTime();
        }

        // Almost immediately after, we can begin decreasing it
        this.idleTimer -= Time.deltaTime;

        // Now, we can check whether or not we want to wander
        if (this.idleTimer <= 0f)
        {
            this.isIdle = false;
            this.enemy.IsIdle = false;

            // Once again, a temp value here is used
            Debug.Log("Making him wander again");
            this.enemy.Wander(this.enemy.searchScale);
        }
    }

    /// <summary>
    /// Gets a random amount of time that the enemy will be idle for
    /// </summary>
    /// <returns>the time that the enemy will be idle for</returns>
    private float GetIdleTime()
    {
        return Random.Range((TimeBetweenPathCreationMedian * 0.5f), 
            TimeBetweenPathCreationMedian * 1.5f);
    }
}
