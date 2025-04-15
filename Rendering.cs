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

    public void ProjectVertices(ConnectedVertices connectedVertices, Hyperobject obj, Vector4[] verticesRelativeToCamera)
    {
        if (verticesRelativeToCamera.All(v => v.w <= 0))
        {
            return;
        }

        bool applyIntersectioning = connectedVertices.connections is not null &&
                (new[] { ConnectedVertices.ConnectionMethod.Solid, ConnectedVertices.ConnectionMethod.Wireframe })
                .Contains(connectedVertices.connectionMethod);

        if (applyIntersectioning)
        {
            Helpers.ApplyIntersectioning(ref verticesRelativeToCamera, ref connectedVertices.connections);
        }

        // Project the vertices to 3D
        Vector3?[] transformedVertices = Helpers.ProjectVerticesTo3d(wa, wb, wc, wd, from, verticesRelativeToCamera, fov);

        if (transformedVertices.All(v => !v.HasValue))
        {
            return;
        }

        Vector3 averagePos = new Vector3(
            transformedVertices.Where(v => v.HasValue).Select(v => v.Value.x).Average(),
            transformedVertices.Where(v => v.HasValue).Select(v => v.Value.y).Average(),
            transformedVertices.Where(v => v.HasValue).Select(v => v.Value.z).Average());

        transformedVertices = transformedVertices.Select(v => v - averagePos).ToArray();
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
