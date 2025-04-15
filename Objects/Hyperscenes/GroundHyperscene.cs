using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHyperscene : Hyperscene
{
    private List<Hyperobject> _objects = new()
    {
        new Tesseract(new Vector4(0, -2f, 0, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(0.5f, 0.5f, 0.5f, 0.5f), new Vector4(25, 3, 25, 25)),

        new Tesseract(new Vector4(0, 0, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.cyan, Vector4.one / 4f),

        new Tesseract(new Vector4(1, 0, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.red, Vector4.one / 2f),
        new Tesseract(new Vector4(0, 1, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.green, Vector4.one / 2f),
        new Tesseract(new Vector4(0, 0, 1, 0), ConnectedVertices.ConnectionMethod.Wireframe, new Color(0, 0.5f, 1), Vector4.one / 2f),
        new Tesseract(new Vector4(0, 0, 0, 1), ConnectedVertices.ConnectionMethod.Wireframe, Color.yellow, Vector4.one / 2f),
    };
    public override List<Hyperobject> Objects => _objects;

    private List<Hyperobject> _fixedObjects = new()
    {
        new Axes()
    };
    public override List<Hyperobject> FixedObjects => _fixedObjects;
}
