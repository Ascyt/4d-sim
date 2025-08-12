using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Includes a bunch of tesseracts, cubes and points.
/// </summary>
public class DefaultHyperscene : Hyperscene
{
    private static readonly Vector4 transformingTesseractPosition = new Vector4(-3, 1, -3, 2);
    private Tesseract transformingTesseract = new Tesseract(transformingTesseractPosition, ConnectedVertices.ConnectionMethod.Wireframe, Color.white);
    private Cube transformingTesseractFace = new Cube(transformingTesseractPosition + new Vector4(0, 0, 0, 0.5f), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 1f, 0f, 0.75f));

    private static readonly Vector4 coloredTesseractPosition = new Vector4(-3, 3, -3, 3);

    private List<Hyperobject> _objects = new()
    {
        new Point(new Vector4(0, 0, 0, 0), Color.white),

        new Tesseract(new Vector4(0, 10, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.white),
        new Tesseract(new Vector4(1, 10, 0, 0), ConnectedVertices.ConnectionMethod.Wireframe, Color.red, Vector4.one / 2f),
        new Tesseract(new Vector4(0, 10, 1, 0), ConnectedVertices.ConnectionMethod.Wireframe, new Color(0f, 0.5f, 1f), Vector4.one / 2f),
        new Tesseract(new Vector4(0, 10, 0, 1), ConnectedVertices.ConnectionMethod.Wireframe, Color.yellow, Vector4.one / 2f),

        new Tesseract(new Vector4(2, 0, 1, 2), ConnectedVertices.ConnectionMethod.Solid, Color.cyan),
        new Cube(new Vector4(0, -1, 2, 4), ConnectedVertices.ConnectionMethod.Solid, Color.yellow),

        new Tesseract(new Vector4(3, 0, -2, 3), ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta),

        new Cube(new Vector4(0, -1, 2, -4), ConnectedVertices.ConnectionMethod.Wireframe, Color.green),

        new Tesseract(new Vector4(-10, 5, 6, -3), ConnectedVertices.ConnectionMethod.Wireframe, Color.red, scale: new Vector4(1f, 2f, 3f, 4f)),
        new Tesseract(new Vector4(-10, 5, 6, -7), ConnectedVertices.ConnectionMethod.Wireframe, Color.yellow),

        new Tesseract(coloredTesseractPosition, ConnectedVertices.ConnectionMethod.Wireframe, Color.white),
        new Tesseract(coloredTesseractPosition + new Vector4(1, 0, 0, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0f, 0f,   0.5f), new Vector4(0, 1, 1, 1) * 0.5f),
        new Tesseract(coloredTesseractPosition + new Vector4(0, 1, 0, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 1f, 0f,   0.5f), new Vector4(1, 0, 1, 1) * 0.5f),
        new Tesseract(coloredTesseractPosition + new Vector4(0, 0, 1, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 0.5f, 1f, 0.5f), new Vector4(1, 1, 0, 1) * 0.5f),
        new Tesseract(coloredTesseractPosition + new Vector4(0, 0, 0, 1), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 1f, 0f,   0.5f), new Vector4(1, 1, 1, 0) * 0.5f),

        new Pentatope(new Vector4(10, -2, -5, 0), ConnectedVertices.ConnectionMethod.Wireframe, new Color(0, 0.5f, 1f), scale:Vector4.one * 2f),
        new Orthoplex(new Vector4(10, 2, -5, 0), ConnectedVertices.ConnectionMethod.Wireframe, new Color(0, 0.5f, 1f), scale:Vector4.one * 2f),
    };
    public override List<Hyperobject> Objects => _objects;

    private List<Hyperobject> _fixedObjects = new()
    {
        new Axes()
    };
    public override List<Hyperobject> FixedObjects => _fixedObjects;
    public override void Start()
    {
        _objects.Add(transformingTesseract);
        _objects.Add(transformingTesseractFace);
    }
    public override List<Hyperobject> Update()
    {
        TransformConnectedVertices(transformingTesseract.vertices, Vector4.zero);
        TransformConnectedVertices(transformingTesseractFace.vertices, transformingTesseractFace.position - transformingTesseract.position);

        return new List<Hyperobject>() { transformingTesseract, transformingTesseractFace };
    }
    private void TransformConnectedVertices(ConnectedVertices[] connectedVertices, Vector4 positionDelta)
    {
        float speed = Time.deltaTime * 2 * Mathf.PI / 4f;

        Quatpair rotation = new Quatpair(speed, 0, 0, 0, 0, 0);
        foreach (ConnectedVertices vertices in connectedVertices)
        {
            vertices.vertices = vertices.vertices
                .Select(v => (v + positionDelta).ApplyRotation(rotation) - positionDelta)
                .ToArray();
        }
    }
}
