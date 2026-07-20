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
    /// The score that the player has
    /// </summary>
    private int score;


    public void Start()
    {
        this.score = 0;
        this.enemyManager.EnemyHit += OnEnemyHit;
    }


    /// <summary>
    /// The actions to occur once an enemy gets hit
    /// </summary>
    private void OnEnemyHit()
    {
        this.score += 150;
        Debug.Log($"Score increased to {this.score}");
    }
}
