using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axes : Hyperobject
{
    public Axes() : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Wireframe,

            new Vector4[] {
                Vector4.zero,
                new Vector4(1, 0, 0, 0),
            },

            Color.red,

            connections: new int[][]
                { new[] { 0, 1 } },

            vertexScale: 1f
        ),
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Wireframe,

            new Vector4[] {
                Vector4.zero,
                new Vector4(0, 1, 0, 0),
            },

            Color.green,

            connections: new int[][]
                { new[] { 0, 1 } },

            vertexScale: 1f
        ),
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Wireframe,

            new Vector4[] {
                Vector4.zero,
                new Vector4(0, 0, 1, 0),
            },

            new Color(0f, 0.5f, 1f),

            connections: new int[][]
                { new[] { 0, 1 } },

            vertexScale: 1f
        ),
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Wireframe,

            new Vector4[] {
                Vector4.zero,
                new Vector4(0, 0, 0, 1),
            },

            Color.yellow,

            connections: new int[][]
                { new[] { 0, 1 } },

            vertexScale: 1f
        ),

        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Vertices,

            new Vector4[] {
                new Vector4(0, 0, 0, 0)
            },

            Color.black,

            vertexScale: 2f
        ),
    }, Vector4.zero)
    {

    }
}
