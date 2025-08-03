using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 4D analogy of an octahedron. Also known as a 4-orthoplex. Made of 16 tetrahedral cells.
/// </summary>
public class Orthoplex : Hyperobject
{
    public Orthoplex(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector4? scale = null) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            connectionMethod,

            GetVertices(scale ?? Vector4.one),

            color,

            // Tetrahedrons using +- combinations of dimensions, duplicate connections can be ignored
            connections: new int[][]
            {
                                                                /* triangles | last tetrahedron vertex */
                  new[] { 0, 2 },     new[] { 0, 4 },     new[] { 2, 4 },        new[] { 0, 6 },     new[] { 2, 6 },     new[] { 4, 6 },     // (0, 2, 4, 6) = ( x,  y,  z,  w) 
                  new[] { 1, 2 },     new[] { 1, 4 },                            new[] { 1, 6 },                                             // (1, 2, 4, 6) = (-x,  y,  z,  w) 1
                  new[] { 0, 3 },                         new[] { 3, 4 },                            new[] { 3, 6 },                         // (0, 3, 4, 6) = ( x, -y,  z,  w)   3
                  new[] { 1, 3 },                                                                                                            // (1, 3, 4, 6) = (-x, -y,  z,  w) 1 3
                                      new[] { 0, 5 },     new[] { 2, 5 },                                                new[] { 5, 6 },     // (0, 2, 5, 6) = ( x,  y, -z,  w)     5
                                      new[] { 1, 5 },                                                                                        // (1, 2, 5, 6) = (-x,  y, -z,  w) 1   5
                                                          new[] { 3, 5 },                                                                    // (0, 3, 5, 6) = ( x, -y, -z,  w)   3 5
                                                                                                                                             // (1, 3, 5, 6) = (-x, -y, -z,  w) 1 3 5
                                                                                 new[] { 0, 7 },     new[] { 2, 7 },     new[] { 4, 7 },     // (0, 2, 4, 6) = ( x,  y,  z, -w) 
                                                                                 new[] { 1, 7 },                                             // (1, 2, 4, 6) = (-x,  y,  z, -w) 1     7
                                                                                                     new[] { 3, 7 },                         // (0, 3, 4, 6) = ( x, -y,  z, -w)   3   7
                                                                                                                                             // (1, 3, 4, 6) = (-x, -y,  z, -w) 1 3   7
                                                                                                                         new[] { 5, 7 },     // (0, 2, 5, 6) = ( x,  y, -z, -w)     5 7
                                                                                                                                             // (1, 2, 5, 6) = (-x,  y, -z, -w) 1   5 7
                                                                                                                                             // (0, 3, 5, 6) = ( x, -y, -z, -w)   3 5 7
                                                                                                                                             // (1, 3, 5, 6) = (-x, -y, -z, -w) 1 3 5 7
            }
        )
    }, position)
    {

    }

    private static Vector4[] GetVertices(Vector4 scale)
    {
        Vector4 s = scale / 2f;
        return
             new Vector4[] {
                new( s.x,    0,    0,    0), // 0
                new(-s.x,    0,    0,    0), // 1
                new(   0,  s.y,    0,    0), // 2
                new(   0, -s.y,    0,    0), // 3
                new(   0,    0,  s.z,    0), // 4
                new(   0,    0, -s.z,    0), // 5
                new(   0,    0,    0,  s.w), // 6
                new(   0,    0,    0, -s.w), // 7
             };
    }
}
