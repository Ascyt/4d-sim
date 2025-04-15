using UnityEngine;
using MIConvexHull;

public class MIVertex : IVertex
{
    public double[] Position { get; set; }

    public MIVertex(Vector3 point)
    {
        Position = new double[] { point.x, point.y, point.z };
    }

    public Vector3 ToVector3()
    {
        return new Vector3((float)Position[0], (float)Position[1], (float)Position[2]);
    }
}