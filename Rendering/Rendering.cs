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
    private readonly Vector4 from = new Vector4(0, 0, 0, 0);
    private readonly Vector4 to = new Vector4(0, 0, 0, -1);
    private readonly Vector4 up = new Vector4(0, 1, 0, 0);
    private readonly Vector4 over = new Vector4(0, 0, 1, 0);
    private Vector4 wa;
    private Vector4 wb;
    private Vector4 wc;
    private Vector4 wd;

    public float fov;

    public static readonly Dictionary<Hyperobject, List<InstantiatedObject>> instantiatedObjects = new();

    private const float ORTHOGRAPHIC_SCALE = 4f;

    public void Initialize()
    {
        Helpers.GetViewingTransformMatrix(from, to, up, over, out wa, out wb, out wc, out wd);
    }

    public void ClearAllRenderedObjects()
    {
        foreach (List<InstantiatedObject> instantiatedObjectValues in instantiatedObjects.Values)
        {
            foreach (InstantiatedObject instantiatedObject in instantiatedObjectValues) 
            {
                RemoveSingleObject(instantiatedObject);
            }
        }
        instantiatedObjects.Clear();
    }
    private void RemoveSingleObject(InstantiatedObject instantiatedObject)
    {
        Destroy(instantiatedObject.gameObj);
        foreach (Object resource in instantiatedObject.resources)
            Destroy(resource);
    }
    public bool RemoveSingleObject(Hyperobject obj)
    {
        if (!instantiatedObjects.TryGetValue(obj, out List<InstantiatedObject> iObjs))
        {
            return false;
        }

        foreach (InstantiatedObject iObj in iObjs)
        {
            RemoveSingleObject(iObj);
        }

        instantiatedObjects.Remove(obj);

        return true;
    }

    public void ProjectVertices(ConnectedVertices connectedVertices, Hyperobject obj, Quatpair cameraRotation, Vector4 cameraPosition, bool allowIntersectioning = true)
    {
        if (!connectedVertices.isEnabled || connectedVertices.vertices.Length == 0)
            return;

        bool applyIntersectioning = allowIntersectioning && connectedVertices.connections is not null &&
                (new[] { ConnectedVertices.ConnectionMethod.Solid, ConnectedVertices.ConnectionMethod.Wireframe })
                .Contains(connectedVertices.connectionMethod);

        int[][] connections = connectedVertices.connections;
        Vector4[] verticesRelativeToCamera = connectedVertices.vertices
            .Select(v => (v + obj.position - cameraPosition).ApplyRotation(Quatpair.Inverse(cameraRotation)))
            .ToArray();
        if (applyIntersectioning)
        {
            Helpers.ApplyIntersectioning(ref verticesRelativeToCamera, ref connections);
        }

        // Project the vertices to 3D
        Vector3?[] transformedVertices = Helpers.ProjectVerticesTo3d(wa, wb, wc, wd, from, verticesRelativeToCamera, fov);

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

    public void ProjectFixedVertices(ConnectedVertices connectedVertices, Hyperobject obj, Quatpair objectRotation, bool orthographic)
    {
        if (!connectedVertices.isEnabled || connectedVertices.vertices.Length == 0)
            return;

        Vector4[] transformedVertices = connectedVertices.vertices
            .Select(v => (v + obj.position).ApplyRotation(Quatpair.Inverse(objectRotation)))
            .ToArray();

        Vector3?[] projectedVertices;
        if (orthographic)
        {
            // Orthographic projection by cutting away w coordinate
            projectedVertices = transformedVertices
                .Select(v => (Vector3?)(new Vector3(v.x, v.y, v.z) * ORTHOGRAPHIC_SCALE))
                .ToArray();
        }
        else
        {
            projectedVertices = Helpers.ProjectVerticesTo3d(wa, wb, wc, wd, new Vector4(0, 0, 0, -2), transformedVertices, fov);
        }

        DisplayObject(connectedVertices, obj, projectedVertices, Vector3.zero, connectedVertices.connections);
    }

    private void DisplayObject(ConnectedVertices connectedVertices, Hyperobject obj, Vector3?[] projectedVertices, Vector3 averagePos, int[][] connections)
    {
        InstantiatedObject? instance = ObjectInstantiator.instance
          .InstantiateObject(projectedVertices, averagePos, connectedVertices.connectionMethod, connectedVertices.color, connections, connectedVertices.vertexScale);

        if (instance != null)
        {
            if (instantiatedObjects.ContainsKey(obj))
                instantiatedObjects[obj].Add(instance.Value);
            else
                instantiatedObjects.Add(obj, new List<InstantiatedObject> { instance.Value });
        }
    }
}
