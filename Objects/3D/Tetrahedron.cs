using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron : Hyperobject
{
    public enum CellOf
    {
        Pentatope,
        Orthoplex,
    }

    public Tetrahedron(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector4? scale = null, CellOf? cellOf = null) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            connectionMethod,

            GetVertices(scale ?? Vector4.one, cellOf),

            color,

            connections: new int[][]
            {
                new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 2 }, // triangle
                new[] { 0, 3 }, new[] { 1, 3 }, new[] { 2, 3 }  // connecting triangle vertices to last vertex
            }
        )
    }, position)
    {
        
    }

    private static Vector4[] GetVertices(Vector4 scale, CellOf? cellOf)
    {
        Vector4 s = scale / 2f;

        if (cellOf is null)
        {
            return
                 new Vector4[] {
                new( s.x, 0, s.z * -(Mathf.Sqrt(2f)/2f), 0), // 0
                new(-s.x, 0, s.z * -(Mathf.Sqrt(2f)/2f), 0), // 1
                new(0,  s.y, s.z *  (Mathf.Sqrt(2f)/2f), 0), // 2
                new(0, -s.y, s.z *  (Mathf.Sqrt(2f)/2f), 0), // 3
                 };
        }

        if (cellOf.Value == CellOf.Pentatope)
        {
            float phi = (1f + Mathf.Sqrt(5f)) / 2f; // Golden ratio

            Vector4 delta = s / (2 - (1f / phi));

            return
                 new Vector4[] {
                    new Vector4(2*s.x  , 0      , 0      , 0      ) - delta, // 0
                    new Vector4(0      , 2*s.y  , 0      , 0      ) - delta, // 1
                    new Vector4(0      , 0      , 2*s.z  , 0      ) - delta, // 2
                    new Vector4(0      , 0      , 0      , 2*s.w  ) - delta, // 3
                 };
        }
        if (cellOf.Value == CellOf.Orthoplex)
        {
            return
                 new Vector4[] {
                    new( s.x,    0,    0,    0), // 0
                    new(   0,  s.y,    0,    0), // 1
                    new(   0,    0,  s.z,    0), // 2
                    new(   0,    0,    0,  s.w), // 3
                 };
        }

        Debug.LogError($"Unsupported cell type {cellOf}. Returning default tetrahedron vertices.");
        return GetVertices(scale, null);
    }
}
