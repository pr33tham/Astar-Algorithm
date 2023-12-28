using UnityEngine;

public class PointInMaze
{
    public int x;
    public int z;

    public PointInMaze(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public Vector2 ToVector()
    {
        return new Vector2(this.x, this.z);
    }
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return x == ((PointInMaze)obj).x && z == ((PointInMaze)obj).z;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
