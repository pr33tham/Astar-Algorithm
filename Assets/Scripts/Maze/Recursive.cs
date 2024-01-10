﻿public class Recursive : GenerateMaze {
    public override void Generate() {
        Generate(10, 10);
    }
    void Generate(int x, int z) {
        if (CountSquareNeighbours(x, z) >= 2) return;
        map[x, z] = 0;

        directions.Shuffle();

        Generate(x + directions[0].x, z + directions[0].z);
        Generate(x + directions[1].x, z + directions[1].z);
        Generate(x + directions[2].x, z + directions[2].z);
        Generate(x + directions[3].x, z + directions[3].z);
    }
}
