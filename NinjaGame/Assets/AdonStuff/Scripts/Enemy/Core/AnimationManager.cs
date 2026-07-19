using UnityEngine;

/// <summary>
/// Houses all of the necessary parameters apart of the parameter
/// </summary>
public class EnemyAnimationManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the animator the enemy has
    /// </summary>
    [SerializeField] private Animator animator;

    /// <summary>
    /// Whether or not the enemy is idle
    /// </summary>
    private bool isIdle;

    /// <summary>
    /// Whether or not the enemy is walking
    /// </summary>
    private bool isWalking;

    /// <summary>
    /// Whether or not the enemy is jumping or dropping
    /// </summary>
    private bool isJumpingDropping;

    /// <summary>
    /// Whether or not the enemy is dying
    /// </summary>
    private bool isDying;

    /// <summary>
    /// Whether or not the enemy is punching
    /// </summary>
    private bool isPunching;


    /// <summary>
    /// Sets whether the enemy is idle
    /// </summary>
    public bool IsIdle
    {
        set
        {
            this.isIdle = value;
        }
    }

    /// <summary>
    /// Sets whether the enemy is walking
    /// </summary>
    public bool IsWalking
    {
        set
        {
            this.isWalking = value;
        }
    }

    /// <summary>
    /// Gets and sets whether the enemy is jumping or dropping
    /// </summary>
    public bool IsJumpingDropping
    {
        get
        {
            return this.isJumpingDropping;
        }
        set
        {
            this.isJumpingDropping = value;
        }
    }

    /// <summary>
    /// Gets and sets whether the enemy is dying
    /// </summary>
    public bool IsDying
    {
        get
        {
            return this.isDying;
        }
        set
        {
            this.isDying = value;
        }
    }

    /// <summary>
    /// Gets and sets whether the enemy is punching something or not
    /// </summary>
    public bool IsPunching
    {
        get
        {
            return this.isPunching;
        }
        set
        {
            this.isPunching = value;
        }
    }

    /// <summary>
    /// Sets each of the bools to these respective fields every frame
    /// </summary>
    public void Update()
    {
        this.animator.SetBool("isIdling", this.isIdle);
        this.animator.SetBool("isWalking", this.isWalking);
        this.animator.SetBool("isJumpingDropping", this.isJumpingDropping);
        this.animator.SetBool("IsDying", this.isDying);
        this.animator.SetBool("isPunching", this.isPunching);
    }

    /// <summary>
    /// Resets each of the parameters to their factory reset places
    /// </summary>
    public void ResetAnimation()
    {
        this.animator.Rebind();
        this.animator.Update(0f);
    }

    /// <summary>
    /// Stops the current animation
    /// </summary>
    public void StopAnimation()
    {
        this.animator.StopPlayback();
    }
}
