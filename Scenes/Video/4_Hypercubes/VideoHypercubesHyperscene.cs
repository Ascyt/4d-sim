using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class VideoHypercubesHyperscene : Hyperscene
{
    private readonly List<Hyperobject> sideFaces = new() {
        new Tesseract(new Vector4(0, .5f, 0, 0),  ConnectedVertices.ConnectionMethod.Solid, Color.black, new Vector4(1f, 0, 1f, 1f)),
        new Tesseract(new Vector4(.5f, 0, 0, 0),  ConnectedVertices.ConnectionMethod.Solid, Color.black, new Vector4(0, 1f, 1f, 1f)),
        new Tesseract(new Vector4(0, -.5f, 0, 0), ConnectedVertices.ConnectionMethod.Solid, Color.black, new Vector4(1f, 0, 1f, 1f)),
        new Tesseract(new Vector4(-.5f, 0, 0, 0), ConnectedVertices.ConnectionMethod.Solid, Color.black, new Vector4(0, 1f, 1f, 1f)),
        new Tesseract(new Vector4(0, 0, .5f, 0),  ConnectedVertices.ConnectionMethod.Solid, Color.black, new Vector4(1f, 1f, 0, 1f)),
        new Tesseract(new Vector4(0, 0, -.5f, 0), ConnectedVertices.ConnectionMethod.Solid, Color.black, new Vector4(1f, 1f, 0, 1f)),
    };

    private List<Hyperobject> _objects = new()
    {
    };
    public override List<Hyperobject> Objects => _objects;

    private List<Hyperobject> _fixedObjects = new()
    {
        new Tesseract(Vector4.zero, ConnectedVertices.ConnectionMethod.Wireframe, Color.white),
        new Cube(new Vector4(0, 0, 0, .5f), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0, 1f, 1f / 2f)),

        // Side faces
    };
    public override List<Hyperobject> FixedObjects => _fixedObjects;

    public override bool IsFixed => true;

    public override (List<Hyperobject>?, List<Hyperobject>?) Update()
    {
        foreach (Hyperobject obj in _fixedObjects)
        {
            obj.Rotation = VideoHypercubesHypersceneInteractivity.Instance.tesseractRotation;
            obj.position = VideoHypercubesHypersceneInteractivity.Instance.tesseractRotation * obj.startPosition;
        }

        for (int i = 0; i < sideFaces.Count; i++)
        {
            sideFaces[i].connectedVertices[0].color = VideoHypercubesHypersceneInteractivity.Instance.highlightedCellColors[i];
        }

        return (null, _fixedObjects);
    }

    public override void Start()
    {
        _fixedObjects.AddRange(sideFaces);
    }
}
