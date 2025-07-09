using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

/// <summary>
/// Includes a bunch of tesseracts, cubes and points.
/// </summary>
public class DefaultHyperscene : Hyperscene
{
    private static readonly Vector4 transformingTesseractPosition = new Vector4(-3, 1, -3, 2);
    private Tesseract transformingTesseract = new Tesseract(transformingTesseractPosition, ConnectedVertices.ConnectionMethod.Wireframe, Color.white);
    private Cube transformingTesseractFace = new Cube(transformingTesseractPosition + new Vector4(0, 0, 0, 0.5f), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 1f, 0f, 0.75f));

    private List<Hyperobject> _objects = new()
    {
        new Axes(0.5f),

        new Point(new Vector4(0, 0, 0, 0), Color.white),

        new Tesseract(new Vector4(2, 0, 1, 2), ConnectedVertices.ConnectionMethod.Solid, Color.cyan),
        new Cube(new Vector4(0, -1, 2, 4), ConnectedVertices.ConnectionMethod.Solid, Color.yellow),

        new Tesseract(new Vector4(3, 0, -2, 3), ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta),

        new Cube(new Vector4(0, -1, 2, -4), ConnectedVertices.ConnectionMethod.Wireframe, Color.green),

        new Tesseract(new Vector4(-10, 5, 6, -3), ConnectedVertices.ConnectionMethod.Wireframe, Color.red, scale: new Vector4(1f, 2f, 3f, 4f)),
        new Tesseract(new Vector4(-10, 5, 6, -7), ConnectedVertices.ConnectionMethod.Wireframe, Color.yellow)
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
    public override bool Update()
    {
        Vector4 averagePosition = Vector4.zero;
        int count = 0;
        bool anyInView = false;
        foreach (ConnectedVertices connectedVertices in transformingTesseract.vertices)
        {
            averagePosition += connectedVertices.transformedVertices
                .Aggregate((a, b) => a + b);

            if (connectedVertices.transformedVertices.Any(v => v.w > 0f))
                anyInView = true;

            count += connectedVertices.transformedVertices.Length;
        }
        averagePosition /= count;

        _ = TransformConnectedVertices(transformingTesseract.vertices, averagePosition);
        prevTranslationValue = TransformConnectedVertices(transformingTesseractFace.vertices, averagePosition);

        return anyInView;
    }
    private float prevTranslationValue = 0f;
    private float TransformConnectedVertices(ConnectedVertices[] connectedVertices, Vector4 averagePosition)
    {
        // TODO: Use rotation instead of translation when quaternion rotation implemented (rotation should be relative to world not to camera)

        //float speed = Time.deltaTime * 2 * Mathf.PI / 10f;

        //Rotation4 rotation = new Rotation4(speed, 0, 0, 0, 0, 0);
        //foreach (ConnectedVertices vertices in connectedVertices)
        //{
        //    vertices.transformedVertices = vertices.transformedVertices
        //        .Select(v => (v - averagePosition).Rotate(rotation) + averagePosition)
        //        .ToArray();
        //}

        float translationValue = Mathf.Sin(Time.time);
        Vector4 translationAmount = CameraState.instance.ana;
        Vector4 translation = translationAmount * translationValue;

        foreach (ConnectedVertices vertices in connectedVertices)
        {
            vertices.transformedVertices = vertices.transformedVertices
                .Select(v => v - (translationAmount * prevTranslationValue) + (translationAmount * translationValue))
                .ToArray();
        }

        return translationValue;
    }
}
