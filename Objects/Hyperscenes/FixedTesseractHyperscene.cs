using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedTesseractHyperscene : Hyperscene
{
    private List<Hyperobject> _objects = new()
    {
    };
    public override List<Hyperobject> Objects => _objects;

    private List<Hyperobject> _fixedObjects = new()
    {
        new Axes(),
        new Tesseract(new Vector4(0, 0, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.white),
        new Cube(new Vector4(0, 0, 0, 0.5f), ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 1f, 1f, 1f / 32f)),
        new Tesseract(new Vector4(0.5f, 0.5f, 0.5f, 0.5f), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0f, 1f, 1f / 16f), Vector4.one * 1f / 8f),
    };
    public override List<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
}
