using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for 4D hyperobjects.
/// </summary>
public abstract class Hyperobject
{
    public readonly ConnectedVertices[] vertices;
    public Vector4 position;
    public RotationEuler4 rotation;

    public Hyperobject(ConnectedVertices[] vertices, Vector4 position)
    {
        this.vertices = vertices;
        this.position = position;
    }
}
