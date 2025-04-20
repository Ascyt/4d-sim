using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axes : Hyperobject
{
    public Axes(float scale=1f) : base(new ConnectedVertices[]
    {
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Wireframe,

            new Vector4[] {
                Vector4.zero,
                new Vector4(scale, 0, 0, 0),
            },

            Color.red,

            connections: new int[][]
                { new[] { 0, 1 } },

            vertexScale: scale
        ),
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Wireframe,

            new Vector4[] {
                Vector4.zero,
                new Vector4(0, scale, 0, 0),
            },

            Color.green,

            connections: new int[][]
                { new[] { 0, 1 } },

            vertexScale: scale
        ),
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Wireframe,

            new Vector4[] {
                Vector4.zero,
                new Vector4(0, 0, scale, 0),
            },

            new Color(0f, 0.5f, 1f),

            connections: new int[][]
                { new[] { 0, 1 } },

            vertexScale: scale
        ),
        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Wireframe,

            new Vector4[] {
                Vector4.zero,
                new Vector4(0, 0, 0, scale),
            },

            Color.yellow,

            connections: new int[][]
                { new[] { 0, 1 } },

            vertexScale: scale
        ),

        new ConnectedVertices(
            ConnectedVertices.ConnectionMethod.Vertices,

            new Vector4[] {
                new Vector4(0, 0, 0, 0)
            },

            Color.black,

            vertexScale: scale * 2f
        ),
    }, Vector4.zero)
    {

    }
}
