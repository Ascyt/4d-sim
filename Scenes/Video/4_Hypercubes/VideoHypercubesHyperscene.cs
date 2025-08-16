using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class VideoHypercubesHyperscene : Hyperscene
{
    private List<Hyperobject> _objects = new()
    {
    };
    public override List<Hyperobject> Objects => _objects;

    private List<Hyperobject> _fixedObjects = new()
    {
        new Tesseract(Vector4.zero, ConnectedVertices.ConnectionMethod.Wireframe, Color.white),
        new Cube(new Vector4(0, 0, 0, 0.5f), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0, 1f, 1f / 2f)),
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

        return (null, _fixedObjects);
    }
}
