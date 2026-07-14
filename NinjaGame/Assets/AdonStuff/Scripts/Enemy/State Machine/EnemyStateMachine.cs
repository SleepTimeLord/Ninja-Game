using UnityEditor.UI;
using UnityEngine;

/// <summary>
/// The manager of an enemy to handle everything surrounding it
/// </summary>
public class EnemyStateMachine : MonoBehaviour
{
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
        this.chase = new ChaseState(this);
        this.wander = new WanderState(this);

        ChangeState(wander);
    }

    // Update is called once per frame
    public void Update()
    {
        this.currentState.Update();
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
}
