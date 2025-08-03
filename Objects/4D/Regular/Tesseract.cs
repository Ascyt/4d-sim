using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 4D analogy of a cube. Also known as an 4-cube. Made of 8 cubic cells.
/// </summary>
public class Tesseract : Hyperobject
{
    public Tesseract(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector4? scale=null) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            connectionMethod,

            GetVertices(scale ?? Vector4.one),

            color,

            connections: new int[][]
            {
                new[] { 0, 1 },   new[] { 1, 2 },   new[] { 2, 3 },   new[] { 3, 0 },   // First square
                new[] { 4, 5 },   new[] { 5, 6 },   new[] { 6, 7 },   new[] { 7, 4 },   // Second square
                new[] { 0, 7 },   new[] { 1, 6 },   new[] { 2, 5 },   new[] { 3, 4 },   // Connecting first and second square to form a cube
                new[] { 8, 9 },   new[] { 9, 10 },  new[] { 10, 11 }, new[] { 11, 8 },  // First square
                new[] { 12, 13 }, new[] { 13, 14 }, new[] { 14, 15 }, new[] { 15, 12 }, // Second Square
                new[] { 8, 15 },  new[] { 9, 14 },  new[] { 10, 13 }, new[] { 11, 12 }, // Connecting first and second square to form a cube
                new[] { 0, 8 },   new[] { 1, 9 },   new[] { 2, 10 },  new[] { 3, 11 },  // Connecting the two cubes
                new[] { 4, 12 },  new[] { 5, 13 },  new[] { 6, 14 },  new[] { 7, 15 }   // Connecting the two cubes
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
                new(-s.x, -s.y, -s.z, -s.w), // 0
                new( s.x, -s.y, -s.z, -s.w), // 1
                new( s.x,  s.y, -s.z, -s.w), // 2
                new(-s.x,  s.y, -s.z, -s.w), // 3
                new(-s.x,  s.y,  s.z, -s.w), // 4
                new( s.x,  s.y,  s.z, -s.w), // 5
                new( s.x, -s.y,  s.z, -s.w), // 6
                new(-s.x, -s.y,  s.z, -s.w), // 7
                new(-s.x, -s.y, -s.z,  s.w), // 8
                new( s.x, -s.y, -s.z,  s.w), // 9
                new( s.x,  s.y, -s.z,  s.w), // 10
                new(-s.x,  s.y, -s.z,  s.w), // 11
                new(-s.x,  s.y,  s.z,  s.w), // 12
                new( s.x,  s.y,  s.z,  s.w), // 13
                new( s.x, -s.y,  s.z,  s.w), // 14
                new(-s.x, -s.y,  s.z,  s.w)  // 15
             };
    } 
}
