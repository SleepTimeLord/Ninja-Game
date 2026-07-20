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

    /// <summary>
    /// The attack manager that controls all of the attacks the enemy has
    /// </summary>
    [SerializeField] private EnemyAttackManager attackManager;

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
    /// The state the enemy should be at a default
    /// </summary>
    public EnemyManager.GlobalState defaultState;

    /// <summary>
    /// The reference to the current platform graph
    /// </summary>
    private PlatformGraph currentGraph;

    /// <summary>
    /// A reference to the manager holding all of the trashcans
    /// </summary>
    private TrashcanContainer trashcanContainer;


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

    /*
     * I know a backing field isn't for this, and I just discovered you can do these, 
     * thus why this is different
     */
    /// <summary>
    /// A reference to to the current platform the player is standing on
    /// </summary>
    public Platform PlayerPlatform { get; set; }

    /// <summary>
    /// A reference to the position the player is at
    /// </summary>
    public Vector2 PlayerPosition { get; set; }

    /// <summary>
    /// A reference to the state machine the enemy has
    /// </summary>
    public EnemyStateMachine StateMachine
    {
        get
        {
            return this.stateMachine;
        }
    }

    /// <summary>
    /// Returns a reference to the current state the state machine is in
    /// </summary>
    public EnemyState CurrentState
    {
        get
        {
            return this.stateMachine.CurrentState;
        }
    }

    /// <summary>
    /// Returns a reference to the animation manager the enemy uses
    /// </summary>
    public EnemyAnimationManager AnimationManager
    {
        get
        {
            return this.animationManager;
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
        bool isInWanderState, Vector2 spawnPosition, CharacterController playerController,
        Platform playerPlatform, Vector2 playerPosition)
    {
        this.trashcanContainer = trashcanContainer;
        this.currentGraph = graph;
        this.navigator.Graph = graph;
        this.attackManager.Controller = playerController;
        this.PlayerPlatform = playerPlatform;
        this.PlayerPosition = playerPosition;

        // The paths are reset and the init spawn position is given BEFORE the state changes
        this.transform.position = spawnPosition;

        // Then the initial platform needs to be found
        this.platformTracker.FindPlatformBelow();

        // Finally, start the behavior
        this.stateMachine.TryChangeState(
            isInWanderState ? this.stateMachine.WanderState : this.stateMachine.ChaseState);
    }

    /// <summary>
    /// The final things that are needed to be done before the enemy goes into the death queue
    /// </summary>
    public void CompleteDeath()
    {
        this.movement.ClearPathParams();
        this.attackManager.DisableHitbox();
        this.stateMachine.ResetState();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Clears the path that the enemy currently has
    /// </summary>
    public void ClearPath()
    {
        this.movement.ClearPathParams();
    }

    /// <summary>
    /// Enables the AttackManagers hitbox
    /// </summary>
    public void EnablePunchHitbox()
    {
        this.attackManager.EnableHitbox();
    }

    /// <summary>
    /// Disables the AttackManager's hitbox
    /// </summary>
    public void DisablePunchHitbox()
    {
        this.attackManager.DisableHitbox();
    }

    /// <summary>
    /// The logic that kickstarts movement of an enemy
    /// </summary>
    /// <param name="platform">the platform that the enemy is going to</param>
    /// <param name="position">the exact position within that platform the enemy 
    /// is going to</param>
    public void StartMovementTo(Platform platform, Vector2 position)
    {
        Debug.Log($"player platform name {platform.name}");
        Debug.Log($"Starting pathing process to {platform.name}");

        // Get the path
        List<PlatformTransition> path =
            this.navigator.SearchPath(CurrentPlatform, platform);

        // Then, have the movement manager take that path into a viable route
        this.movement.SetPath(path, position);
    }

    /// <summary>
    /// Called every frame. Updates the movement algorithm
    /// </summary>
    /// <param name="speed">how fast the enemy should be moving</param>
    public void TickMovement(float speed)
    {
        this.movement.UpdateMovement(speed, this.CurrentPlatform);
    }

    /// <summary>
    /// Finds a random trashcan for the enemy to wander into from the trashcan container
    /// </summary>
    /// <returns>a random trashcan</returns>
    public Trashcan GetRandomTrashcan()
    {
        return this.trashcanContainer.GetRandomTrashcan();
    }

    /// <summary>
    /// Finds a random platform for the enemy to wander into from the platform container
    /// </summary>
    /// <returns>a random platform</returns>
    public Platform GetRandomPlatform()
    {
        return this.currentGraph.GetRandomPlatform();
    }
}
