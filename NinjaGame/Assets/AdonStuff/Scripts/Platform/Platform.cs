using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for what makes a platform a platform
/// </summary>
/// <remarks>To create a readable platform to the graph, this platform reference must be
/// under the Platforms GameObject</remarks>
public class Platform : MonoBehaviour
{
    /// <summary>
    /// The boundaries of the platform
    /// </summary>
    /// <remarks>This is primarily used to ensure that there's a reference to the min, max, and 
    /// center of the platform</remarks>
    private Bounds bounds;

    /// <summary>
    /// A reference to the TransitionalPlatform component, if a platform contains one
    /// </summary>
    private TransitionalPlatform transitionalComp;

    /// <summary>
    /// Returns the TransitionalPlatform reference
    /// </summary>
    public TransitionalPlatform TransitionalRef
    {
        get
        {
            return this.transitionalComp;
        }
    }

    /// <summary>
    /// Returns a reference to the bounds of the platform
    /// </summary>
    public Bounds Bounds
    {
        get
        {
            return this.bounds;
        }
    }


    /// <summary>
    /// Basically a miniature constructor of the platform
    /// </summary>
    public void Awake()
    { 
        // Always gotta check if we done messed up a bit early
        if (!IsPlacedCorrectly())
        {
            Debug.LogWarning($"Platform {name} is not in the correct spot in the hierarchy. " +
                $"It needs to be under the Platform GO. Pls and ty :pray:");
        }

        this.bounds = this.GetComponent<Collider2D>().bounds;
        this.transitionalComp = GetComponent<TransitionalPlatform>();

        // Made for debugging purposes. Will remove in post
        if (transitionalComp != null)
        {
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
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
    /// Checks whether the Platform is placed correctly in the scene
    /// </summary>
    /// <returns>whether or not the Platform is placed uner the correct GO</returns>
    /// <remarks>To ensure that the PlatformGraph can read the platform, ensure that this GO
    /// is marked under the Platforms GO :pray:</remarks>
    private bool IsPlacedCorrectly()
    {
        return this.GetComponentInParent<PlatformGraph>() != null;
    }
}
