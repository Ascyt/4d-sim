using UnityEngine;

public abstract class RenderableObject
{
    public Vector4[] vertices;

    public RenderableObject(Vector4[] vertices)
    {
        this.vertices = vertices;
    }
}