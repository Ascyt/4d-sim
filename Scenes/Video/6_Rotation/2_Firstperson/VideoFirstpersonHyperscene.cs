using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class VideoRotationFirstpersonHyperscene : Hyperscene
{
    private Cube highlightedCell = new Cube(new Vector4(0, 0, 0, 0.5f), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0, 1f, 0f));

    private HashSet<Hyperobject> _objects = new()
    {
        new Tesseract(Vector4.zero, ConnectedVertices.ConnectionMethod.Wireframe, Color.white, Vector4.one),
    };
    public override HashSet<Hyperobject> Objects => _objects;

    private HashSet<Hyperobject> _fixedObjects = new()
    {
    };
    public override HashSet<Hyperobject> FixedObjects => _fixedObjects;
    public override Vector4 StartingPosition => new(0, 0, 0, -3f);

    public override void Start()
    {
        _objects.Add(highlightedCell);
    }

    public override (HashSet<Hyperobject>?, HashSet<Hyperobject>?) Update()
    {
        highlightedCell.connectedVertices[0].color = VideoRotationFirstpersonHypersceneInteractivity.Instance.highlightedCellColor;
        return (new HashSet<Hyperobject> { highlightedCell }, null);
    }
}
