using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

/// <summary>
/// Includes a Clifford Torus that can be extruded from a cylinder using the scene slider.
/// </summary>
public class FixedDuoprismHyperscene : Hyperscene
{
    private const int SCHLAEFLI_P = 3;

    private readonly Duoprism obj = new(
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

    public FixedDuoprismHyperscene()
    {
        _fixedObjects.Add(obj);
    }

    public override int SceneSliderCount => 2;
    public override (HashSet<Hyperobject>?, HashSet<Hyperobject>?) OnSceneSlidersUpdate(float[] values)
    {
        int schlaefliP = 2 + (int)(values[0] * 6f);
        int schlaefliQ = 2 + (int)(values[1] * 6f);

        obj.connectedVertices = new ConnectedVertices[]
        {
            Duoprism.GetConnectedVertices(ConnectedVertices.ConnectionMethod.Wireframe, obj.connectedVertices[0].color, 
            schlaefliP, schlaefliQ, 
            Vector4.one / 1.125f)
        };

        onSceneTextChange?.Invoke($"{schlaefliP}-{schlaefliQ}");

        return (null, new() { obj });
    }
}
