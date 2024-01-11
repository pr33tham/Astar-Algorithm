using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class Algorithm : MonoBehaviour {

    //Variables and Objects
    public GenerateMaze maze;
    public GameObject start;
    public GameObject goal;
    public GameObject path;

    PathMarker startNode;
    PathMarker goalNode;
    PathMarker lastPos;

    private List<PathMarker> openList = new List<PathMarker>();
    private List<PathMarker> closedList = new List<PathMarker>();

    bool completed = false;
    bool hasStarted = false;
    bool pathFound = false;

    private void RemoveAllMarkers() {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Marker");
        foreach(GameObject obj in objects) {
            Destroy(obj);
        }
    }
    private void InitialiseSearch() {
        completed = false;
        RemoveAllMarkers();
        openList.Clear();
        closedList.Clear();

        List<CoordinatesInMaze> corridors = new List<CoordinatesInMaze>();
        for(int z = 0; z < maze.depth; z++) { 
            for(int x = 0; x < maze.length; x++) {
                if (maze.map[x, z] == 0) {
                    corridors.Add(new CoordinatesInMaze(x, z));
                }
            }
        }
        corridors.Shuffle();
        startNode = new PathMarker(
            corridors[0], 0.0f, 0.0f, 0.0f, null,
            Instantiate(start, new Vector3(corridors[0].x * maze.scale, (float)maze.scale / 2, corridors[0].z * maze.scale), Quaternion.identity)
            );
        goalNode = new PathMarker(
            corridors[1], 0.0f, 0.0f, 0.0f, null,
            Instantiate(goal, new Vector3(corridors[1].x * maze.scale, (float)maze.scale / 2, corridors[1].z * maze.scale), Quaternion.identity)
            );
        openList.Add(startNode);
        lastPos = startNode;
    }

    private float CalculateG(CoordinatesInMaze from, CoordinatesInMaze to) {
        return Vector2.Distance(from.ToVector(), to.ToVector());
    }

    private float CalculateHeuristic(CoordinatesInMaze from, CoordinatesInMaze to) {
        return Vector2.Distance(from.ToVector(), to.ToVector()); // TODO: apply different heuristic methods - distance, 
    }

    private bool IsCLosed(CoordinatesInMaze check) {
        foreach(PathMarker p in closedList) {
            if (p.location.Equals(check)) return true;
        }
        return false;
    }

    bool UpdateMarker(CoordinatesInMaze pos, float g, float h, float f, PathMarker prt) {

        foreach (PathMarker p in openList) {

            if (p.location.Equals(pos)) {

                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    private void Search(PathMarker currentNode) {
        if (currentNode == null) return;
        if (currentNode.Equals(goalNode)) {
            completed = true;
            return;
        }
        foreach(CoordinatesInMaze dir in maze.directions) {

            CoordinatesInMaze neighbour = currentNode.location + dir;
            if (neighbour.x < 1 || neighbour.x >= maze.length || neighbour.z < 1 || neighbour.z >= maze.depth) continue;
            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            if (IsCLosed(neighbour)) continue;

            float g = CalculateG(currentNode.location, neighbour);
            float h = CalculateHeuristic(neighbour, goalNode.location);
            float f = g + h;

            GameObject pathBlock = Instantiate(path, 
                new Vector3(neighbour.x * maze.scale, (float)maze.scale / 2, neighbour.z * maze.scale), 
                Quaternion.identity);
            if (!UpdateMarker(neighbour, g, h, f, currentNode)) {

                openList.Add(new PathMarker(neighbour, g, h, f, currentNode, pathBlock));
            }
            
        }
        openList.OrderBy(pf => pf.F).ThenBy(ph => ph.H).ToList<PathMarker>(); 
        PathMarker markerToAddInClosedList = (PathMarker)openList.ElementAt(0);
        openList.RemoveAt(0);
        closedList.Add(markerToAddInClosedList);
        lastPos = markerToAddInClosedList;
    }

    private void GetPath() {
        RemoveAllMarkers();

        PathMarker begin = lastPos;
        while (!startNode.Equals(begin) && begin != null) {
            Instantiate(path, new Vector3(begin.location.x * maze.scale, (float)maze.scale / 2, begin.location.z * maze.scale), Quaternion.identity);
            begin = begin.parent;
        }
        Instantiate(start, new Vector3(startNode.location.x * maze.scale, (float)maze.scale / 2, startNode.location.z * maze.scale), Quaternion.identity);
        Instantiate(goal, new Vector3(goalNode.location.x * maze.scale, (float)maze.scale / 2, goalNode.location.z * maze.scale), Quaternion.identity);

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            InitialiseSearch();
            hasStarted = true;
        }
        if(Input.GetKeyDown(KeyCode.R) &&  hasStarted) {
            while (!lastPos.Equals(goalNode)) {
            Search(lastPos);
            }
            pathFound = true;
        }

        if(Input.GetKeyDown(KeyCode.T) && pathFound) {
            GetPath();
        }
    }
}
