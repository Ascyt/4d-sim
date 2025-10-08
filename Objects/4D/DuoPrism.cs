using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DuoPrism : Hyperobject
{
    public DuoPrism(Vector4 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, int schlaefliP, int schlaefliQ, Vector4? scale = null) : base(new ConnectedVertices[]
    {
        GetConnectedVertices(connectionMethod, color, schlaefliP, schlaefliQ, scale)
    }, position)
    {

    }
    public static ConnectedVertices GetConnectedVertices(ConnectedVertices.ConnectionMethod connectionMethod, Color color, int schlaefliP, int schlaefliQ, Vector4? scale = null)
        => new(
            connectionMethod,

            GetVertices(scale ?? Vector4.one, schlaefliP, schlaefliQ),

            color,

            connections: GetConnections(schlaefliP, schlaefliQ)
        );

    private static Vector4[] GetVertices(Vector4 scale, int schlaefliP, int schlaefliQ)
    {
        int P = schlaefliP;
        int Q = schlaefliQ;
        Vector4[] verts = new Vector4[P * Q];

        float deltaP = Helpers.TAU / P;
        float deltaQ = Helpers.TAU / Q;

        int idx = 0;
        for (int p = 0; p < P; p++)
        {
            float angleP = p * deltaP;

            for (int q = 0; q < Q; q++)
            {
                float angleQ = q * deltaQ;
                verts[idx] = new Vector4(
                    scale.x * Mathf.Cos(angleP),
                    scale.y * Mathf.Sin(angleP),
                    scale.z * Mathf.Cos(angleQ),
                    scale.w * Mathf.Sin(angleQ)
                );
                idx++;
            }
        }

        return verts;
    }

    private static int[][] GetConnections(int schlaefliP, int schlaefliQ)
    {
        int P = schlaefliP;
        int Q = schlaefliQ;
        int totalEdges = 2 * P * Q; // P*Q along P-direction and P*Q along Q-direction
        int[][] edges = new int[totalEdges][];
        int e = 0;

        for (int p = 0; p < P; p++)
        {
            for (int q = 0; q < Q; q++)
            {
                int idx = p * Q + q;
                int nextP = ((p + 1) % P) * Q + q;    // neighbor along the P-cycle (same q)
                int nextQ = p * Q + ((q + 1) % Q);    // neighbor along the Q-cycle (same p)

                edges[e++] = new int[] { idx, nextP };
                edges[e++] = new int[] { idx, nextQ };
            }
        }

        return edges;
    }
}
