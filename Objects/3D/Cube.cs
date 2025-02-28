using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Hyperobject
{
    public Cube() : base(new Tetrahedron[]
    {
        new Tetrahedron(
            new Vector4(0, 0, 0, 0),
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 1, 0)
            ),
        new Tetrahedron(
            new Vector4(1, 0, 0, 0),
            new Vector4(1, 1, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(1, 0, 1, 0)
            ),
        new Tetrahedron(
            new Vector4(0, 1, 0, 0),
            new Vector4(1, 1, 0, 0),
            new Vector4(0, 1, 1, 0),
            new Vector4(1, 0, 1, 0)
            ),
        new Tetrahedron(
            new Vector4(0, 1, 1, 0),
            new Vector4(1, 0, 1, 0),
            new Vector4(1, 1, 1, 0),
            new Vector4(0, 0, 1, 0)
            ),
        new Tetrahedron(
            new Vector4(1, 0, 1, 0),
            new Vector4(1, 1, 0, 0),
            new Vector4(1, 1, 1, 0),
            new Vector4(0, 0, 1, 0)
            )
    })
    {
        
    }
}
