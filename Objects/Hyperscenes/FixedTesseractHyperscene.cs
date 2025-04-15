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
        new Tesseract(new Vector4(0, 0, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.white, 1f),
    };
    public override List<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
}
