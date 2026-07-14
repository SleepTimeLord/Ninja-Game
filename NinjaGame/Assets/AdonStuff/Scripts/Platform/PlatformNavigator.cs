using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A manager to handle any pathfinding enemies must do between one platform to another
/// </summary>
public class PlatformNavigator : MonoBehaviour
{
    [SerializeField] PlatformGraph graph;


    /// <summary>
    /// Conducts a BFS to get from point A to point B
    /// </summary>
    /// <param name="currentPlatform">point A, or the starting point</param>
    /// <param name="goalPlatform">point B, or the platform that </param>
    /// <returns>the path constructed that connects point A and point B</returns>
    public List<PlatformTransition> SearchPath(Platform currentPlatform, Platform goalPlatform)
    {
        Queue<Platform> platformQueue = new Queue<Platform>();
        HashSet<Platform> visitedPlatforms = new HashSet<Platform>();
        Dictionary<Platform, PlatformTransition> cameFromSet = 
            new Dictionary<Platform, PlatformTransition>();

        
        platformQueue.Enqueue(currentPlatform);
        visitedPlatforms.Add(currentPlatform);

        // This loop essentially goes through each node/platform until the goal platform is found.
        while (platformQueue.Count > 0)
        {
            Platform current = platformQueue.Dequeue();

            if (current == goalPlatform)
            {
                break;
            }

            // Visit each of the transitions, and jolt them down
            foreach (PlatformTransition transition in this.graph.GetTransitions(current))
            {
                Platform neighbor = transition.DestinationPlatform;

                if (visitedPlatforms.Contains(neighbor))
                {
                    continue;
                }

                visitedPlatforms.Add(neighbor);
                cameFromSet[neighbor] = transition;
                platformQueue.Enqueue(neighbor);
            }
        }

        // Once the search is done, the path can be made and returned
        return MakePath(cameFromSet, currentPlatform, goalPlatform);
    }

    /// <summary>
    /// Creates the path based on the BFS
    /// </summary>
    /// <param name="cameFromSet">a reference to each of the source and destination platforms
    /// based off the search</param>
    /// <param name="startPlatform">the platform that was started on</param>
    /// <param name="goalPlatform">the platform to end on</param>
    /// <returns>a completed path that goes to point A to point B</returns>
    private List<PlatformTransition> MakePath(
        Dictionary<Platform, PlatformTransition> cameFromSet, 
        Platform startPlatform, Platform goalPlatform)
    {
        List<PlatformTransition> path = new List<PlatformTransition>();

        // We're basically about to go from end to start, then reverse it
        Platform current = goalPlatform;

        /*
         * cameFrom is basically the finalized path that was gathered from the search. 
         * I'm basically doing a bunch of questioning of "where did you come from"
         * to each of the paths until we get the correct one
         */
        while (current != startPlatform)
        {
            PlatformTransition transition = cameFromSet[current];
            path.Add(transition);
            current = transition.SourcePlatform;
        }

        /*
         * The start is never added because the enemy's start point is always it's active
         * location
         */
        path.Reverse();

        return path;
    }
}
