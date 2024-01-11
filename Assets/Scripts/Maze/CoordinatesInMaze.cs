using UnityEngine;

public class CoordinatesInMaze {
    public int x, z;

    public CoordinatesInMaze(int _x, int _z) {
        x = _x; //x coordinate
        z = _z; //z coordinate
    }

    public Vector3 ToVector() {
        return new Vector2(this.x, this.z);
    }

    public static CoordinatesInMaze operator +(CoordinatesInMaze a, CoordinatesInMaze b) {
        return new CoordinatesInMaze(a.x + b.x, a.z + b.z);
    }

    public override bool Equals(object obj) {
        if (obj == null || !this.GetType().Equals(obj.GetType())) return false;
        else {
            CoordinatesInMaze p = (CoordinatesInMaze)obj;
            return p.x == x && p.z == z;
        }
    }

    public override int GetHashCode() {
        return 0;
    }
}

