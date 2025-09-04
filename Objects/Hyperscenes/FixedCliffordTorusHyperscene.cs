using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

/// <summary>
/// Includes a Clifford Torus that can be extruded from a cylinder using the scene slider.
/// </summary>
public class FixedCliffordTorusHyperscene : Hyperscene
{
    private readonly CliffordTorus extrudingObject = new(
        Vector4.zero,
        ConnectedVertices.ConnectionMethod.Wireframe,
        Color.white,
        scale: new Vector4(0f, 1f, 1f, 1f) / 1.125f);

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

    public FixedCliffordTorusHyperscene()
    {
        _fixedObjects.Add(extrudingObject);
    }

    public override bool ShowSceneSlider => true;
    public override (HashSet<Hyperobject>?, HashSet<Hyperobject>?) OnSceneSliderUpdate(float value)
    {
        extrudingObject.connectedVertices = new ConnectedVertices[]
        {
            CliffordTorus.GetConnectedVertices(ConnectedVertices.ConnectionMethod.Wireframe, extrudingObject.connectedVertices[0].color, new Vector4(value, 1f, 1f, 1f) / 1.125f)
        };

        return (null, new() { extrudingObject });
    }
}
