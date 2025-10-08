using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

/// <summary>
/// Includes a Clifford Torus that can be extruded from a cylinder using the scene slider.
/// </summary>
public class FixedDuoPrismHyperscene : Hyperscene
{
    private const int SCHLAEFLI_P = 3;

    private readonly DuoPrism obj = new(
        Vector4.zero,
        ConnectedVertices.ConnectionMethod.Wireframe,
        Color.white,
        schlaefliP: SCHLAEFLI_P,
        schlaefliQ: 2,
        scale: Vector4.one / 1.125f);

    private readonly HashSet<Hyperobject> _objects = new()
    {
    };
    public override HashSet<Hyperobject> Objects => _objects;

    private readonly HashSet<Hyperobject> _fixedObjects = new()
    {
        new Axes()
    };
    public override HashSet<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;
    public override bool IsOrthographic => false;

    public FixedDuoPrismHyperscene()
    {
        _fixedObjects.Add(obj);
    }

    public override int SceneSliderCount => 2;
    public override (HashSet<Hyperobject>?, HashSet<Hyperobject>?) OnSceneSlidersUpdate(float[] values)
    {
        obj.connectedVertices = new ConnectedVertices[]
        {
            DuoPrism.GetConnectedVertices(ConnectedVertices.ConnectionMethod.Wireframe, obj.connectedVertices[0].color, 
            2 + (int)(values[0] * 6f), 2 + (int)(values[1] * 6f), 
            Vector4.one / 1.125f)
        };

        return (null, new() { obj });
    }
}
