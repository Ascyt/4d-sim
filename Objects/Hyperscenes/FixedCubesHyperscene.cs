using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCubesHyperscene : Hyperscene
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

    public FixedCubesHyperscene()
    {
        const int n = 6;

        for (int i = 0; i < n; i++)
        {
            float w = ((i / (float)n * 2) - 0.5f);
            if (!IsOrthographic)
                w /= 2;

            _fixedObjects.Add(
                new Cube(new Vector4(0, 0, 0, w), ConnectedVertices.ConnectionMethod.Wireframe, new Color(i / (float)n, i / (float)n, 1)));
        }
    }
}
