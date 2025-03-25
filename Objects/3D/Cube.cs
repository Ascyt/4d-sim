using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Hyperobject
{
    public Cube(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color) : base(new ConnectedVertices[]
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
            },
            
            color,

            connections: new int[,]
            {
                { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 }, // First square
                { 4, 5 }, { 5, 6 }, { 6, 7 }, { 7, 4 }, // Second square
                { 0, 7 }, { 1, 6 }, { 2, 5 }, { 3, 4 }, // Connecting first and second square to form a cube
            }
        )
    }, position)
    {
        
    }
}
