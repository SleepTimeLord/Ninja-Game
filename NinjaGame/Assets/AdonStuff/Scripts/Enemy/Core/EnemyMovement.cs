using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A component of the enemy to handle movement
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    /// <summary>
    /// The path that the enemy needs to take to get to its destination from its current point
    /// </summary>
    private Queue<PlatformTransition> currentPath;

    /// <summary>
    /// The exact point the enemy needs to go to
    /// </summary>
    private Vector2 finalDestination;

    /// <summary>
    /// The start point for where the movement arc gets created
    /// </summary>
    private Vector2 specialTransitionStartPoint;

    /// <summary>
    /// The current step that the 
    /// </summary>
    private PlatformTransition currentPathPlatformStep;

    /// <summary>
    /// The progress, in seconds, of how long before the transition for jumping/dropping ends
    /// </summary>
    /// <remarks>This will be used in the calculation for the arc. It should never exceed 1.</remarks>
    private float transitionProgress;

    /// <summary>
    /// How long a transition will last
    /// </summary>
    /// <remarks>This could be the length of an animation</remarks>
    private const float TransitionDuration = 1f;


    /// <summary>
    /// Called by the state machine to set a particular path 
    /// </summary>
    /// <param name="path">the path that the enemy is taking</param>
    /// <param name="finalDestination">a smash reference</param>
    public void SetPath(List<PlatformTransition> path, Vector2 finalDestination)
    {
        this.currentPath = new Queue<PlatformTransition>(path);
        this.finalDestination = finalDestination;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speed"></param>
    public void UpdateMovement(float speed, Platform currentPlatform)
    {
        // How lenient the enemy has to be away from the target before it's considered complete
        const float TargetLeniency = 0.1f;
        Vector2 currentPosition = this.transform.position;

        // First, check whether or not movement is necessarily needed
        if (this.finalDestination == Vector2.zero)
        {
            return;
        }
        // Next, check whether or not the movement has been complete
        else if (Vector2.Distance(currentPosition, this.finalDestination) < TargetLeniency || 
            this.transitionProgress >= 1f)
        {
            ClearPathParams();
            return;
        }
        /*
         * Next, check whether or not it's necessary to just move to the exact position, which
         * occurs only when the platform has been met
         */
        else if (currentPlatform == this.currentPathPlatformStep.DestinationPlatform)
        {
            UpdateWalk(this.finalDestination, speed);
            return;
        }
        // Where the real meat starts to come in. Movement now needs to be considered exactly
        else
        {
            /*
             * To make sure we actually have a target to go to. If it's not just simply a walk,
             * then it needs to have its transitionProgress reset
             */
            if (this.currentPathPlatformStep == null)
            {
                this.currentPathPlatformStep = this.currentPath.Dequeue();

                if (!(this.currentPathPlatformStep.Transition is
                    PlatformTransition.TransitionType.WALK))
                {
                    /*
                     * The start-point needs to be defined here, as it's the true start-time
                     * for the special transition
                     */
                    this.specialTransitionStartPoint = this.transform.position;
                }
            }

            // Now, what happens next is entirely dependent on the movement
            switch (this.currentPathPlatformStep.Transition)
            {
                case PlatformTransition.TransitionType.WALK:
                    UpdateWalk(this.currentPathPlatformStep.TargetPosition, speed);
                    break;
                case PlatformTransition.TransitionType.JUMP:
                case PlatformTransition.TransitionType.DROP:
                    this.transitionProgress += Time.deltaTime / TransitionDuration;

                    // AKA the vertex of the parabola
                    float peakHeight = this.currentPathPlatformStep.Transition == 
                        PlatformTransition.TransitionType.DROP ? 3f : 1f;

                    Vector2 newDropPosition = UpdateArc(this.transform.position,
                        this.currentPathPlatformStep.TargetPosition, this.transitionProgress, 
                        peakHeight);

                    this.transform.position = newDropPosition;
                    break;
            }
        }
    }

    /// <summary>
    /// Resets each path parameter to be blank, ready for another path
    /// </summary>
    private void ClearPathParams()
    {
        this.currentPath.Clear();
        this.currentPathPlatformStep = null;
        this.finalDestination = Vector2.zero;
        this.transitionProgress = -1f;
        this.specialTransitionStartPoint = Vector2.zero;
    }

    /// <summary>
    /// The actions to occur when walking from one location to another
    /// </summary>
    /// <param name="target">the target position to get to</param>
    /// <param name="speed"></param>
    private void UpdateWalk(Vector2 target, float speed)
    {
        this.transform.position = Vector2.MoveTowards(
            this.transform.position, target, speed * Time.deltaTime);
    }

    /// <summary>
    /// Updates the arc that's created during a jump/drop
    /// </summary>
    /// <param name="start">the start position</param>
    /// <param name="end">the end position</param>
    /// <param name="time">over the span of (time)? in seconds</param>
    /// <param name="peakHeight">the peak height of the parabola created</param>
    /// <returns>the position the enemy during the arc based on the height</returns>
    private Vector2 UpdateArc(Vector2 start, Vector2 end, float time, float peakHeight)
    {
        // Actively changes the position each frame to a newer one. Moves linearly
        Vector2 position = Vector2.Lerp(start, end, time);

        // Actually adds the parabolic shape by basically giving it a curve
        position.y += Mathf.Sin(time * Mathf.PI) * peakHeight;

        return position;
    }
}
