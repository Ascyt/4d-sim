using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Includes a tesseract with one cell being highlighted and a smaller tesseract at one fo the vertices.
/// </summary>
public class FixedRotationalPlanesHyperscene : Hyperscene
{
    private List<Hyperobject> _objects = new()
    {
    };
    public override List<Hyperobject> Objects => _objects;

    private const float PLANES_SCALE = 0.25f;
    private const float PLANES_ALPHA = 1f;

    private List<Hyperobject> _fixedObjects = new()
    {
        new Axes(),

        // XY
        new Tesseract(new Vector4(1, 1, 0, 0) * PLANES_SCALE / 2f, 
            ConnectedVertices.ConnectionMethod.Solid, new Color(0.5f, 0.5f, 0f, PLANES_ALPHA), 
            new Vector4(1, 1, 0, 0) * PLANES_SCALE),

        // XZ
        new Tesseract(new Vector4(1, 0, 1, 0) * PLANES_SCALE / 2f, 
            ConnectedVertices.ConnectionMethod.Solid, new Color(0.5f, 0f, 0.5f, PLANES_ALPHA), 
            new Vector4(1, 0, 1, 0) * PLANES_SCALE),

        // YZ
        new Tesseract(new Vector4(0, 1, 1, 0) * PLANES_SCALE / 2f, 
            ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 0.5f, 0.5f, PLANES_ALPHA), 
            new Vector4(0, 1, 1, 0) * PLANES_SCALE),

        // XW
        new Tesseract(new Vector4(1, 0, 0, 1) * PLANES_SCALE / 2f,
            ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0.5f, 0.5f, PLANES_ALPHA), 
            new Vector4(1, 0, 0, 1) * PLANES_SCALE),

        // YW
        new Tesseract(new Vector4(0, 1, 0, 1) * PLANES_SCALE / 2f, 
            ConnectedVertices.ConnectionMethod.Solid, new Color(0.5f, 1f, 0.5f, PLANES_ALPHA), 
            new Vector4(0, 1, 0, 1) * PLANES_SCALE),

        // ZW
        new Tesseract(new Vector4(0, 0, 1, 1) * PLANES_SCALE / 2f,
            ConnectedVertices.ConnectionMethod.Solid, new Color(0.5f, 0.75f, 1f, PLANES_ALPHA), 
            new Vector4(0, 0, 1, 1) * PLANES_SCALE),

        new Cube(new Vector4(0, 0, 0, -0.5f), ConnectedVertices.ConnectionMethod.Wireframe, Color.white, Vector3.one * 0.5f)
    };
    public override List<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
    public override bool IsOrthographic => true;
}
