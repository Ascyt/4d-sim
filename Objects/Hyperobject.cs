using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for 4D hyperobjects.
/// </summary>
public abstract class Hyperobject
{
    public readonly ConnectedVertices[] connectedVertices;
    public readonly Vector4 startPosition;
    public Vector4 position;

    private Quatpair _rotation = Quatpair.identity;
    public Quatpair Rotation
    {
        get => _rotation;
        set
        {
            foreach (ConnectedVertices part in connectedVertices)
            {
                for (int i = 0; i < part.vertices.Length; i++)
                {
                    part.vertices[i] = value * part.identityVertices[i];
                }
            }

            _rotation = value;
        }
    }

    public Hyperobject(ConnectedVertices[] vertices, Vector4 position, Quatpair? rotation=null)
    {
        this.connectedVertices = vertices;
        this.position = position;
        this.startPosition = position;
        _rotation = rotation ?? Quatpair.identity;
    }
}
