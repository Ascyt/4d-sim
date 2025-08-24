using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Hyperobject
{
    public Cube(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector3? scale = null, Quatpair? rotation = null) : base(new ConnectedVertices[]
    {
        new(
            connectionMethod,

            GetVertices(scale ?? Vector3.one),

            color,

            connections: new int[][]
            {
                new[] { 0, 1 }, new[] { 1, 2 }, new[] { 2, 3 }, new[] { 3, 0 }, // First square
                new[] { 4, 5 }, new[] { 5, 6 }, new[] { 6, 7 }, new[] { 7, 4 }, // Second square
                new[] { 0, 7 }, new[] { 1, 6 }, new[] { 2, 5 }, new[] { 3, 4 }, // Connecting first and second square to form a cube
            }
        )
    }, position, rotation)
    {
        
    }

    private static Vector4[] GetVertices(Vector3 scale)
    {
        Vector3 s = scale / 2f;
        return
             new Vector4[] {
                new(-s.x, -s.y, -s.z, 0), // 0
                new( s.x, -s.y, -s.z, 0), // 1
                new( s.x,  s.y, -s.z, 0), // 2
                new(-s.x,  s.y, -s.z, 0), // 3
                new(-s.x,  s.y,  s.z, 0), // 4
                new( s.x,  s.y,  s.z, 0), // 5
                new( s.x, -s.y,  s.z, 0), // 6
                new(-s.x, -s.y,  s.z, 0), // 7
             };
    }
}
