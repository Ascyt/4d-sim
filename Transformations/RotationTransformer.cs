using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for rotating points in 4D space using Euler Angles from Rotation4.
/// </summary>
public static class RotationTransformer
{
    public enum RotationPlane
    {
        XW,
        YW,
        ZW,
        XY,
        XZ,
        YZ
    }

    // Thanks to https://math.stackexchange.com/a/44974
    public static Quatpair ApplyRotation(this Quatpair rotation, RotationEuler4 delta, bool worldSpace)
    {
        return rotation
            .ApplyRotationInSinglePlane(RotationPlane.XW, delta.xw, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.YW, delta.yw, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.ZW, delta.zw, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.XY, delta.xy, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.XZ, delta.xz, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.YZ, delta.yz, worldSpace);
    }
    public static Quatpair ApplyRotationInSinglePlane(this Quatpair rotation, RotationPlane plane, float delta, bool worldSpace)
    {
        if (delta == 0f)
            return rotation;

        return rotation.ApplyRotation(GetRotationForPlane(plane, delta), worldSpace);
    }
    public static Quatpair ApplyRotation(this Quatpair rotation, Quatpair delta, bool worldSpace)
    {
        if (worldSpace)
        {
            return delta * rotation;
        }
        else
        {
            return rotation * delta;
        }
    }

    // Thanks to https://math.stackexchange.com/a/44974
    public static Vector4 ApplyRotation(this Vector4 vector, Quatpair rotation, Quatpair? alignment=null)
    {
        if (alignment.HasValue)
        {
            // Apply reverse alignment rotation first
            vector = ApplyRotation(vector, Quatpair.Inverse(alignment.Value));
        }

        return rotation * vector;
    }

    // Thanks to https://math.stackexchange.com/a/44974
    private static Quatpair GetRotationForPlane(RotationPlane plane, float angle)
    {
        float t = angle / 2f;

        return plane switch
        {
            RotationPlane.XW => new Quatpair
                            (
                                new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t)),
                                new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t))
                            ),
            RotationPlane.YW => new Quatpair
                            (
                                new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t)),
                                new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t))
                            ),
            RotationPlane.ZW => new Quatpair
                            (
                                new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t)),
                                new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t))
                            ),
            RotationPlane.XY => new Quatpair
                            (
                                new Quaternion(0, 0, Mathf.Sin(-t), Mathf.Cos(-t)),
                                new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t))
                            ),
            RotationPlane.XZ => new Quatpair
                            (
                                new Quaternion(0, Mathf.Sin(-t), 0, Mathf.Cos(-t)),
                                new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t))
                            ),
            RotationPlane.YZ => new Quatpair
                            (
                                new Quaternion(Mathf.Sin(-t), 0, 0, Mathf.Cos(-t)),
                                new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t))
                            ),
            _ => throw new ArgumentException("Invalid plane"),
        };
    }
}