using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Hyperobject
{
    public Cube(Vector4 position) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            new Vector4(0, 0, 0, 0),
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(1, 1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(1, 0, 1, 0),
            new Vector4(0, 1, 1, 0),
            new Vector4(1, 1, 1, 0)
            )
    }, position)
    {
        
    }
}
