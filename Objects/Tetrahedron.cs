using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron 
{
    private Vector4[] _vertices;
    public Vector4[] vertices
    {
        get { return _vertices; }
        set 
        {
            if (value.Length != 4)
            {
                throw new System.ArgumentException("Tetrahedron must have 4 vertices");
            }
            _vertices = value;
        }
    }

    public Tetrahedron(Vector4 a, Vector4 b, Vector4 c, Vector4 d)
        : this(new Vector4[4] { a, b, c, d }) { }
    public Tetrahedron(Vector4[] vertices)
    {
        this.vertices = vertices;
    }

    public void PlaceInWorld()
    {
        
    }
}
