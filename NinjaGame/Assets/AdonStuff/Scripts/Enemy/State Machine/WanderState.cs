using NUnit.Framework;
using UnityEngine;

/// <summary>
/// The state where the enemy isn't aware of the Player
/// </summary>
public class WanderState : EnemyState
{
    /// <summary>
    /// A reference to the navigator that the enemies all share
    /// </summary>
    private PlatformNavigator navigator;

    /// <summary>
    /// A reference to the graph that the enemies all share
    /// </summary>
    private PlatformGraph graph;

    /// <summary>
    /// Whether or not the enemy is wandering
    /// </summary>
    private bool isWandering;

    /// <summary>
    /// The approximate time it takes, in seconds, before the enemy tries to find another place
    /// to wander to again
    /// </summary>
    private const float TimeBetweenPathCreationMedian = 5f;

    public WanderState(EnemyStateMachine stateMachine) 
        : base(stateMachine)
    {
    }


    public override void CheckForTransition()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Makes the enemy start to wander to a random place
    /// </summary>
    public override void Enter()
    {
        this.isWandering = false;

        // The only thing that this state is doing initially is finding a random spot to go to
        GetRandomPath();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Get a path to a random point 
    /// </summary>
    private void GetRandomPath()
    {
        Platform platformDestination = this.graph.GetRandomPlatform();
        Vector2 pointDestination = platformDestination.GetValidPoint();

        //List<Platform> path = this.navigator.SearchPath();
    }
}
