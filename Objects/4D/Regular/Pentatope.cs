using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 4D analogy of a tetrahedron. Also known as a 4-simplex. Made of 5 tetrahedral cells. 
/// </summary>
public class Pentatope : Hyperobject
{
    public Pentatope(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector4? scale = null) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            connectionMethod,

            GetVertices(scale ?? Vector4.one),

            color,

            connections: new int[][]
            {
                new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 2 }, // triangle
                new[] { 0, 3 }, new[] { 1, 3 }, new[] { 2, 3 }, // tetrahedron
                new[] { 0, 4 }, new[] { 1, 4 }, new[] { 2, 4 }, new[] { 3, 4 }  // connecting tetrahedron vertices to last vertex
            }
        )
    }, position)
    {
        
    }

    private static Vector4[] GetVertices(Vector4 scale)
    {
        Vector4 s = scale / 2f;
        float phi = (1f + Mathf.Sqrt(5f)) / 2f; // Golden ratio

        Vector4 delta = s / (2 - (1f / phi));

        return
             new Vector4[] {
                new Vector4(2*s.x  , 0      , 0      , 0      ) - delta, // 0
                new Vector4(0      , 2*s.y  , 0      , 0      ) - delta, // 1
                new Vector4(0      , 0      , 2*s.z  , 0      ) - delta, // 2
                new Vector4(0      , 0      , 0      , 2*s.w  ) - delta, // 3
                new Vector4(phi*s.x, phi*s.y, phi*s.z, phi*s.w) - delta, // 4
             };
    }
}
