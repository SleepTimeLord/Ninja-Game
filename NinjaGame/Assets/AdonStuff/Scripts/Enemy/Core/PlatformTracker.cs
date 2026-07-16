using UnityEngine;

/// <summary>
/// Manages all of the tracking with platforms
/// </summary>
public class PlatformTracker : MonoBehaviour
{
    /// <summary>
    /// The platform that the enemy is currently at
    /// </summary>
    private Platform currentPlatform;


    /// <summary>
    /// The platform that the enemy is currently at
    /// </summary>
    public Platform CurrentPlatform
    {
        get
        {
            return this.currentPlatform;
        }
    }


    /// <summary>
    /// Checks whether a new platform has been walked on and updates it if so
    /// </summary>
    /// <param name="collision">the object that the enemy collided on</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Platform platform = collision.gameObject.GetComponentInParent<Platform>();

        if (platform != null)
        {
            this.currentPlatform = platform;
        }
    }
}
