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

    public static Vector4 Rotate(this Vector4 point, Rotation4 rotation)
    {
        return point
            .Rotate(RotationPlane.XW, rotation.xw)
            .Rotate(RotationPlane.YW, rotation.yw)
            .Rotate(RotationPlane.ZW, rotation.zw)
            .Rotate(RotationPlane.YZ, rotation.yz)
            .Rotate(RotationPlane.XZ, rotation.xz)
            .Rotate(RotationPlane.XY, rotation.xy);
    }
    public static Vector4 RotateNeg(this Vector4 point, Rotation4 rotation)
    {
        rotation = -rotation;

        return point
            .Rotate(RotationPlane.XY, rotation.xy)
            .Rotate(RotationPlane.XZ, rotation.xz)
            .Rotate(RotationPlane.YZ, rotation.yz)
            .Rotate(RotationPlane.ZW, rotation.zw)
            .Rotate(RotationPlane.YW, rotation.yw)
            .Rotate(RotationPlane.XW, rotation.xw);
    }
    public static Vector4 Rotate(this Vector4 point, RotationPlane plane, float angle)
    {
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        switch (plane)
        {
            case RotationPlane.XY:
                return new Vector4(
                    cos * point.x + sin * point.y,
                    -sin * point.x + cos * point.y,
                    point.z,
                    point.w
                );

            case RotationPlane.XZ:
                return new Vector4(
                    cos * point.x + sin * point.z,
                    point.y,
                    -sin * point.x + cos * point.z,
                    point.w
                );
            case RotationPlane.XW:
                return new Vector4(
                    cos * point.x + sin * point.w,
                    point.y,
                    point.z,
                    -sin * point.x + cos * point.w
                );

            case RotationPlane.YZ:
                return new Vector4(
                    point.x,
                    cos * point.y + sin * point.z,
                    -sin * point.y + cos * point.z,
                    point.w
                );

            case RotationPlane.YW:
                return new Vector4(
                    point.x,
                    cos * point.y + sin * point.w,
                    point.z,
                    -sin * point.y + cos * point.w
                );

            case RotationPlane.ZW:
                return new Vector4(
                    point.x,
                    point.y,
                    cos * point.z + sin * point.w,
                    -sin * point.z + cos * point.w
                );

            default:
                throw new ArgumentException("Invalid rotation plane");
        }
    }
}