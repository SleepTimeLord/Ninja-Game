using UnityEngine;

/// <summary>
/// A description of the exact transition that should occur while creating and executing a path
/// </summary>
public class PlatformTransition
{
    /// <summary>
    /// The exact kind of transition from the current platform to the next that needs to be taken
    /// </summary>
    public enum TransitionType
    {
        WALK,
        JUMP,
        DROP
    }

    /// <summary>
    /// Used during searching to designate which platform the current one is coming from
    /// </summary>
    private Platform sourcePlatform;

    /// <summary>
    /// The exact platform that's being attempted to go to
    /// </summary>
    private Platform destinationPlatform;

    /// <summary>
    /// The position within the platform to go to
    /// </summary>
    private Vector2 targetPosition;

    /// <summary>
    /// The kind of transition to take
    /// </summary>
    private TransitionType transition;


    public Platform SourcePlatform
    {
        get
        {
            return this.sourcePlatform;
        }
    }

    public Platform DestinationPlatform
    {
        get
        {
            return this.destinationPlatform;
        }
    }

    public Vector2 TargetPosition
    {
        get
        {
            return this.targetPosition;
        }
    }

    public TransitionType Transition
    {
        get
        {
            return this.transition;
        }
    }


    /// <summary>
    /// The creation of a proper "step" in the navigation process
    /// </summary>
    /// <param name="sourcePlatform">The platform that comes before the current platform.</param>
    /// <param name="destinationPlatform">the platform that's being attempted to go to</param>
    /// <param name="targetPosition">the position within the platform to go to</param>
    /// <param name="transition">the exact kind of transition to take</param>
    public PlatformTransition(Platform sourcePlatform, Platform destinationPlatform, 
        Vector2 targetPosition, TransitionType transition)
    {
        this.sourcePlatform = sourcePlatform;
        this.destinationPlatform = destinationPlatform;
        this.targetPosition = targetPosition;
        this.transition = transition;
    }
}
