using UnityEngine;

/// <summary>
/// An object the player can hide in to move around enemies
/// </summary>
public class Trashcan : MonoBehaviour
{
    [Header("Enemy Interaction Reference")]
    ///<summary>
    ///A reference to the interactable wrapper class
    /// </summary>
    [SerializeField] private EnemyInteractable interactable;


    /// <summary>
    /// Returns whether or not the player is inside the trashcan
    /// </summary>
    public bool IsPlayerInside
    {
        get
        {
            return this.interactable.IsPlayerInside;
        }
    }


    /// <summary>
    /// The actions to occur once the player has interacted with the trashcan
    /// </summary>
    public void SetPlayerInside()
    {
        this.interactable.ChangePlayerStatus();
    }
}
