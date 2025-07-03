using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Plane : Hyperobject
{
    public Plane(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector2? scale = null) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
        connectionMethod,

            GetVertices(scale ?? Vector2.one),

            color,

            connections: new int[][]
            {
                new[] { 0, 1 }, new[] { 1, 2 }, new[] { 2, 3 }, new[] { 3, 0 }
            }
        )
    }, position)
    {

    }

    private static Vector4[] GetVertices(Vector2 scale)
    {
        Vector3 s = scale / 2f;
        return
             new Vector4[] {
                new(-s.x, -s.y, 0, 0), // 0
                new( s.x, -s.y, 0, 0), // 1
                new( s.x,  s.y, 0, 0), // 2
                new(-s.x,  s.y, 0, 0), // 3
             };
    }
}
