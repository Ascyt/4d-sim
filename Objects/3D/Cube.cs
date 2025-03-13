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
                new Vector4(-.5f, -.5f, -.5f, -.5f),
                new Vector4(.5f, -.5f, -.5f, -.5f),
                new Vector4(-.5f, .5f, -.5f, -.5f),
                new Vector4(.5f, .5f, -.5f, -.5f),
                new Vector4(-.5f, -.5f, .5f, -.5f),
                new Vector4(.5f, -.5f, .5f, -.5f),
                new Vector4(-.5f, .5f, .5f, -.5f),
                new Vector4(.5f, .5f, .5f, -.5f)
            },
            
            color
        )
    }, position)
    {
        
    }
}
