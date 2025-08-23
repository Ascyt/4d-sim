using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Includes a tesseract with one cell being highlighted and a smaller tesseract at one fo the vertices.
/// </summary>
public class FixedTesseractHyperscene : Hyperscene
{
    private HashSet<Hyperobject> _objects = new()
    {
    };
    public override HashSet<Hyperobject> Objects => _objects;

    private HashSet<Hyperobject> _fixedObjects = new()
    {
        new Axes(),
        new Tesseract(Vector4.zero, ConnectedVertices.ConnectionMethod.Wireframe, Color.white),
        new Cube(new Vector4(0, 0, 0, 0.5f), ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 1f, 1f, 1f / 4f)),
        //new Tesseract(Vector4.one / 2f, ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0f, 1f, 1f / 4f), Vector4.one * 1f / 8f),
    };
    public override HashSet<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
}
