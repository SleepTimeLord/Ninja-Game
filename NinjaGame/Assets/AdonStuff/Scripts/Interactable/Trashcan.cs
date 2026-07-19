using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// An object the player can hide in to move around enemies
/// </summary>
public class Trashcan : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlatformTracker tracker;

    /// <summary>
    /// Whether or not the player is inside the trashcan
    /// </summary>
    private bool isPlayerInside;


    /// <summary>
    /// Returns whether or not the player is inside the trashcan
    /// </summary>
    public bool IsPlayerInside
    {
        get
        {
            return this.IsPlayerInside;
        }
    }
    
    /// <summary>
    /// Returns a reference to the platform the trashcan is on
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
    public Vector2 Position
    {
        get
        {
            return this.transform.position;
        }
    }


    /// <summary>
    /// Immediately checks whether the player is inside of the trashcan
    /// </summary>
    public void Awake()
    {
        PlayerCheck();
        this.tracker.FindPlatformBelow();
    }


    /// <summary>
    /// The actions to check whether or not the player is inside the trashcan
    /// </summary>
    public void PlayerCheck()
    {
        foreach (Transform child in this.transform)
        {
            if (child.CompareTag("Player"))
            {
                this.isPlayerInside = true;
                return;
            }
        }

        this.isPlayerInside = false;
    }
}
