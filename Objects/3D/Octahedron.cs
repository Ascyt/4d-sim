using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octahedron : Hyperobject
{
    public Octahedron(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector4? scale = null) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            connectionMethod,

            GetVertices(scale ?? Vector3.one),

            color,

            connections: new int[][]
            {
                  new[] { 0, 2 },     new[] { 0, 4 },     new[] { 2, 4 },   // (0, 2, 4) = ( x,  y,  z)
                  new[] { 1, 2 },     new[] { 1, 4 },   /*new[] { 2, 4 },*/ // (1, 2, 4) = (-x,  y,  z)
                  new[] { 0, 3 },   /*new[] { 0, 4 },*/   new[] { 3, 4 },   // (0, 3, 4) = ( x, -y,  z)
                  new[] { 1, 3 },   /*new[] { 1, 4 },*/ /*new[] { 3, 4 },*/ // (1, 3, 4) = (-x, -y,  z)
                /*new[] { 0, 2 },*/   new[] { 0, 5 },     new[] { 2, 5 },   // (0, 2, 5) = ( x,  y, -z)
                /*new[] { 1, 2 },*/   new[] { 1, 5 },   /*new[] { 2, 5 },*/ // (1, 2, 5) = (-x,  y, -z)
                /*new[] { 0, 3 },*/ /*new[] { 0, 5 },*/   new[] { 3, 5 },   // (0, 3, 5) = ( x, -y, -z)
                /*new[] { 1, 3 },*/ /*new[] { 1, 5 },*/ /*new[] { 3, 5 },*/ // (1, 3, 5) = (-x, -y, -z)
            }
        )
    }, position)
    {

    }

    private static Vector4[] GetVertices(Vector3 scale)
    {
        Vector3 s = scale / 2f;
        return
             new Vector4[] {
                new( s.x,    0,    0, 0), // 0
                new(-s.x,    0,    0, 0), // 1
                new(   0,  s.y,    0, 0), // 2
                new(   0, -s.y,    0, 0), // 3
                new(   0,    0,  s.z, 0), // 4
                new(   0,    0, -s.z, 0), // 5
             };
    }
}
