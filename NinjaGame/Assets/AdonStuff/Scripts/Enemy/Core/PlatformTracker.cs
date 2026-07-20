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
    /// When a platform is needed to be found immediately, this is a way for it to do so
    /// </summary>
    public void FindPlatformBelow()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, Vector2.down, 2f);

        foreach (RaycastHit2D hit in hits)
        {
            Platform platform = hit.collider.GetComponent<Platform>();

            if (platform != null)
            {
                this.currentPlatform = platform;
                return;
            }
        }
    }


    /// <summary>
    /// Checks whether a new platform has been walked on and updates it if so
    /// </summary>
    /// <param name="collision">the object that the enemy collided on</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Platform platform = collision.gameObject.GetComponent<Platform>();

        if (platform != null)
        {
            this.currentPlatform = platform;
        }
    }
}
