using UnityEngine;

/// <summary>
/// Contains and manages each of the active trashcans in-game
/// </summary>
public class TrashcanContainer : MonoBehaviour
{
    /// <summary>
    /// A reference to all of the trashcans
    /// </summary>
    private Trashcan[] trashcans;


    /// <summary>
    /// Sets each of the trashcans that are active in the scene
    /// </summary>
    public void Start()
    {
        this.trashcans = GetComponentsInChildren<Trashcan>();
        Debug.Log(this.trashcans.Length);
    }


    public void Update()
    {
        foreach (Trashcan trashcan in this.trashcans)
        {
            trashcan.PlayerCheck();
        }
    }

    /// <summary>
    /// Finds a return trashcan
    /// </summary>
    /// <returns>a randomly-found trashcan</returns>
    public Trashcan GetRandomTrashcan()
    {
        int randomIndex = Random.Range(0, this.trashcans.Length);

        return this.trashcans[randomIndex];
    }
}