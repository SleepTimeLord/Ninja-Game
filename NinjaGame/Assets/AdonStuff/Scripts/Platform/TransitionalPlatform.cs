using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An extra component of a platform that allows transportation between platforms
/// </summary>
/// <remarks>This is a wrapper class, meaning that there really isn't much information in it,
/// and is really just used for extraction</remarks>
public class TransitionalPlatform : MonoBehaviour
{
    /// <summary>
    /// The platforms that the transitional platform connects to
    /// </summary>
    /// <remarks>The connected platform should not have one to the same platform that 
    /// it's being referred to</remarks>
    [SerializeField] private List<Platform> connectedPlatforms;


    /// <summary>
    /// Acts as a failsafe for the developers in case anything messes up
    /// </summary>
    public void Start()
    {
        /*
         * This platform's connected platforms shouldn't contain the platform itself,
         * so it needs to be tackled as soon as possible
         */
        Platform currentPlatform = this.GetComponent<Platform>();

        if (this.connectedPlatforms.Contains(currentPlatform))
        {
            Debug.LogWarning($"Platform {name} contains a connection to itself. Please remove.");
        }
    }

    /// <summary>
    /// Checks whether the current platform connects to another 
    /// </summary>
    /// <param name="otherPlatform">the other platform to get to</param>
    /// <returns>whether the current platform connects to otherPlatform/returns>
    public bool CanReach(Platform otherPlatform)
    {
        return this.connectedPlatforms.Contains(otherPlatform);
    }
}
