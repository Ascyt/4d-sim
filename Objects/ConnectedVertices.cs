using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedVertices 
{
    public enum ConnectionMethod
    {
        Solid, Wireframe, Vertices
    }

    public ConnectionMethod connectionMethod;
    public Vector4[] vertices;
    public int[,] connections;

    public Color color;

    public ConnectedVertices(ConnectionMethod connectionMethod, Vector4[] vertices, Color color, int[,] connections=null)
    {
        this.connectionMethod = connectionMethod;
        this.vertices = vertices;
        this.color = color;
        this.connections = connections;
    }
}
