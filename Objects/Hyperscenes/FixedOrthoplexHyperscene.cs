using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Includes a tesseract with one cell being highlighted and a smaller tesseract at one fo the vertices.
/// </summary>
public class FixedOrthoplexHyperscene : Hyperscene
{
    private List<Hyperobject> _objects = new()
    {
    };
    public override List<Hyperobject> Objects => _objects;

    private List<Hyperobject> _fixedObjects = new()
    {
        new Axes(),
        new Orthoplex(Vector4.zero, ConnectedVertices.ConnectionMethod.Wireframe, Color.white),
        new Tetrahedron(Vector4.zero, ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 1f, 1f, 1f / 4f), cellOf:Tetrahedron.CellOf.Orthoplex),
    };
    public override List<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
}
