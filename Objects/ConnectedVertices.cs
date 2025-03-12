using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedVertices 
{
    public Vector4[] vertices;

    public ConnectedVertices(params Vector4[] vertices)
    {
        this.vertices = vertices;
    }
}
