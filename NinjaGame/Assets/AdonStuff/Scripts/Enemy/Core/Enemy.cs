using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    /// <summary>
    /// A reference to the enemy's animation manager
    /// </summary>
    [SerializeField] private EnemyAnimationManager animationManager;

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
    /// An iterative time for the amount of the time it's been since an enemy has officially died
    /// </summary>
    private float deathAnimationCounter;

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
    /// A reference to the manager holding all of the trashcans
    /// </summary>
    private TrashcanContainer trashcanContainer;

    /// <summary>
    /// A reference to the target the enemy currently has
    /// </summary>
    /// <remarks>In this case, it can really either be a trashcan or a platform</remarks>
    private GameObject currentTarget;


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
    /// Returns whether or not the enemy is ready to die
    /// </summary>
    public bool IsReadyForDeath
    {
        get
        {
            if (this.deathAnimationCounter >= 1f)
            {
                this.IsDying = false;
                this.deathAnimationCounter = 0f;
                return true;
            }
            return false;
        }
    }

    #region Animation-Related Properties
    /// <summary>
    /// Sets whether the enemy is idle
    /// </summary>
    public bool IsIdle
    {
        set
        {
            this.animationManager.IsIdle = value;
        }
    }

    /// <summary>
    /// Sets whether the enemy is walking
    /// </summary>
    public bool IsWalking
    {
        set
        {
            this.animationManager.IsWalking = value;
        }
    }

    /// <summary>
    /// Gets and sets whether the enemy is jumping or dropping
    /// </summary>
    public bool IsJumpingDropping
    {
        get
        {
            return this.animationManager.IsJumpingDropping;
        }
        set
        {
            this.animationManager.IsJumpingDropping = value;
        }
    }

    /// <summary>
    /// Gets and sets whether the enemy is dying
    /// </summary>
    public bool IsDying
    {
        get
        {
            return this.animationManager.IsDying;
        }
        set
        {
            this.animationManager.IsDying = value;
        }
    }

    /// <summary>
    /// Gets and sets whether the enemy is punching something or not
    /// </summary>
    public bool IsPunching
    {
        get
        {
            return this.animationManager.IsPunching;
        }
        set
        {
            this.animationManager.IsPunching = value;
        }
    }
    #endregion

    /// <summary>
    /// The ultimate target that the enemy has to go for during a route
    /// </summary>
    public GameObject CurrentTarget
    {
        get
        {
            return this.currentTarget;
        }
    }

    /// <summary>
    /// A reference to the sprite collider and its position
    /// </summary>
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
    /// The actions the prefab should take when it's (re)introduced into the world
    /// </summary>
    /// <param name="graph">a reference to the platform graph that's being used by the 
    /// manager</param>
    /// <param name="isInWanderState">whether or not the initial state for the enemy will consist
    /// of it wandering</param>
    /// <param name="spawnPosition">The place the enemy will start off at</param>
    /// <param name="trashcanContainer">A reference to the trash can container</param>
    public void Initialize(PlatformGraph graph, TrashcanContainer trashcanContainer, 
        bool isInWanderState, Vector2 spawnPosition)
    {
        this.trashcanContainer = trashcanContainer;
        this.currentGraph = graph;
        this.navigator.Graph = graph;

        // The paths are reset and the init spawn position is given BEFORE the state changes
        ResetEnemy(spawnPosition);

        // Then the initial platform needs to be found
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
        this.animationManager.ResetAnimation();
        this.spriteCollider.transform.position = spawnPosition;
        this.deathAnimationCounter = 0f;
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
        const float BaseSearchChance = 0.55f;

        float searchChance = Mathf.Clamp01(BaseSearchChance * searchScale);

        /*
         * Both potential events (searching and not searching) still have the same process of 
         * finding a target platform and having the location, thus why they're up here instead of 
         * being local to the if-statement
         */
        Platform targetPlatform;
        Vector2 trueTargetLocation;

        if (Random.value <= searchChance)
        {
            Trashcan randomTrashcan = this.trashcanContainer.GetRandomTrashcan();

            targetPlatform = randomTrashcan.CurrentPlatform;
            trueTargetLocation = randomTrashcan.Position;
            this.currentTarget = randomTrashcan.gameObject;
            Debug.Log($"Searching from platform {this.CurrentPlatform} to {targetPlatform.name}; " +
                $"home of {randomTrashcan.name}");
        }
        else
        {
            targetPlatform = this.currentGraph.GetRandomPlatform();
            trueTargetLocation = targetPlatform.GetValidPoint();
            this.currentTarget = targetPlatform.gameObject;
            Debug.Log($"Starting from platform {this.CurrentPlatform} to {targetPlatform.name}");
        }

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
    /// <param name="updatedSearchScale">the new search scale that should be reapplied</param>
    public void Tick(bool isInWanderState, float updatedSearchScale)
    {
        this.searchScale = updatedSearchScale;
        
        /*
         * Movement isn't necessary when there's a bigger animation going on. That's handled in 
         * their own respective scripts
         */
        if (IsDying)
        {
            this.deathAnimationCounter += Time.deltaTime;
            return;
        }

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
    public void BeginDeath()
    {
        this.movement.ClearPathParams();
        this.IsDying = true;
    }
}
