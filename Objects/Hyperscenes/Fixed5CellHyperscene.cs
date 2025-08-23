using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Includes a tesseract with one cell being highlighted and a smaller tesseract at one fo the vertices.
/// </summary>
public class Fixed5CellHyperscene : Hyperscene
{
    private HashSet<Hyperobject> _objects = new()
    {
    };
    public override HashSet<Hyperobject> Objects => _objects;

    private HashSet<Hyperobject> _fixedObjects = new()
    {
        new Axes(),
        new C5(Vector4.zero, ConnectedVertices.ConnectionMethod.Wireframe, Color.white),
        new Tetrahedron(Vector4.zero, ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 1f, 1f, 1f / 4f), cellOf:Tetrahedron.CellOf.Pentatope),
    };
    public override HashSet<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
}
