using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ObjectInstantiator;

/// <summary>
/// Rendering class for projecting and displaying 4D objects from a camera (incl. from, to, up, over) in a 3D space.
/// </summary>
public class Rendering : MonoBehaviour
{
    public void Initialize(Vector4 from, Vector4 to, Vector4 up, Vector4 over, float fov)
    {
        Helpers.GetViewingTransformMatrix(from, to, up, over, out wa, out wb, out wc, out wd);
        this.from = from;
        this.fov = fov;
    }
    private Vector4 wa;
    private Vector4 wb;
    private Vector4 wc;
    private Vector4 wd;
    private Vector4 from;
    private float fov;

    private static readonly Dictionary<Hyperobject, List<InstantiatedObject>> instantiatedObjects = new();

    public void ClearAllRenderedObjects()
    {
        foreach (List<InstantiatedObject> instantiatedObjectValues in instantiatedObjects.Values)
        {
            foreach (InstantiatedObject instantiatedObject in instantiatedObjectValues) 
            {
                Destroy(instantiatedObject.gameObj);
                foreach (Object resource in instantiatedObject.resources)
                    Destroy(resource); 
            }
        }
        instantiatedObjects.Clear();
    }

    public void ProjectVertices(ConnectedVertices connectedVertices, Hyperobject obj, Vector4[] verticesRelativeToCamera, bool allowIntersectioning = true,
        Vector4? wa = null, Vector4? wb = null, Vector4? wc = null, Vector4? wd = null, Vector4? from=null)
    {
        bool applyIntersectioning = allowIntersectioning && connectedVertices.connections is not null &&
                (new[] { ConnectedVertices.ConnectionMethod.Solid, ConnectedVertices.ConnectionMethod.Wireframe })
                .Contains(connectedVertices.connectionMethod);

        int[][] connections = connectedVertices.connections;
        if (applyIntersectioning)
        {
            Helpers.ApplyIntersectioning(ref verticesRelativeToCamera, ref connections);
        }

        // Project the vertices to 3D
        Vector3?[] transformedVertices = Helpers.ProjectVerticesTo3d(wa ?? this.wa, wb ?? this.wb, wc ?? this.wc, wd ?? this.wd, from ?? this.from, verticesRelativeToCamera, fov);

        Vector3 averagePos = Vector3.zero;
        int valueCount = 0;
        for (int i = 0; i < transformedVertices.Length; i++)
        {
            if (transformedVertices[i].HasValue)
            {
                valueCount++;
                averagePos += transformedVertices[i].Value;
            }
        }
        if (valueCount == 0)
        {
            // No vertices are in front of the camera, so we don't need to display anything.
            return;
        }

        averagePos /= valueCount; // average position of all vertices in front of the camera

        transformedVertices = transformedVertices.Select(v => v - averagePos).ToArray();

        DisplayObject(connectedVertices, obj, transformedVertices, averagePos, connections);
    }

    public void ProjectFixedVertices(ConnectedVertices connectedVertices, Hyperobject obj, Vector4[] vertices, bool orthographic)
    {
        if (orthographic)
        {
            Vector3?[] transformedVertices = vertices
                .Select(v => (Vector3?)new Vector3(v.x, v.y, v.z)) // orthographic projection by cutting away w coordinate
                .ToArray();

            DisplayObject(connectedVertices, obj, transformedVertices, Vector3.zero, connectedVertices.connections);
            return;
        }

        Vector4 fixedFrom = new(0, 0, 0, 2);
        Vector4 fixedTo = new(0, 0, 0, -1);
        Vector4 fixedUp = new(0, 1, 0, 0);
        Vector4 fixedOver = new(0, 0, 1, 0);
        Helpers.GetViewingTransformMatrix(fixedFrom, fixedTo, fixedUp, fixedOver, out Vector4 fixedWa, out Vector4 fixedWb, out Vector4 fixedWc, out Vector4 fixedWd);

        ProjectVertices(connectedVertices, obj, vertices, false, fixedWa, fixedWb, fixedWc, fixedWd, fixedFrom);
    }

    private void DisplayObject(ConnectedVertices connectedVertices, Hyperobject obj, Vector3?[] transformedVertices, Vector3 averagePos, int[][] connections)
    {
        InstantiatedObject? instance = ObjectInstantiator.instance
          .InstantiateObject(transformedVertices, averagePos, connectedVertices.connectionMethod, connectedVertices.color, connections, connectedVertices.vertexScale);

        if (instance != null)
        {
            if (instantiatedObjects.ContainsKey(obj))
                instantiatedObjects[obj].Add(instance.Value);
            else
                instantiatedObjects.Add(obj, new List<InstantiatedObject> { instance.Value });
        }
    }
}
