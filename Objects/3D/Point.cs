using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : Hyperobject
{
    public Point(Vector4 position) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Vertices,

            new Vector4[] {
                new Vector4(0, 0, 0, 0)
            }
        )
    }, position)
    {

    }
}
