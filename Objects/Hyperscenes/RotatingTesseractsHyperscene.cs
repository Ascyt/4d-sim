using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

/// <summary>
/// Includes a bunch of tesseracts, cubes and points.
/// </summary>
public class RotatingTesseractsHyperscene : Hyperscene
{
    private const float EPSILON = 1f / 4096f;

    private static readonly Vector4 singleRotationTesseractAPosition = new(-1f, -1f, 0, 0);
    private readonly HashSet<Hyperobject> singleRotationATesseract = new()
    {
        new Tesseract(singleRotationTesseractAPosition, ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta),
        new Tesseract(singleRotationTesseractAPosition - new Vector4(.125f, 0, 0, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0f, 0f), new Vector4(.25f, .5f, EPSILON, EPSILON)),
        new Tesseract(singleRotationTesseractAPosition + new Vector4(.125f, 0, 0, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 1f, 0f), new Vector4(.25f, .5f, EPSILON, EPSILON)),
    };
    private static readonly Vector4 singleRotationTesseractBPosition = new(-1f, 1f, 0, 0);
    private readonly HashSet<Hyperobject> singleRotationBTesseract = new()
    {
        new Tesseract(singleRotationTesseractBPosition, ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta),
        new Tesseract(singleRotationTesseractBPosition - new Vector4(0, 0, .125f, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(0f,.5f, 1f), new Vector4(EPSILON, EPSILON, .25f, .5f)),
        new Tesseract(singleRotationTesseractBPosition + new Vector4(0, 0, .125f, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 1f, 0f), new Vector4(EPSILON, EPSILON, .25f, .5f)),
    };
    private static readonly Vector4 doubleRotationTesseractPosition = new(1f, 0, 0);
    private readonly HashSet<Hyperobject> doubleRotationTesseract = new()
    {
        new Tesseract(doubleRotationTesseractPosition, ConnectedVertices.ConnectionMethod.Wireframe, Color.cyan),
        new Tesseract(doubleRotationTesseractPosition - new Vector4(.125f, 0, 0, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 0f, 0f), new Vector4(.25f, .5f, EPSILON, EPSILON)),
        new Tesseract(doubleRotationTesseractPosition + new Vector4(.125f, 0, 0, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(0f, 1f, 0f), new Vector4(.25f, .5f, EPSILON, EPSILON)),
        new Tesseract(doubleRotationTesseractPosition - new Vector4(0, 0, .125f, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(0f,.5f, 1f), new Vector4(EPSILON, EPSILON, .25f, .5f)),
        new Tesseract(doubleRotationTesseractPosition + new Vector4(0, 0, .125f, 0), ConnectedVertices.ConnectionMethod.Solid, new Color(1f, 1f, 0f), new Vector4(EPSILON, EPSILON, .25f, .5f)),
    };


    private readonly HashSet<Hyperobject> _objects = new()
    {
    };
    public override HashSet<Hyperobject> Objects => _objects;

    private readonly HashSet<Hyperobject> _fixedObjects = new()
    {
        new Axes()
    };
    public override HashSet<Hyperobject> FixedObjects => _fixedObjects;
    public override void Start()
    {
        _objects.UnionWith(singleRotationATesseract);
        _objects.UnionWith(doubleRotationTesseract);
    }
    public override Vector4 StartingPosition => new Vector4(0, 0, 0, -5f);
    public override (HashSet<Hyperobject>?, HashSet<Hyperobject>?) Update()
    {
        float speed = Time.deltaTime * Helpers.TAU / 8f;

        Quatpair singleRotationADelta = new(0, 0, 0, speed, 0, 0); // XY
        foreach (Hyperobject obj in singleRotationATesseract)
        {
            obj.RotateAroundPoint(singleRotationADelta, singleRotationTesseractAPosition, worldSpace:false);
        }
        Quatpair singleRotationBDelta = new(0, 0, speed, 0, 0, 0); // ZW
        foreach (Hyperobject obj in singleRotationBTesseract)
        {
            obj.RotateAroundPoint(singleRotationBDelta, singleRotationTesseractBPosition, worldSpace:false);
        }


        Quatpair doubleRotationDelta = new(0, 0, speed, speed, 0, 0); // XY, ZW
        foreach (Hyperobject obj in doubleRotationTesseract)
        {
            obj.RotateAroundPoint(doubleRotationDelta, doubleRotationTesseractPosition, worldSpace: false);
        }

        HashSet<Hyperobject> rerenderObjects = new();
        rerenderObjects.UnionWith(singleRotationATesseract);
        rerenderObjects.UnionWith(singleRotationBTesseract);
        rerenderObjects.UnionWith(doubleRotationTesseract);

        return (rerenderObjects, null);
    }
}
