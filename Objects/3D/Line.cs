using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : Hyperobject
{
    public Line(Vector4 start, Vector4 end, Color color) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Vertices,

            new Vector4[] {
                new Vector4(0, 0, 0, 0),
                end - start
            },

            color
        )
    }, start)
    {

    }
}
