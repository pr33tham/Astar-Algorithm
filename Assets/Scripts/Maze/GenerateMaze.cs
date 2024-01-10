using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour {

    public List<CoordinatesInMaze> directions = new List<CoordinatesInMaze>() {
                                            new CoordinatesInMaze(1,0),
                                            new CoordinatesInMaze(0,1),
                                            new CoordinatesInMaze(-1,0),
                                            new CoordinatesInMaze(0,-1) };

    public int length = 10;
    public int depth = 10;
    public int scale = 5; // change it to 5 for plane to place perfectly
    public GameObject parent;

    public byte[,] map;

    //camera angle

    //  set everything as wall
    private void Initialise() {
        map = new byte[length, depth];
        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < length; x++) map[x, z] = 1; // wall
        }
    }
    //  corridors
    public virtual void Generate() {
        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < length; x++) {
                if (Random.Range(0, 100) < 50) map[x, z] = 0; //corridor
            }
        }
    }

    private void GenerateWalls() {
        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < length; x++)
                if (map[x, z] == 1) {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "Cube[" + x.ToString() + ", " + z.ToString() + "]";
                    cube.transform.parent = parent.transform;
                    cube.transform.localScale = new Vector3(scale, scale, scale);
                    cube.transform.position = new Vector3(x * scale, scale / 2, z * scale);
                }
        }
    }

    public int CountSquareNeighbours(int x, int z) {
        int count = 0;
        if (x <= 0 || x >= length - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

    public int CountDiagonalNeighbours(int x, int z) {
        int count = 0;
        if (x <= 0 || x >= length - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z - 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        return count;
    }

    public int CountAllNeighbours(int x, int z) {
        return CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
    }

    //plane
    private void GeneratePlane() {
        float pos = (float)scale * 15 - (float)scale/2;
        Debug.Log(pos);
        float planeScale = (float)scale * 3;
        Debug.Log(pos);
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = new Vector3(pos, 1, pos);
        plane.transform.localScale = new Vector3(planeScale, 0, planeScale);
        plane.GetComponent<Renderer>().material.color = Color.black;
    }
    
    private void Start() {
        Initialise();
        Generate();
        GenerateWalls();
        GeneratePlane();
    }
}