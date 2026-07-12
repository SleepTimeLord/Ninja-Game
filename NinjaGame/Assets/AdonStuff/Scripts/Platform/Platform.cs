using UnityEditor.UI;
using UnityEngine;

/// <summary>
/// The base class for what makes a platform a platform
/// </summary>
public class Platform : MonoBehaviour
{
    /// <summary>
    /// The boundaries of the platform
    /// </summary>
    /// <remarks>This is primarily used to ensure that there's a reference to the min, max, and 
    /// center of the platform</remarks>
    private Bounds bounds;

    /// <summary>
    /// Whether or not the platform has transtional properites; as in, it can change from
    /// one platform to another
    /// </summary>
    /// <remarks>To create a platform with a transitional component, you have to just simply add
    /// the component to the scene, and add the respective platforms to be linked to the current 
    /// one</remarks>
    private bool containsTransition;


    /// <summary>
    /// Returns whether a platform contains transitional properties
    /// </summary>
    /// <remarks>Used in the searching algorithm to get from one location to another</remarks>
    public bool ContainsTransition
    {
        get
        {
            return this.containsTransition;
        }
    }


    /// <summary>
    /// Basically a miniature constructor of the platform
    /// </summary>
    public void Start()
    {
        this.bounds = this.GetComponent<Collider2D>().bounds;
        this.containsTransition = CheckIfTransitionalPlatform();

        // Made for debugging purposes. Will remove in post
        if (containsTransition)
        {
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();

            if (renderer != null)
                renderer.color = Color.blue;
        }
    }

    /// <summary>
    /// Finds a valid point that's within the bounds of the platform
    /// </summary>
    /// <returns>a random point that's on the platform</returns>
    /// <remarks>used when an enemy is finding a place to pathfind to</remarks>
    public Vector2 GetValidPoint()
    {
        float randomX = Random.Range(
            this.bounds.min.x,
            this.bounds.max.x);
        float randomY = this.bounds.max.y;

        return new Vector2(randomX, randomY);
    }

    /// <summary>
    /// Checks if there is any additional transitional platform on the prefab 
    /// </summary>
    /// <returns>whether there is a transitional platform component</returns>
    private bool CheckIfTransitionalPlatform()
    {
        return this.GetComponent<TransitionalPlatform>() != null;
    }
}
