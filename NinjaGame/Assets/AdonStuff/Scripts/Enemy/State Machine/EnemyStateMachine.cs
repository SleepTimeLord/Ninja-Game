using System.Collections;
using UnityEditor.ShaderKeywordFilter;
using UnityEditor.UI;
using UnityEngine;

/// <summary>
/// The manager of an enemy to handle everything surrounding it
/// </summary>
public class EnemyStateMachine : MonoBehaviour
{
    /// <summary>
    /// The enemy the state machine represents
    /// </summary>
    [SerializeField] private Enemy enemy;

    /// <summary>
    /// A reference to the active state an enemy is in
    /// </summary>
    private EnemyState currentState;

    /// <summary>
    /// A reference to a chase state
    /// </summary>
    private ChaseState chase;

    /// <summary>
    /// A reference to a wander state
    /// </summary>
    private WanderState wander;

    
    /// <summary>
    /// The actions that occur in the initialization of an enemy
    /// </summary>
    /// <remarks>Always assumes that the initial state will be the wander state</remarks>
    public void Start()
    {
        // Initialize the states first
        this.chase = new ChaseState(this, this.enemy);
        this.wander = new WanderState(this, this.enemy);

        // Then, wait for everything to be ready
        StartCoroutine(WaitUntilReady());
    }

    // Update is called once per frame
    public void Update()
    {
        if (this.currentState != null)
        {
            this.currentState.Update();
        }
    }


    /// <summary>
    /// The actions taken to switch to a new state
    /// </summary>
    /// <param name="toWhichState">the state to be switched into</param>
    public void ChangeState(EnemyState toWhichState)
    {
        this.currentState?.Exit();
        this.currentState = toWhichState;
        this.currentState.Enter();
    }

    /// <summary>
    /// Waits until the enemy is actually ready and everything is truly loaded to start itself
    /// </summary>
    /// <returns>Nothing</returns>
    private IEnumerator WaitUntilReady()
    {
        while (this.enemy.CurrentPlatform == null)
        {
            yield return null;
        }

        ChangeState(this.wander);
    }
}
