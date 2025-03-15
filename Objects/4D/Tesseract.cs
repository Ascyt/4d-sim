using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tesseract : Hyperobject
{
    public Tesseract(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Material material) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            connectionMethod,

             new Vector4[] {
                new Vector4(-.5f, -.5f, -.5f, -.5f), // 0
                new Vector4(.5f, -.5f, -.5f, -.5f), // 1
                new Vector4(.5f, .5f, -.5f, -.5f), // 2
                new Vector4(-.5f, .5f, -.5f, -.5f), // 3
                new Vector4(-.5f, .5f, .5f, -.5f), // 4
                new Vector4(.5f, .5f, .5f, -.5f), // 5
                new Vector4(.5f, -.5f, .5f, -.5f), // 6
                new Vector4(-.5f, -.5f, .5f, -.5f),  // 7
    
                new Vector4(-.5f, -.5f, -.5f, .5f), // 8
                new Vector4(.5f, -.5f, -.5f, .5f), // 9
                new Vector4(.5f, .5f, -.5f, .5f), // 10
                new Vector4(-.5f, .5f, -.5f, .5f), // 11
                new Vector4(-.5f, .5f, .5f, .5f), // 12
                new Vector4(.5f, .5f, .5f, .5f), // 13
                new Vector4(.5f, -.5f, .5f, .5f), // 14
                new Vector4(-.5f, -.5f, .5f, .5f)  // 15
            },

            color,

            material,

            connections: new int[,]
            {
                { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 }, // First square
                { 4, 5 }, { 5, 6 }, { 6, 7 }, { 7, 4 }, // Second square
                { 0, 7 }, { 1, 6 }, { 2, 5 }, { 3, 4 }, // Connecting first and second square to form a cube

                { 8, 9 }, { 9, 10 }, { 10, 11 }, { 11, 8 }, // First square
                { 12, 13 }, { 13, 14 }, { 14, 15 }, { 15, 12 }, // Second Square
                { 8, 15 }, { 9, 14 }, { 10, 13 }, { 11, 12 }, // Connecting first and second square to form a cube

                { 0, 8 }, { 1, 9 }, { 2, 10 }, { 3, 11 }, // Connecting the two cubes
                { 4, 12 }, { 5, 13 }, { 6, 14 }, { 7, 15 } // Connecting the two cubes
            }
        )
    }, position)
    {

    }
}
