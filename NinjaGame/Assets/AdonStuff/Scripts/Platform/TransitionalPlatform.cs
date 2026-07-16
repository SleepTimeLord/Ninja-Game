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
    /// Returns a sharable copy of all of the connected copies of the platform's the 
    /// TransitionalPlatform is connected to
    /// </summary>
    public IReadOnlyList<Platform> ConnectedPlatforms
    {
        get
        {
            return this.connectedPlatforms;
        }
    }


    /// <summary>
    /// Acts as a failsafe for the developers in case anything messes up
    /// </summary>
    public void Awake()
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
}
