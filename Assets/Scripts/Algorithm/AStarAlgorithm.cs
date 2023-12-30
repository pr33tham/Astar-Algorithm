using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PathMarker
{
    public PointInMaze location;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(PointInMaze loc, float g, float h, GameObject m, PathMarker parent)
    {
        this.location = loc;
        this.G = g;
        this.H = h;
        this.F = this.G + this.H;
        this.marker = m;
        this.parent = parent;
    }
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return location.Equals(((PathMarker)obj).location);
    }

    public override int GetHashCode()
    {
        return 0;
    }
}

public class AStarAlgorithm : MonoBehaviour
{

    public GenerateMaze maze;
    public GameObject start;
    public Material startMateral;
    public GameObject goal;
    public Material goalMaterial;
    public GameObject PathP;
    public Material pathMaterial;

    List<PathMarker> openList = new List<PathMarker>();
    List<PathMarker> closedList = new List<PathMarker>();

    PathMarker startNode;
    PathMarker goalNode;
    PathMarker lastPosition;

    bool completed = false;
    bool hasStarted = false;
    private void RemoveAllMarkers()
    {
        GameObject[] markersInScene = GameObject.FindGameObjectsWithTag("Marker");
        foreach (GameObject obj in markersInScene)
        {
            Destroy(obj);
        }
    }

    private bool IsClosed(PointInMaze toCheck)
    {
        foreach (PathMarker p in closedList)
        {
            if (p.location.Equals(toCheck))
            {
                return true;
            }
        }
        return false;
    }

    private bool UpdateMarker(PointInMaze loc, float g, float h, PathMarker par)
    {
        foreach (PathMarker p in openList)
        {

            if (p.location.Equals(loc))
            {

                p.G = g;
                p.H = h;
                p.F = g + h;
                p.parent = par;
                return true;
            }
        }
        return false;
    }

    private void BeginSearch()
    {
        completed = false;
        RemoveAllMarkers();
        openList.Clear();
        closedList.Clear();

        List<PointInMaze> corridors = new List<PointInMaze>();
        for (int z = 0; z < maze.depth; z++)
        {
            for (int x = 0; x < maze.length; x++)
            {
                if (maze.map[x, z] == 0)
                {
                    corridors.Add(new PointInMaze(x, z));
                }
            }
        }
        corridors.Shuffle();

        Vector3 startLocation = new Vector3(corridors[0].x * maze.scale, maze.scale / 2, corridors[0].z * maze.scale);
        GameObject startCharacter = Instantiate(start, startLocation, Quaternion.identity);
        startCharacter.transform.localScale = new Vector3(maze.scale / 2, maze.scale, maze.scale / 2);
        startNode = new PathMarker(new PointInMaze((int)startLocation.x, (int)startLocation.z), 0, 0, startCharacter, null);

        Vector3 goalLocation = new Vector3(corridors[1].x * maze.scale, maze.scale / 2, corridors[1].z * maze.scale);
        GameObject goalCharacter = Instantiate(goal, goalLocation, Quaternion.identity);
        goalCharacter.transform.localScale = new Vector3(maze.scale / 2, maze.scale, maze.scale / 2);
        goalNode = new PathMarker(new PointInMaze((int)goalLocation.x, (int)goalLocation.z), 0, 0, goalCharacter, null);


        openList.Add(startNode);
        lastPosition = startNode;
    }

    private void Search(PathMarker currentNode)
    {
        if (currentNode == null) return;
        if (currentNode.Equals(goalNode))
        {
            completed = true;
            return;
        }
        foreach (PointInMaze dir in maze.directions)
        {

            PointInMaze neighbour = dir + currentNode.location;

            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (neighbour.x < 1 || neighbour.x >= maze.length || neighbour.z < 1 || neighbour.z >= maze.depth) continue;
            if (IsClosed(neighbour)) continue;

            float g = CalculateG(currentNode, neighbour);
            float h = HeuristicForH(neighbour);

            Vector3 neighborLocation = new Vector3(neighbour.x * maze.scale, maze.scale / 2, neighbour.z * maze.scale);
            GameObject path = Instantiate(PathP, neighborLocation, Quaternion.identity);

            if (!UpdateMarker(neighbour, g, h, currentNode))
            {
                openList.Add(new PathMarker(neighbour, g, h, path, currentNode));
            }
        }

        openList = openList.OrderBy(pf => pf.F).ThenBy(ph => ph.H).ToList<PathMarker>();
        PathMarker leastF = (PathMarker)openList.ElementAt(0);
        openList.RemoveAt(0);

        closedList.Add(leastF);

        leastF.marker.GetComponent<Renderer>().material = pathMaterial;
        lastPosition = leastF;
    }

    private static float CalculateG(PathMarker currentNode, PointInMaze neighbour)
    {
        return Vector2.Distance(currentNode.location.ToVector(), neighbour.ToVector()) + currentNode.G;
    }

    private float HeuristicForH(PointInMaze neighbour)
    {
        return Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            BeginSearch();
            hasStarted = true;
        }
        if (hasStarted && Input.GetKeyDown(KeyCode.R))
        {
            Search(lastPosition);
        }
    }

}
