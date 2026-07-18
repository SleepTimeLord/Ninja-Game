using System;
using UnityEngine;

/// <summary>
/// A wrapper class containing information for things the enemy can interact with
/// </summary>
public class EnemyInteractable : MonoBehaviour
{
    [Header("Additional Nodes")]
    ///<summary>
    /// A reference to the tracker the can has
    /// </summary>
    [SerializeField] private PlatformTracker tracker;

    /// <summary>
    /// A reference to the transform the sprite has
    /// </summary>
    [SerializeField] private Transform spriteTransform;

    /// <summary>
    /// Whether the player is inside the trashcan
    /// </summary>
    private bool isPlayerInside;


    /// <summary>
    /// The platform the trash can is on
    /// </summary>
    public Platform CurrentPlatform
    {
        get
        {
            return this.tracker.CurrentPlatform;
        }
    }

    /// <summary>
    /// Returns the position of the trashcan
    /// </summary>
    public Vector2 SpritePosition
    {
        get
        {
            return this.spriteTransform.position;
        }
    }

    /// <summary>
    /// Returns and sets whether the player was inside
    /// </summary>
    public bool IsPlayerInside
    {
        get
        {
            return this.isPlayerInside;
        }
    }


    /// <summary>
    /// Swaps the player status to whatever it needs to be set to
    /// </summary>
    public void ChangePlayerStatus()
    {
        this.isPlayerInside = !this.IsPlayerInside;
    }
}
