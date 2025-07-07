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

    public static Rotation4 AddEuler(this Rotation4 rotation, RotationEuler4 delta)
    {
        // Convert the delta to Quaternion and apply it to the existing rotation
        foreach (RotationPlane rotationPlane in Enum.GetValues(typeof(RotationPlane)))
        {
            float angle = delta[rotationPlane];
            if (Mathf.Approximately(angle, 0f))
                continue;

            GetQuaternionPairForPlane(rotationPlane, angle, out Quaternion a, out Quaternion b);

            rotation.rotationXW_YW_ZW = a * rotation.rotationXW_YW_ZW;
            rotation.rotationXY_XZ_YZ = b * rotation.rotationXY_XZ_YZ;
        }

        return rotation;
    }
    public static Rotation4 SubEuler(this Rotation4 rotation, RotationEuler4 delta)
    {
        return rotation.AddEuler(-delta);
    }

    // Thanks to https://math.stackexchange.com/a/44974
    public static Vector4 ApplyRotation(this Vector4 vector, Rotation4 rotation, bool worldSpace)
    {
        Quaternion v = Vector4ToQuaternion(vector);
        Quaternion rotated = rotation.rotationXY_XZ_YZ * v * rotation.rotationXW_YW_ZW;
        return QuaternionToVector4(rotated);
    }

    public static void GetQuaternionPairForPlane(RotationPlane plane, float angle, out Quaternion a, out Quaternion b)
    {
        switch (plane)
        {
            case RotationPlane.XW:
                a = QuaternionFromAxisAngle(new Vector3(1, 0, 0), -angle / 2f);
                b = QuaternionFromAxisAngle(new Vector3(1, 0, 0), angle / 2f);
                break;
            case RotationPlane.YW:
                a = QuaternionFromAxisAngle(new Vector3(0, 1, 0), -angle / 2f);
                b = QuaternionFromAxisAngle(new Vector3(0, 1, 0), angle / 2f);
                break;
            case RotationPlane.ZW:
                a = QuaternionFromAxisAngle(new Vector3(0, 0, 1), -angle / 2f);
                b = QuaternionFromAxisAngle(new Vector3(0, 0, 1), angle / 2f);
                break;
            case RotationPlane.XY:
                a = QuaternionFromAxisAngle(new Vector3(0, 0, 1), angle / 2f);
                b = QuaternionFromAxisAngle(new Vector3(0, 0, 1), angle / 2f);
                break;
            case RotationPlane.XZ:
                a = QuaternionFromAxisAngle(new Vector3(0, 1, 0), angle / 2f);
                b = QuaternionFromAxisAngle(new Vector3(0, 1, 0), angle / 2f);
                break;
            case RotationPlane.YZ:
                a = QuaternionFromAxisAngle(new Vector3(1, 0, 0), angle / 2f);
                b = QuaternionFromAxisAngle(new Vector3(1, 0, 0), angle / 2f);
                break;
            default:
                throw new ArgumentException("Invalid plane");
        }
    }

    private static Quaternion QuaternionFromAxisAngle(Vector3 axis, float angle)
    {
        axis = axis.normalized;
        float halfAngle = angle / 2f;
        float sin = Mathf.Sin(halfAngle);
        float cos = Mathf.Cos(halfAngle);
        return new Quaternion(axis.x * sin, axis.y * sin, axis.z * sin, cos);
    }

    public static Quaternion Vector4ToQuaternion(Vector4 v)
    {
        return new Quaternion(v.x, v.y, v.z, v.w);
    }

    public static Vector4 QuaternionToVector4(Quaternion q)
    {
        return new Vector4(q.x, q.y, q.z, q.w);
    }
}