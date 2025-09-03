using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffordTorus : Hyperobject
{
    private const float DELTA = Helpers.TAU / 16f;

    public CliffordTorus(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector4? scale = null, int? resolution=null) : base(new ConnectedVertices[]
    {
        GetConnectedVertices(connectionMethod, color, scale, resolution)
    }, position)
    {

    }
    public static ConnectedVertices GetConnectedVertices(ConnectedVertices.ConnectionMethod connectionMethod, Color color, Vector4? scale = null, int? resolution=null)
        => new(
            connectionMethod,

            GetVertices(scale ?? Vector4.one, resolution ?? 16),

            color,

            connections: GetConnections(resolution ?? 16)
        );

    private static Vector4[] GetVertices(Vector4 scale, int resolution)
    {
        float delta = Helpers.TAU / resolution;

        List<Vector4> vertices = new();
        for (float f1 = 0; f1 < Helpers.TAU; f1+=delta)
        {
            for (float f2 = 0; f2 < Mathf.PI * 2f; f2 += delta)
            {
                vertices.Add(new Vector4(
                    scale.x * Mathf.Cos(f1),
                    scale.y * Mathf.Sin(f1),
                    scale.z * Mathf.Cos(f2),
                    scale.w * Mathf.Sin(f2)
                ));
            }
        }

        return vertices.ToArray(); 
    }

    private static int[][] GetConnections(int resolution)
    {
        List<int[]> connections = new();
        int pointsPerCircle = resolution;
        int totalPoints = pointsPerCircle * pointsPerCircle;
        for (int i = 0; i < totalPoints; i++)
        {
            // Connect to next point in the same circle
            connections.Add(new int[] { i, (i + 1) % pointsPerCircle + (i / pointsPerCircle) * pointsPerCircle });
            // Connect to corresponding point in the next circle
            connections.Add(new int[] { i, (i + pointsPerCircle) % totalPoints });
        }
        return connections.ToArray();
    }
}
