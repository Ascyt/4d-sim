using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Includes a ground hypercube with a few tesseracts in the center.
/// </summary>
public class GroundHyperscene : Hyperscene
{
    private HashSet<Hyperobject> _objects = new()
    {
        new Tesseract(new Vector4(0, -2f, 0, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(0.5f, 0.5f, 0.5f, 0.5f), new Vector4(10, 3, 10, 10 )),

        new C5(new Vector4(0, 0, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.cyan, Vector4.one / 2f),

        new Tesseract(new Vector4(1, 0, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.red, Vector4.one / 2f),
        new Tesseract(new Vector4(0, 1, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.green, Vector4.one / 2f),
        new Tesseract(new Vector4(0, 0, 1, 0), ConnectedVertices.ConnectionMethod.Wireframe, new Color(0, 0.5f, 1), Vector4.one / 2f),
        new Tesseract(new Vector4(0, 0, 0, 1), ConnectedVertices.ConnectionMethod.Wireframe, Color.yellow, Vector4.one / 2f),
    };
    public override HashSet<Hyperobject> Objects => _objects;

    private HashSet<Hyperobject> _fixedObjects = new()
    {
        new Axes()
    };
    public override HashSet<Hyperobject> FixedObjects => _fixedObjects;
    public override Vector4 StartingPosition => new Vector4(0, 0, 0, -1.5f);
}
