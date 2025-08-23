using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Orthographic scene that includes a bunch of cubes, each with a different w offset to make them appear like a sliced tesseract.
/// </summary>
public class FixedCubesHyperscene : Hyperscene
{
    private HashSet<Hyperobject> _objects = new()
    {
    };
    public override HashSet<Hyperobject> Objects => _objects;

    private HashSet<Hyperobject> _fixedObjects = new()
    {
        new Axes()
    };
    public override HashSet<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
    public override bool IsOrthographic => true;

    public FixedCubesHyperscene()
    {
        const int n = 6;

        for (int i = 0; i < n; i++)
        {
            float w = ((i / (float)n * 2) - 0.5f);
            if (n % 2 == 0)
                w -= 1f / n; // Offset so that the first cube is at w=0

            _fixedObjects.Add(
                new Cube(new Vector4(0, 0, 0, w), ConnectedVertices.ConnectionMethod.Wireframe, new Color(i / (float)n, i / (float)n, 1)));
        }
    }
}
