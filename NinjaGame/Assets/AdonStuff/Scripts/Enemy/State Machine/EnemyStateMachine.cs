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
    /// The state that was requested to be switched into
    /// </summary>
    /// <remarks>only used in special cases when an animation needs to be complete</remarks>
    private EnemyState pendingState;

    /// <summary>
    /// A reference to a chase state
    /// </summary>
    private ChaseState chase;

    /// <summary>
    /// A reference to a wander state
    /// </summary>
    private WanderState wander;


    /// <summary>
    /// Returns a reference to the wander state
    /// </summary>
    public WanderState WanderState
    {
        get
        {
            return this.wander;
        }
    }

    /// <summary>
    /// Returns a reference to the chase state
    /// </summary>
    public ChaseState ChaseState
    {
        get
        {
            return this.chase;
        }
    }

    
    /// <summary>
    /// The actions that occur in the initialization of an enemy
    /// </summary>
    public void Awake()
    {
        this.chase = new ChaseState(this.enemy);
        this.wander = new WanderState(this.enemy);
    }

    // Update is called once per frame
    public void Update()
    {
        TryCompleteTransition();

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
        // Ensures that, if anything particular to a state is occurring, things don't just exit
        if (this.currentState == null || (this.currentState != null && this.currentState.CanExit))
        {
            this.currentState?.Exit();
            this.currentState = toWhichState;
            this.currentState.Enter();
        }
        else
        {
            this.pendingState = toWhichState;
        }
    }

    /// <summary>
    /// Sets all the states to nothing, ensuring that nothing can happen
    /// </summary>
    public void CancelStates()
    {
        this.currentState = null;
        this.pendingState = null;
    }

    /// <summary>
    /// Tries to complete a transition, if there is one
    /// </summary>
    private void TryCompleteTransition()
    {
        /*
         * Because this is called every frame, there are times when it really doesn't need to be
         * called. This makes sure that that's considered
         */
        if (this.pendingState == null || !this.currentState.CanExit)
        {
            return;
        }

        Debug.Log("Completing transition");
        this.currentState.Exit();
        this.currentState = this.pendingState;
        this.pendingState = null;
        this.currentState.Enter();
    }
}
