using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Rendering : MonoBehaviour
{
    public struct InstantiatedObject
    {
        public GameObject gameObj;
        public List<Object> resources;
        public InstantiatedObject(GameObject gameObj, List<Object> resources)
        {
            this.gameObj = gameObj; this.resources = resources; 
        }
    }

    public void Initialize(float fov)
    {
        this.fov = fov;
    }
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

    public void ProjectVertices(ConnectedVertices connectedVertices, Hyperobject obj, Vector4 from, Vector4 to, Vector4 up, Vector4 over, Vector4 wa, Vector4 wb, Vector4 wc, Vector4 wd)
    {
        Vector4?[] movedVertices = new Vector4?[connectedVertices.vertices.Length];
        for (int i = 0; i < connectedVertices.vertices.Length; i++)
        {
            movedVertices[i] = obj.position + connectedVertices.vertices[i];
            if (Helpers.IsPointBehindHyperplane(from, to - from, movedVertices[i].Value))
                movedVertices[i] = null;
        }

        // Project the vertices to 3D
        Vector3?[] transformedVertices = Helpers.ProjectVerticesTo3d(from, wa, wb, wc, wd, movedVertices, fov);

        if (transformedVertices.All(v => !v.HasValue))
        {
            return;
        }

        transformedVertices = transformedVertices
            .Where(v => v.HasValue)
            .ToArray();

        Vector3 averagePos = new Vector3(
            transformedVertices.Select(v => v.Value.x).Average(),
            transformedVertices.Select(v => v.Value.y).Average(),
            transformedVertices.Select(v => v.Value.z).Average());

        transformedVertices = transformedVertices.Select(v => v - averagePos).ToArray();

        DisplayObject(connectedVertices, obj, transformedVertices, averagePos); 
    }

    public void ProjectFixedObject(ConnectedVertices connectedVertices, Hyperobject obj)
    {
        Vector3[] transformedVertices = connectedVertices.vertices
            .Select(v => new Vector3(v.x, v.y, v.z)) // orthographic projection by cutting away w coordinate
            .ToArray();

        Vector3?[] transformedVerticesNullable = new Vector3?[transformedVertices.Length];
        for (int i = 0; i < transformedVertices.Length; i++)
        {
            transformedVerticesNullable[i] = transformedVertices[i];
        }

        DisplayObject(connectedVertices, obj, transformedVerticesNullable, Vector3.zero);
    }

    private void DisplayObject(ConnectedVertices connectedVertices, Hyperobject obj, Vector3?[] transformedVertices, Vector3 averagePos)
    {
        (GameObject, List<Object>)? instance = ObjectInstantiator.instance
          .InstantiateObject(transformedVertices, averagePos, connectedVertices.connectionMethod, connectedVertices.color, connectedVertices.connections, connectedVertices.vertexScale);

        if (instance != null)
        {
            InstantiatedObject instantiatedObject = new InstantiatedObject(instance.Value.Item1, instance.Value.Item2);

            if (instantiatedObjects.ContainsKey(obj))
                instantiatedObjects[obj].Add(instantiatedObject);
            else
                instantiatedObjects.Add(obj, new List<InstantiatedObject> { instantiatedObject });
        }
    }
}
