using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PlatformTransition;

/// <summary>
/// A representation of each active platform in the map at once
/// </summary>
/// <remarks>This is a directed graph, meaning that children won't have references
/// to their parent, but vise versa. Each node will be a transitional platform while the leaves
/// or children are regular platforms.</remarks>
public class PlatformGraph : MonoBehaviour
{
    /// <summary>
    /// A reference to all the platforms currently in-game
    /// </summary>
    private Platform[] platforms;

    /// <summary>
    /// A reference to all of the adjacencies in the map
    /// </summary>
    private Dictionary<Platform, List<PlatformTransition>> adjacencyList;


    public void Start()
    {
        BuildGraph();
    }

    /// <summary>
    /// Gets the transitions of a platform
    /// </summary>
    /// <param name="basedOnWhichPlatform">the platform in question</param>
    /// <returns>the transitions that a platform contains</returns>
    public IEnumerable<PlatformTransition> GetTransitions(Platform basedOnWhichPlatform)
    {
        return this.adjacencyList[basedOnWhichPlatform];
    }

    /// <summary>
    /// Finds a random platform 
    /// </summary>
    /// <returns>a random platform</returns>
    public Platform GetRandomPlatform()
    {
        int randomIndex = Random.Range(0, this.platforms.Length);

        return this.platforms[randomIndex];
    }

    /// <summary>
    /// Creates a "Graph" by gathering references to all of the platforms
    /// </summary>
    /// <remarks>This isn't a traditional graph per se, due to the fact that the Transitional
    /// Platforms already cover their own adjacencies. Truly, this is just a wrapper class, and
    /// this is really all it needs to get started</remarks>
    private void BuildGraph()
    {
        this.platforms = FindObjectsByType<Platform>();
        this.adjacencyList = new Dictionary<Platform, List<PlatformTransition>>();

        GetAdjacencies();
        PrintGraph();
    }

    /// <summary>
    /// Finds all of the platforms that are adjacent to one another
    /// </summary>
    private void GetAdjacencies()
    {
        // The tolerance, in pixels, for what is considered an adjacent platform
        const float xTolerance = 1f;
        const float yTolerance = 1f;

        foreach (Platform current in this.platforms)
        {
            // Before going into the next loop, that one can be checked and added
            if (current.TransitionalRef != null)
            {
                AddTransitionalConnections(current);
            }
            
            foreach (Platform potentialNeighbor in this.platforms)
            {
                if (current == potentialNeighbor)
                {
                    continue;
                }

                Bounds currentBounds = current.Bounds;
                Bounds neighborBounds = potentialNeighbor.Bounds;
                bool isLevel = Mathf.Abs(
                    currentBounds.max.y - neighborBounds.max.y) <= yTolerance;
                bool isLeftAdjacent = Mathf.Abs(
                    currentBounds.min.x - neighborBounds.max.x) <= xTolerance;
                bool isRightAdjacent = Mathf.Abs(
                    currentBounds.max.x - neighborBounds.min.x) <= xTolerance;

                if (isLevel && (isLeftAdjacent || isRightAdjacent))
                {
                    if (!this.adjacencyList.ContainsKey(current))
                    {
                        this.adjacencyList[current] = new List<PlatformTransition>();
                    }

                    PlatformTransition transition = new PlatformTransition(current,
                        potentialNeighbor, potentialNeighbor.TopCenter, TransitionType.WALK);

                    this.adjacencyList[current].Add(transition);
                }
            }
        }
    }

    /// <summary>
    /// Takes the connections the TransitionalPlatform has and converts them into TransitionTypes
    /// </summary>
    /// <param name="fromWhichPlatform">the platform in question</param>
    private void AddTransitionalConnections(Platform fromWhichPlatform)
    {
        // For easy referencing
        TransitionalPlatform transitionalPlatformRef = fromWhichPlatform.TransitionalRef;
        
        // Ensure that there is actually an entry in the dictionary for the platform in question
        if (!this.adjacencyList.ContainsKey(fromWhichPlatform))
        {
            this.adjacencyList[fromWhichPlatform] = new List<PlatformTransition>();
        }

        /*
         * We know that there are references to other platforms that needs to be accounted for, 
         * thus this loop can help find out what kind of transition it is: a jump, or a drop
         */
        foreach (Platform transitionalAdjacency in transitionalPlatformRef.ConnectedPlatforms)
        {
            TransitionType transitionType = TransitionType.WALK;

            /*
             * The type of transition can easily be found by just taking the difference in y
             * between the two platforms
             */
            float yDifference = transitionalAdjacency.Bounds.center.y - 
                fromWhichPlatform.Bounds.center.y;

            if (yDifference > 0f)
            {
                transitionType = TransitionType.JUMP;
            }
            else if (yDifference < 0f)
            {
                transitionType = TransitionType.DROP;
            }
            else
            {
                Debug.LogWarning($"Platform {fromWhichPlatform.name} is level with " +
                    $"{transitionalAdjacency.name}. It doesn't need a transition to it. Please " +
                    $"remove it.");
                continue;
            }

            PlatformTransition newTransition = new PlatformTransition(fromWhichPlatform, 
                transitionalAdjacency, transitionalAdjacency.TopCenter, transitionType);
            this.adjacencyList[fromWhichPlatform].Add(newTransition);
        }
    }

    /// <summary>
    /// Prints the graph for debugging purposes
    /// </summary>
    private void PrintGraph()
    {
        foreach (KeyValuePair<Platform, List<PlatformTransition>> entry in this.adjacencyList)
        {
            Platform key = entry.Key;

            Debug.Log($"{key.name}:");

            foreach (PlatformTransition transition in entry.Value)
            {
                Debug.Log(
                    $"\t{transition.DestinationPlatform.name} " +
                    $"({transition.Transition}) " +
                    $"Target: {transition.TargetPosition}");
            }
        }
    }
}
