using UnityEngine;

public class PathMarker {
    public CoordinatesInMaze location;
    public float G, H, F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(CoordinatesInMaze loc, float g, float h, float f, PathMarker par, GameObject marker) {
        location = loc;
        G = g;
        H = h;
        F = f;
        parent = par;
        this.marker = marker;
    }

    public override bool Equals(object obj) {
        if (obj == null || !this.GetType().Equals(obj.GetType())) return false;
        else {
            return location.Equals(((PathMarker)obj).location);
        }
    }

    public override int GetHashCode() {
        return 0;
    }
}