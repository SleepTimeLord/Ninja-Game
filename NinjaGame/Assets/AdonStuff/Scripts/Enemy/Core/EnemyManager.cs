using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using UnityEngine;

/// <summary>
/// Controls the state and mind of each enemy
/// </summary>
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// The state that each enemy should be in
    /// </summary>
    public enum GlobalState
    {
        WANDER,
        CHASE
    }

    /// <summary>
    /// Invoked when the player hits the enemy AFTER handling the entire process
    /// </summary>
    public event Action EnemyHit;

    [Header("Necessary Components")]
    /// <summary>
    /// A reference to the graph the platforms use
    /// </summary>
    [SerializeField] private PlatformGraph platformGraph;

    /// <summary>
    /// A reference to the trashcan container the scene uses
    /// </summary>
    [SerializeField] private TrashcanContainer trashcanContainer;

    /// <summary>
    /// A reference to the slash the enemy has
    /// </summary>
    [SerializeField] private SlashTrigger playerSlash;

    /// <summary>
    /// A reference to the player
    /// </summary>
    [SerializeField] private CharacterController playerController;

    [Header("Enemy Settings")]
    /// <summary>
    /// The prefab the enemy uses
    /// </summary>
    [SerializeField] private Enemy enemyPrefab;

    /// <summary>
    /// The number of enemies that are to be on-screen at once
    /// </summary>
    [SerializeField] private int MaxEnemies = 1;

    /// <summary>
    /// A reference to each enemy in the game that's alive
    /// </summary>
    private List<Enemy> activeEnemies;

    /// <summary>
    /// A reference to the enemies that are in 
    /// </summary>
    private List<Enemy> inTransitionEnemies;

    /// <summary>
    /// A reference to each of the enemies that are dead
    /// </summary>
    private Queue<Enemy> deadEnemies;

    /// <summary>
    /// The state of which each enemy should be at
    /// </summary>
    private GlobalState currentGlobalState;


    /// <summary>
    /// Returns the player's exact position
    /// </summary>
    public Vector2 PlayerPosition
    {
        get
        {
            return this.playerController.transform.position;
        }
    }

    /// <summary>
    /// Returns the platform the player is on
    /// </summary>
    public Platform PlayerPlatform
    {
        get
        {
            return this.playerController.ctx.platformTracker.CurrentPlatform;
        }
    }

    /// <summary>
    /// Returns whether or not the state for each enemy should be for it to wander
    /// </summary>
    private bool IsInWanderState
    {
        get
        {
            return this.currentGlobalState == GlobalState.WANDER;
        }
    }

    /// <summary>
    /// The scale of which 
    /// </summary>
    private float SearchScale
    {
        get
        {
            /// <summary>
            /// The lowest scale there can possibly be for searching
            /// </summary>
            const float MinSearchScale = 0.5f;

            /// <summary>
            /// The highest scale there can possibly be for searching
            /// </summary>
            const float MaxSearchScale = 2.75f;

            // How empty the screen is overall, calculated as a percentage
            float screenToEnemyEmptiness = 1f - (float)this.activeEnemies.Count / MaxEnemies;

            // Due to the fact that this relationship is, in nature, linear, it can be lerped
            return Mathf.Lerp(MinSearchScale, MaxSearchScale, screenToEnemyEmptiness);
        }
    }


    /// <summary>
    /// The actions taken once the manager loads
    /// </summary>
    public void Awake()
    {
        this.activeEnemies = new List<Enemy>();
        this.deadEnemies = new Queue<Enemy>();
        this.currentGlobalState = GlobalState.WANDER;
        this.playerSlash.EnemyHit += OnEnemyHit;
    }

    // Update is called once per frame
    public void Update()
    {
        /*
         * The first thing to do is check whether or not there's a sufficient number of enemies 
         * that aren't dead at the moment
         */
        if (this.activeEnemies.Count < MaxEnemies)
        {
            SpawnEnemy();
        }

        // Next, update each enemy and feed it the necessary information it needs
        for (int i = this.activeEnemies.Count - 1; i >= 0; i--)
        {
            Enemy currentEnemy = this.activeEnemies[i];

            currentEnemy.searchScale = this.SearchScale;
            currentEnemy.defaultState = this.currentGlobalState;
            currentEnemy.PlayerPlatform = PlayerPlatform;
            currentEnemy.PlayerPosition = PlayerPosition;

            if (currentEnemy.CurrentState is DeathState confirmedState && 
                confirmedState.IsDeathComplete)
            {
                this.activeEnemies.Remove(currentEnemy);
                this.deadEnemies.Enqueue(currentEnemy);
                currentEnemy.CompleteDeath();
            }
        }
    }

    /// <summary>
    /// Changes the global enemy state depending on whether the player is hidden or not
    /// </summary>
    /// <param name="isHidden">whether or not the player is hidden</param>
    public void ChangeGlobalEnemyState(bool isHidden)
    {
        if (isHidden)
        {
            this.currentGlobalState = GlobalState.WANDER;
            return;
        }
        this.currentGlobalState = GlobalState.CHASE;
    }

    /// <summary>
    /// The actions taken for when an enemy is about to spawn
    /// </summary>
    private void SpawnEnemy()
    {
        // The first thing to check is whether or not we can just get take a dead enemy
        Enemy soonToBeEnemy;
        Vector2 randomSpawnPoint = GetRandomPoint();

        Debug.Log("Got here");
        /*
         * If there isn't one, we just have to instantiate a new one before actually giving it
         * the values it needs to run properly
         */
        if (!this.deadEnemies.TryDequeue(out soonToBeEnemy))
        {
            soonToBeEnemy = Instantiate(this.enemyPrefab, this.gameObject.transform);
        }
        else
        {
            Debug.Log("Extracted " + soonToBeEnemy.name);
        }

        soonToBeEnemy.gameObject.SetActive(true);

        soonToBeEnemy.Initialize(this.platformGraph, this.trashcanContainer, IsInWanderState,
            randomSpawnPoint, this.playerController, PlayerPlatform, PlayerPosition);
        this.activeEnemies.Add(soonToBeEnemy);
    }

    /// <summary>
    /// Finds a point based on a random platform
    /// </summary>
    /// <returns>a random point</returns>
    private Vector2 GetRandomPoint()
    {
        Platform randomPlatform = this.platformGraph.GetRandomPlatform();
        return randomPlatform.GetValidPoint();
    }

    /// <summary>
    /// The actions to happen once an enemy is hit
    /// </summary>
    /// <param name="enemy">the enemy in question</param>
    private void OnEnemyHit(Enemy enemy)
    {
        this.EnemyHit?.Invoke();
        enemy.StateMachine.ForceChangeState(enemy.StateMachine.DeathState);
    }
}
