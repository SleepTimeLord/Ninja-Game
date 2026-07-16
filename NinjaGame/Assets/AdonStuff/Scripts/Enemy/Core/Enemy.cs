using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main brain of an enemy. Holds each component and manages each aspect of it
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Managers")]
    /// <summary>
    /// A reference to the tracker the enemy has to know which tracker its on
    /// </summary>
    [SerializeField] private PlatformTracker platformTracker;

    /// <summary>
    /// A reference to the movement manager
    /// </summary>
    [SerializeField] private EnemyMovement movement;

    /// <summary>
    /// A reference to the navigator/pathfinder the enemy uses
    /// </summary>
    [SerializeField] private PlatformNavigator navigator;

    [Header("Platform-Specifics")]
    /// <summary>
    /// A reference to the graph used in the main game
    /// </summary>
    [SerializeField] private PlatformGraph graph;

    [Header("Sprite & Others")]
    /// <summary>
    /// A reference to an enemy's collider
    /// </summary>
    [SerializeField] private Collider2D spriteCollider;

    /// <summary>
    /// How fast the enemy is moving while chasing the player
    /// </summary>
    private const float ChaseSpeed = 5f;

    /// <summary>
    /// How fast the enemy is moving while wandering to a random spot
    /// </summary>
    private const float WanderSpeed = 2f;


    /// <summary>
    /// Returns whether the enemy has a path and needs to move
    /// </summary>
    public bool HasPath
    {
        get
        {
            return this.movement.HasPath;
        }
    }

    public Collider2D SpriteCollider
    {
        get
        {
            return this.spriteCollider;
        }
    }

    /// <summary>
    /// Returns the platform the enemy is on
    /// </summary>
    public Platform CurrentPlatform
    {
        get
        {
            return this.platformTracker.CurrentPlatform;
        }
    }


    /// <summary>
    /// Called by the wander state to kickstart the exclusive process involving wandering
    /// </summary>
    /// <param name="searchScale">how much lesser/greater the chance that the enemy searches
    /// an interactable item</param>
    public void Wander(float searchScale)
    {
        /*
         * For an enemy to wander it needs to first determine whether it's searching or 
         * if it's not. This is determined with a random value spin.
         */
        // NOTE: This hasn't been implemented, so it's not too necessary yet
        /// <summary>
        /// The odds of the enemy searching
        /// </summary>
        const float BaseSearchChance = 0.55f;

        /*
         * There are (mainly) two parts to wandering: Going to a location, and waiting there.
         * That will be determined here.
         */
        // First, a location to wander to should be calculated
        Platform targetPlatform = this.graph.GetRandomPlatform();
        Vector2 trueTargetLocation = targetPlatform.GetValidPoint();
        Debug.Log($"Target Platform: {targetPlatform.name}");

        // Next, we can use this random platform to create a path
        List<PlatformTransition> platformPath = this.navigator.SearchPath(this.CurrentPlatform, targetPlatform);

        // Then we set that path for movement to begin
        this.movement.SetPath(platformPath, trueTargetLocation);

        // There isn't any more to do here, as the rest is handled directly in the WanderState
    }

    /// <summary>
    /// Called every frame. Updates the movement algorithm
    /// </summary>
    /// <param name="isInWanderState">whether or not the enemy is wandering</param>
    public void Tick(bool isInWanderState)
    { 
        if (isInWanderState)
        {
            this.movement.UpdateMovement(WanderSpeed, this.CurrentPlatform);
        }
        else
        {
            this.movement.UpdateMovement(ChaseSpeed, this.CurrentPlatform);
        }
    }

    public void SetRespawnPoint(Vector2 respawnPoint)
    {
        this.transform.position = respawnPoint;
    }

    /// <summary>
    /// Finds a respawn point based on a platform
    /// </summary>
    /// <returns>a random respawn point</returns>
    public Vector2 GetRandomRespawnPoint()
    {
        Platform randomPlatform = this.graph.GetRandomPlatform();
        return randomPlatform.GetValidPoint();
    }
}
