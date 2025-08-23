using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

/// <summary>
/// Orthographic scene that includes a bunch of cubes, each with a different w offset to make them appear like a sliced tesseract.
/// </summary>
public class FixedExtrudingCubeHyperscene : Hyperscene
{
    private readonly Tesseract extrudingObject = new(
        Vector4.zero,
        ConnectedVertices.ConnectionMethod.Wireframe,
        Color.white,
        scale: new Vector4(0f, 1f, 1f, 1f));

    private readonly Tesseract highlightedCell = new(
        Vector4.zero,
        ConnectedVertices.ConnectionMethod.Solid,
        new Color(1f, 0f, 1f, 1f / 4f),
        scale: new Vector4(1f / 4096f, 1f, 1f, 1f));

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
    public override bool IsOrthographic => true;

    public FixedExtrudingCubeHyperscene()
    {
        _fixedObjects.Add(extrudingObject);
        _fixedObjects.Add(highlightedCell);
    }

    public override bool ShowSceneSlider => true;
    public override (HashSet<Hyperobject>?, HashSet<Hyperobject>?) OnSceneSliderUpdate(float value)
    {
        extrudingObject.connectedVertices = new ConnectedVertices[] 
        { 
            Tesseract.GetConnectedVertices(ConnectedVertices.ConnectionMethod.Wireframe, extrudingObject.connectedVertices[0].color, new Vector4(value, 1f, 1f, 1f)) 
        };
        highlightedCell.position = new Vector4(value / 2f, 0f, 0f, 0f);

        return (null, new() { extrudingObject, highlightedCell });
    }
    //public override (HashSet<Hyperobject>?, HashSet<Hyperobject>?) Update()
    //{
    //    float speed = Time.deltaTime * 2 * Mathf.PI / 4f *0;

    //    Quatpair rotationDelta = new(0, 0, 0, speed, 0, 0);

    //    extrudingObject.RotateAroundPoint(rotationDelta, Vector4.zero, worldSpace: false);
    //    highlightedCell.RotateAroundPoint(rotationDelta, Vector4.zero, worldSpace: false);

    //    return (null, new() { extrudingObject, highlightedCell });
    //}
}
