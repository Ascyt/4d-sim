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
    public static Rotation4 ApplyRotation(this Rotation4 rotation, RotationEuler4 delta, bool worldSpace)
    {
        return rotation
            .ApplyRotationInSinglePlane(RotationPlane.XW, delta.xw, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.YW, delta.yw, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.ZW, delta.zw, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.XY, delta.xy, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.XZ, delta.xz, worldSpace)
            .ApplyRotationInSinglePlane(RotationPlane.YZ, delta.yz, worldSpace);
    }
    public static Rotation4 ApplyRotationInSinglePlane(this Rotation4 rotation, RotationPlane plane, float delta, bool worldSpace)
    {
        if (delta == 0f)
            return rotation;

        return rotation.ApplyRotation(GetRotationForPlane(plane, delta), worldSpace);
    }
    public static Rotation4 ApplyRotation(this Rotation4 rotation, Rotation4 delta, bool worldSpace)
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
    public static Vector4 ApplyRotation(this Vector4 vector, Rotation4 rotation, Rotation4? alignment=null)
    {
        if (alignment.HasValue)
        {
            // Apply reverse alignment rotation first
            vector = ApplyRotation(vector, alignment.Value.Inverse());
        }

        Quaternion v = Vector4ToQuaternion(vector);
        Quaternion rotated = rotation.leftQuaternion * v * rotation.rightQuaternion;
        return QuaternionToVector4(rotated);
    }

    // Thanks to https://math.stackexchange.com/a/44974
    private static Rotation4 GetRotationForPlane(RotationPlane plane, float angle)
    {
        float t = angle / 2f;

        switch (plane)
        {
            case RotationPlane.XW:
                return new Rotation4
                (
                    new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t)),
                    new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t))
                );
            case RotationPlane.YW:
                return new Rotation4
                (
                    new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t)),
                    new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t))
                );
            case RotationPlane.ZW:
                return new Rotation4
                (
                    new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t)),
                    new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t))
                );
            case RotationPlane.XY:
                return new Rotation4
                (
                    new Quaternion(0, 0, Mathf.Sin(-t), Mathf.Cos(-t)),
                    new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t))
                );
            case RotationPlane.XZ:
                return new Rotation4
                (
                    new Quaternion(0, Mathf.Sin(-t), 0, Mathf.Cos(-t)),
                    new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t))
                );
            case RotationPlane.YZ:
                return new Rotation4
                (
                    new Quaternion(Mathf.Sin(-t), 0, 0, Mathf.Cos(-t)),
                    new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t))
                );
            default:
                throw new ArgumentException("Invalid plane");
        }
    }

    private static Quaternion Vector4ToQuaternion(Vector4 vector)
    {
        return new Quaternion(vector.x, vector.y, vector.z, vector.w);
    }
    private static Vector4 QuaternionToVector4(Quaternion quaternion)
    {
        return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
}