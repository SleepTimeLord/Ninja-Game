using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The manager handling the logic of punching the player
/// </summary>
public class EnemyAttackManager : MonoBehaviour
{
    /// <summary>
    /// The collider that the enemy will use to detect the player
    /// </summary>
    [SerializeField] private Collider2D punchBox;

    private CharacterController controller;


    public CharacterController Controller
    {
        set
        {
            this.controller = value;
        }
    }

    /// <summary>
    /// Returns whether the punchbox is actually active
    /// </summary>
    public bool IsPunchboxActive
    {
        get
        {
            return this.punchBox.enabled;
        }
    }


    /// <summary>
    /// Enables the hitbox to allow the effect of the box to actually occur
    /// </summary>
    public void EnableHitbox()
    {
        this.punchBox.enabled = true;
    }

    /// <summary>
    /// Disables the hitbox for when the enemy isn't punching
    /// </summary>
    public void DisableHitbox()
    {
        this.punchBox.enabled = false;
    }
    
    /// <summary>
    /// Allows the player to actually get hit
    /// </summary>
    /// <param name="collision">the collision the punch hit</param>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Hit {collision.name}");

        Debug.Log("Before component search");

        var player =
            collision.GetComponentInChildren<CharacterController>();

        Debug.Log("After component search");

        Debug.Log(player == null);

        if (collision.gameObject.CompareTag("Player"))
        {
            this.controller.TakeDamage(10, this.punchBox.transform.position);
        }
    }
}
