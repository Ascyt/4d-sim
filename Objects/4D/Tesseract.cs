using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tesseract : Hyperobject
{
    public Tesseract(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, float scale) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            connectionMethod,

             new Vector4[] {
                new Vector4(-1, -1, -1, -1) * (scale / 2f), // 0
                new Vector4(1, -1, -1, -1)  * (scale / 2f), // 1
                new Vector4(1, 1, -1, -1)   * (scale / 2f), // 2
                new Vector4(-1, 1, -1, -1)  * (scale / 2f), // 3
                new Vector4(-1, 1, 1, -1)   * (scale / 2f), // 4
                new Vector4(1, 1, 1, -1)    * (scale / 2f), // 5
                new Vector4(1, -1, 1, -1)   * (scale / 2f), // 6
                new Vector4(-1, -1, 1, -1)  * (scale / 2f), // 7
    
                new Vector4(-1, -1, -1, 1)  * (scale / 2f), // 8
                new Vector4(1, -1, -1, 1)   * (scale / 2f), // 9
                new Vector4(1, 1, -1, 1)    * (scale / 2f), // 10
                new Vector4(-1, 1, -1, 1)   * (scale / 2f), // 11
                new Vector4(-1, 1, 1, 1)    * (scale / 2f), // 12
                new Vector4(1, 1, 1, 1)     * (scale / 2f), // 13
                new Vector4(1, -1, 1, 1)    * (scale / 2f), // 14
                new Vector4(-1, -1, 1, 1)   * (scale / 2f)  // 15
            },

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
}
