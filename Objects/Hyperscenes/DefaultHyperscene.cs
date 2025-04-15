using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultHyperscene : Hyperscene
{
    private List<Hyperobject> _objects = new()
    {
        new Axes(),

        new Point(new Vector4(0, 0, 0, 0), Color.white),

        new Tesseract(new Vector4(2, 0, 1, 2), ConnectedVertices.ConnectionMethod.Solid, Color.cyan, 1f),
        new Cube(new Vector4(0, -1, 2, 4), ConnectedVertices.ConnectionMethod.Solid, Color.yellow),
        
        new Tesseract(new Vector4(3, 0, -2, 3), ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta, 1f),
        
        new Cube(new Vector4(0, -1, 2, -4), ConnectedVertices.ConnectionMethod.Wireframe, Color.green),
        
        new Tesseract(new Vector4(-10, 5, 6, -3), ConnectedVertices.ConnectionMethod.Wireframe, Color.red, 3f),
        new Tesseract(new Vector4(-10, 5, 6, -7), ConnectedVertices.ConnectionMethod.Wireframe, Color.yellow, .5f),
    };
    public override List<Hyperobject> Objects => _objects;

    public override bool ShowFixedAxes => true;
}
