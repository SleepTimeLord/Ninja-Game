using System;
using TMPro;
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

    /// <summary>
    /// The manager that holds all of the platforms
    /// </summary>
    [SerializeField] private PlatformGraph graph;

    [Header("Other Important Nodes")]
    /// <summary>
    /// A reference to the main player component
    /// </summary>
    [SerializeField] private SlashTrigger playerSlash;

    /// <summary>
    /// The result of the last frame for whether the player was hidden in the last frame
    /// </summary>
    private bool wasPlayerHiddenLastFrame;

    /// <summary>
    /// The score that the player has
    /// </summary>
    private int score;
    public TextMeshProUGUI scoreText;


    public void Start()
    {
        this.score = 0;
        this.scoreText.text = "Score: " + score;
        this.wasPlayerHiddenLastFrame = true;
        this.playerSlash.EnemyHit += OnEnemyHit;
    }

    public void Update()
    {
        if (this.trashcanContainer.isActiveAndEnabled && this.graph.isActiveAndEnabled)
        {
            bool isPlayerHidden = this.trashcanContainer.IsPlayerHidden;

            if (this.wasPlayerHiddenLastFrame != isPlayerHidden)
            {
                this.wasPlayerHiddenLastFrame = isPlayerHidden;
                Debug.Log("Switching");
                this.enemyManager.ChangeGlobalEnemyState(isPlayerHidden);
            }
        }
    }


    /// <summary>
    /// The actions to occur once an enemy gets hit
    /// </summary>
    private void OnEnemyHit(Enemy enemy)
    {
        this.score += 150;
        this.scoreText.text = "Score: " + score;
        Debug.Log($"Score increased to {this.score}");
    }
}
