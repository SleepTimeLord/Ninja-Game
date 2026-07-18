using UnityEngine;

/// <summary>
/// The central hub for all of the managers and the center for their actions
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    /// <summary>
    /// The manager that holds all of the enemies
    /// </summary>
    [SerializeField] private EnemyManager enemyManager;

    /// <summary>
    /// The manager that holds all of the trashcans
    /// </summary>
    [SerializeField] private TrashcanContainer trashcanContainer;

    [Header("Other Important Nodes")]
    /// <summary>
    /// A reference to the main player component
    /// </summary>
    [SerializeField] private NinjaRoot player;

    /// <summary>
    /// An extra reference to the slash the player has
    /// </summary>
    [SerializeField] private SlashTrigger playerSlash;

    /// <summary>
    /// The score that the player has
    /// </summary>
    private int score;


    public void Start()
    {
        this.score = 0;
        this.playerSlash.EnemyHit += OnEnemyHit;
    }


    /// <summary>
    /// The actions to occur once an enemy gets hit
    /// </summary>
    /// <param name="enemy">the enemy in question getting hit</param>
    /// <remarks>Although this isn't ideal for the GameManager to have this as well, it's useful
    /// for also increasing score</remarks>
    private void OnEnemyHit(Enemy enemy)
    {
        this.score += 150;
    }
}
