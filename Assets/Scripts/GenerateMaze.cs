using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateMaze : MonoBehaviour
{
    public List<PointInMaze> directions = new List<PointInMaze>() {
                                            new PointInMaze(1,0),
                                            new PointInMaze(0,1),
                                            new PointInMaze(-1,0),
                                            new PointInMaze(0,-1) };

    public GameObject parent;
    public new Transform camera;
    public Material cubeMaterial;
    public int length = 10; //x
    public int depth = 10; //z
    public byte[,] map;
    public int scale; // size of walls


    private void InitialiseMaze()
    {
        map = new byte[length, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < length; x++)
            {
                map[x, z] = 1;

            }
        }
    }
    public virtual void Generate()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < length; x++)
            {
                if (x == 0 || z == 0 || x == length - 1 || z == depth - 1) continue; // do not change for end walls
                if (Random.Range(0, 100) < 50)
                {
                    map[x, z] = 0;
                }
            }
        }
    }
    void GenerateWalls()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < length; x++)
            {
                if (map[x, z] == 1)
                {
                    Vector3 location = new Vector3(x * scale, 0, z * scale);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
                    cube.transform.parent = parent.transform;
                    cube.transform.position = location;
                    cube.transform.localScale = new Vector3(scale, scale, scale);
                    cube.GetComponent<Renderer>().material = cubeMaterial;
                }
            }
        }
    }
    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= length - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

    public int CountDiagonalNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= length - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z - 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        return count;
    }

    public int CountAllNeighbours(int x, int z)
    {
        return CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
    }
    void GenerateGround()
    {
        float pos = (scale * length) / 2;
        Vector3 position = new Vector3(pos - 2.5f, 0, pos - 2.5f);
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = position;
        plane.transform.localScale = new Vector3(scale * 3 + 1, 1, scale * 3 + 1);
        plane.GetComponent<Renderer>().material.color = Color.black;
    }

    // Start is called before the first frame update
    void Start()
    {
        int calculatedDistace = scale * length;
        camera.transform.position = new Vector3(calculatedDistace/2, calculatedDistace, calculatedDistace/2);
        InitialiseMaze();
        Generate();
        GenerateWalls();
        GenerateGround();
    }
   

}
