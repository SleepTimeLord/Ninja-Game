using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A component of the enemy to handle movement
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("Physics Components")]
    /// <summary>
    /// A reference to the Rigidbody2D the enemy has
    /// </summary>
    [SerializeField] Rigidbody2D rb;

    [Header("Necessary nodes")]
    ///<summary>
    /// The enemy the movement is assisting with
    /// </summary>
    [SerializeField] Enemy enemy;

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
    /// Returns whether the enemy is actively pursuing a path
    /// </summary>
    public bool HasPath
    {
        get
        {
            return this.currentPath.Count > 0 ||
               this.currentPathPlatformStep != null ||
               this.finalDestination != Vector2.zero;
        }
    }


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
    /// Called every frame to handle movement with the enemy
    /// </summary>
    /// <param name="speed">the speed the enemy needs to go</param>
    /// <param name="currentPlatform">The platform the enemy is on</param>
    public void UpdateMovement(float speed, Platform currentPlatform)
    {
        // How lenient the enemy has to be away from the target before it's considered complete
        const float TargetLeniency = 0.5f;

        Vector2 currentPosition = this.transform.position;

        // First, check whether or not movement is necessarily needed
        if (!HasPath)
        {
            Debug.Log("There's no path, meaning that there's nothing more to do at the moment");
            return;
        }

        /*
         * We know that there is a path at this point, so it means we can now check whether we have
         * an active step the enemy is taking
         */ 
        if (this.currentPathPlatformStep != null)
        {
            /*
             * The enemy's target after considering the enemy's height without considering drops or
             * jumps
             */
            Vector2 baseEnemyTarget = GetTargetPosition(
                this.currentPathPlatformStep.TargetPosition);

            /*
             * When there is a path left, it means we need to actually consider what exact 
             * transition is going on
             */
            switch (this.currentPathPlatformStep.Transition)
            {
                /*
                 * Walking is as simple as the last conditional-it just needs to have its walking
                 * updated if the enemy isn't already on the platform it needs to be on
                 */
                case PlatformTransition.TransitionType.WALK:
                    Debug.Log($"Walking to {baseEnemyTarget}");
                    this.UpdateWalk(baseEnemyTarget, speed);

                    if (EnemyCloseEnoughToTarget(TargetLeniency, baseEnemyTarget, currentPosition))
                    {
                        break;
                    }
                    return;
                /*
                 * Jumping and dropping are fairly similar. We need to check whether or not they're
                 * in the middle of their transition, and if so, update it. It's also worth 
                 * checking for whether or not the transition is complete, to which settings need
                 * to be reset
                 */
                case PlatformTransition.TransitionType.JUMP:
                case PlatformTransition.TransitionType.DROP:
                    // A jump will have a more dramatic parabola, with a greater height
                    float peakHeight = this.currentPathPlatformStep.Transition ==
                        PlatformTransition.TransitionType.DROP ? 3f : 1f;
                    Vector2 newTransitionPosition = GetTargetPosition(
                        this.currentPathPlatformStep.TargetPosition, true);

                    // Check for time BEFORE iterating it to ensure correct values
                    if (this.transitionProgress >= 1)
                    {
                        EndTransition();
                        this.transitionProgress = 0f;
                        break;
                    }
                    else if (this.transitionProgress == 0)
                    {
                        StartTransition(currentPosition);
                    }

                    // Then update the transition's progress
                    this.transitionProgress += Time.deltaTime / TransitionDuration;

                    Vector2 newDropPosition = UpdateArc(this.specialTransitionStartPoint,
                        newTransitionPosition, this.transitionProgress, peakHeight);

                    Debug.Log($"Jumping/dropping to {newDropPosition}");
                    this.transform.position = newDropPosition;
                    return;
            }
        }

        /*
         * At this point, a new transition needs to be assigned because the last one finished. 
         * If there isn't any more transitions left, it means the path has ended, and things can be reset
         */
        if (!this.currentPath.TryDequeue(out this.currentPathPlatformStep))
        {
            this.ClearPathParams();
        }
        else
        {
            Debug.Log("Finished previous state");
        }
    }

    /// <summary>
    /// Resets each path parameter to be blank, ready for another path
    /// </summary>
    private void ClearPathParams()
    {
        this.currentPathPlatformStep = null;
        this.finalDestination = Vector2.zero;
        this.transitionProgress = -0f;
        this.specialTransitionStartPoint = Vector2.zero;
    }

    /// <summary>
    /// Allows the enemy to be able to be fully manipulated by the jump physics
    /// </summary>
    /// <param name="currentPosition">The position the enemy starts in</param>
    private void StartTransition(Vector2 currentPosition)
    {
        this.specialTransitionStartPoint = currentPosition;
        this.rb.gravityScale = 0;
        this.enemy.SpriteCollider.enabled = false;
    }

    /// <summary>
    /// Allows the enemy to feel the effects of gravity and collide with other objects
    /// </summary>
    private void EndTransition()
    {
        this.rb.gravityScale = 1;
        this.enemy.SpriteCollider.enabled = true;
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

    /// <summary>
    /// Calculates the position a platform should consider the target based on its position
    /// </summary>
    /// <param name="basedOnWhatPos">the position the platform is on</param>
    /// <param name="shouldConsiderJumpHeight">whether or not there should be extra room for the 
    /// enemy to jump all the way up above the platform</param>
    /// <returns>the position the enemy should expect to wait for</returns>
    private Vector2 GetTargetPosition(Vector2 basedOnWhatPos, 
        bool shouldConsiderJumpHeight = false)
    {
        float extendsScale = shouldConsiderJumpHeight ? 2 : 1;

        return basedOnWhatPos +
            (Vector2.up * this.enemy.SpriteCollider.bounds.extents.y) * extendsScale;
    }

    /// <summary>
    /// Finds out whether the enemy is close enough to the target to move onto the next 
    /// </summary>
    /// <param name="targetLeniency">how close the enemy should be to the target</param>
    /// <param name="targetPosition">the current position of the target</param>
    /// <param name="currentPosition">the current position of the enemy</param>
    /// <returns>whether or not the enemy is close enough to the target</returns>
    private bool EnemyCloseEnoughToTarget(float targetLeniency, Vector2 targetPosition, 
        Vector2 currentPosition)
    {
        return Vector2.Distance(currentPosition, targetPosition) <= targetLeniency;
    }
}
