using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hyperobject
{
    public readonly Tetrahedron[] hypermesh;
    public Vector4 position;
    public Hyperrotation rotation;

    public Hyperobject(Tetrahedron[] hypermesh)
    {
        this.hypermesh = hypermesh;
    }
}
