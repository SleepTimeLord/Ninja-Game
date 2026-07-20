using UnityEngine;

/// <summary>
/// The state for when the enemy is dead
/// </summary>
public class DeathState : EnemyState
{
    /// <summary>
    /// How long it takes for the enemy to despawn
    /// </summary>
    private const float TimeUntilDespawn = 1.5f;

    /// <summary>
    /// A reference to the enemy's animation manager
    /// </summary>
    private EnemyAnimationManager animationManager;

    /// <summary>
    /// Whether or not the death of the enemy is fully complete
    /// </summary>
    private bool isDeathAnimationFinished;

    /// <summary>
    /// The time it takes until the enemy despawns after the death is complete
    /// </summary>
    private float timeUntilDespawn;

    
    /// <summary>
    /// Returns whether a death is completely done and the enemy is good to enter the dead queue
    /// </summary>
    public bool IsDeathComplete
    {
        get
        {
            /*
             * An enemy needs to have completed the entire animation and the despawning time has 
             * to have complete
             */
            return this.isDeathAnimationFinished && this.timeUntilDespawn <= 0f;
        }
    }


    public DeathState(Enemy enemy, EnemyStateMachine stateMachine) 
        : base(enemy, stateMachine)
    {
    }

    /// <summary>
    /// Sets the time until despawn clock and begins the death animation
    /// </summary>
    public override void Enter()
    {
        this.isDeathAnimationFinished = false;
        this.animationManager = this.enemy.AnimationManager;
        Debug.Log("Entered death state");

        this.timeUntilDespawn = TimeUntilDespawn;
        this.animationManager.PlayDeath();
    }

    // Nothing needs to happen here, as Exit is impossible to get to
    public override void Exit()
    {

    }

    /// <summary>
    /// Decreases the despawn counter when needed
    /// </summary>
    public override void Update()
    {
        if (this.isDeathAnimationFinished)
        {
            this.timeUntilDespawn -= Time.deltaTime;
        }
    }

    /// <summary>
    /// The actions to occur once the death animation is complete
    /// </summary>
    public void OnAnimationComplete()
    {
        this.isDeathAnimationFinished = true;
    }
}
