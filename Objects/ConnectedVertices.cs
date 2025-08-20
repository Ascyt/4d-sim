using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a collection of 4D vertices that can be considered as belonging to the same part of an object.
/// </summary>
public class ConnectedVertices 
{
    public enum ConnectionMethod
    {
        /// <summary>
        /// Connects vertices with triangles, creating a convex hull object. 
        /// </summary>
        Solid,
        /// <summary>
        /// Renders vertices as points and connects them with lines, creating a wireframe object.
        /// </summary>
        Wireframe,
        /// <summary>
        /// Only renders the vertices as points without any connections.
        /// </summary>
        Vertices
    }

    public ConnectionMethod connectionMethod;
    public readonly Vector4[] identityVertices; 
    public Vector4[] vertices;
    public readonly int[][] connections;
    public float? vertexScale;
    public bool isEnabled = true;

    public Color color;

    public ConnectedVertices(ConnectionMethod connectionMethod, Vector4[] vertices, Color color, int[][] connections=null, float? vertexScale=null)
    {
        this.connectionMethod = connectionMethod;

        this.identityVertices = vertices;
        this.vertices = new Vector4[vertices.Length];
        Array.Copy(vertices, this.vertices, vertices.Length);

        this.color = color;
        this.connections = connections;
        this.vertexScale = vertexScale;
    }
}
