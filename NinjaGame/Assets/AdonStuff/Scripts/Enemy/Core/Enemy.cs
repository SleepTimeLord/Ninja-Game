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

    /// <summary>
    /// A reference to the enemy's state machine
    /// </summary>
    [SerializeField] private EnemyStateMachine stateMachine;

    [Header("Sprite & Others")]
    /// <summary>
    /// A reference to an enemy's collider
    /// </summary>
    [SerializeField] private Collider2D spriteCollider;


    /// <summary>
    /// The scale of which the enemy should search
    /// </summary>
    public float searchScale;

    /// <summary>
    /// How fast the enemy is moving while chasing the player
    /// </summary>
    private const float ChaseSpeed = 5f;

    /// <summary>
    /// How fast the enemy is moving while wandering to a random spot
    /// </summary>
    private const float WanderSpeed = 2f;

    /// <summary>
    /// The reference to the current platform graph
    /// </summary>
    private PlatformGraph currentGraph;

    /// <summary>
    /// A reference to the manager controlling the enemy
    /// </summary>
    private EnemyManager manager;


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
    /// Returns whether or not an enemy is in the middle of a transition
    /// </summary>
    public bool IsInTransition
    {
        get
        {
            return this.movement.IsInTransition;
        }
    }


    /// <summary>
    /// The actions the prefab should take when it's (re)introduced into the world
    /// </summary>
    /// <param name="graph">a reference to the platform graph that's being used by the 
    /// manager</param>
    /// <param name="isInWanderState">whether or not the initial state for the enemy will consist
    /// of it wandering</param>
    /// <param name="spawnPosition">The v</param>
    public void Initialize(PlatformGraph graph, bool isInWanderState, Vector2 spawnPosition)
    {
        this.currentGraph = graph;
        this.navigator.Graph = graph;

        // The paths are reset and the init spawn position is given BEFORE the state changes
        ResetEnemy(spawnPosition);

        // Then the initial platform is found
        this.platformTracker.FindPlatformBelow();

        // Finally, start the behavior
        this.stateMachine.ChangeState(
            isInWanderState ? this.stateMachine.WanderState : this.stateMachine.ChaseState);
    }

    /// <summary>
    /// Ensures that everything is reset to its default values
    /// </summary>
    /// <param name="spawnPosition">the position to which the enemy</param>
    public void ResetEnemy(Vector2 spawnPosition)
    {
        this.movement.ClearPathParams();
        this.transform.position = spawnPosition;
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
        /// <summary>
        /// The base odds of the enemy searching
        /// </summary>
        const float BaseSearchChance = 0.35f;

        // Clamped so that the chance is never above 100
        float searchChance = Mathf.Clamp01(BaseSearchChance * searchScale);

        if (Random.value <= searchChance)
        {
            Debug.Log("Search successful");
            return;
        }
        else
        {
            /*
             * There are (mainly) two parts to wandering: Going to a location, and waiting there.
             * That will be determined here.
             */
            // First, a location to wander to should be calculated
            Platform targetPlatform = this.currentGraph.GetRandomPlatform();
            Vector2 trueTargetLocation = targetPlatform.GetValidPoint();
            Debug.Log($"Starting from platform {this.CurrentPlatform} to {targetPlatform.name}");

            if (this.CurrentPlatform == null)
            {
                Debug.LogWarning("The current platform is null");
            }

            // Next, we can use this random platform to create a path
            List<PlatformTransition> platformPath = this.navigator.SearchPath(this.CurrentPlatform, targetPlatform);

            // Then we set that path for movement to begin
            this.movement.SetPath(platformPath, trueTargetLocation);
        }

        // There isn't any more to do here, as the rest is handled directly in the WanderState
    }

    /// <summary>
    /// Called every frame. Updates the movement algorithm
    /// </summary>
    /// <param name="isInWanderState">whether or not the enemy is wandering</param>
    /// <param name="updatedSearchScale">the new search scale that should be reapplied</param>
    public void Tick(bool isInWanderState, float updatedSearchScale)
    {
        this.searchScale = updatedSearchScale;

        if (isInWanderState)
        {
            this.movement.UpdateMovement(WanderSpeed, this.CurrentPlatform);
        }
        else
        {
            this.movement.UpdateMovement(ChaseSpeed, this.CurrentPlatform);
        }
    }

    /// <summary>
    /// The actions to occur once the enemy dies
    /// </summary>
    public void Die()
    {
        this.movement.ClearPathParams();
        this.gameObject.SetActive(false);
        this.enabled = false;
    }
}
