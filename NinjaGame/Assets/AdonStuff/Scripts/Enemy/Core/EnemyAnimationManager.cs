using System;
using UnityEngine;

/// <summary>
/// Houses all of the necessary parameters apart of the parameter
/// </summary>
public class EnemyAnimationManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the animator the enemy contains
    /// </summary>
    [SerializeField] private Animator animator;

    /// <summary>
    /// A reference to the enemy the animation manager controls
    /// </summary>
    [SerializeField] private Enemy enemy;

    /// <summary>
    /// A collection of all of the animations with their names in the Animator
    /// </summary>
    [Header("Animation Names")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walk";
    [SerializeField] private string jumpDropAnimation = "JumpingDropping";
    [SerializeField] private string punchAnimation = "Punch";
    [SerializeField] private string deathAnimation = "Death";

    #region Play Methods
    public void PlayIdle()
    {
        animator.Play(idleAnimation);
    }

    public void PlayWalk()
    {
        animator.Play(walkAnimation);
    }

    public void PlayJumpDrop()
    {
        animator.Play(jumpDropAnimation);
    }

    public void PlayPunch()
    {
        animator.Play(punchAnimation);
    }

    public void PlayDeath()
    {
        animator.Play(deathAnimation);
    }
    #endregion

    /// <summary>
    /// The actions to occur once the punch has finished
    /// </summary>
    public void FinishPunch()
    {
        if (this.enemy.CurrentState is PunchState confirmedState)
        {
            confirmedState.OnAnimationFinished();
        }
    }

    /// <summary>
    /// The actions to occur once the death animation is complete
    /// </summary>
    public void FinishDeath()
    {
        if (this.enemy.CurrentState is DeathState confirmedState)
        {
            confirmedState.OnAnimationComplete();
        }
    }
}
