using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedSingleCubeHyperscene : Hyperscene
{
    private List<Hyperobject> _objects = new()
    {
    };
    public override List<Hyperobject> Objects => _objects;

    private List<Hyperobject> _fixedObjects = new()
    {
        new Axes()
    };
    public override List<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
    public override bool IsOrthographic => false;

    public FixedSingleCubeHyperscene()
    {
        _fixedObjects.Add(
            new Cube(Vector4.zero, ConnectedVertices.ConnectionMethod.Wireframe, new Color(1, 1, 1)));
    }
}
