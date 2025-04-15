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
    /// <summary>
    /// Vertices transformed so camera position and rotation is at 0 (no projection)
    /// </summary>
    public Vector4[] transformedVertices;
    public readonly int[][] connections;
    public float? vertexScale;

    public Color color;

    public ConnectedVertices(ConnectionMethod connectionMethod, Vector4[] vertices, Color color, int[][] connections=null, float? vertexScale=null)
    {
        this.connectionMethod = connectionMethod;
        this.vertices = vertices;
        this.color = color;
        this.connections = connections;
        this.vertexScale = vertexScale;
    }
}
