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

            rotation.rightQuaternion = a * rotation.rightQuaternion;
            rotation.leftQuaternion = b * rotation.leftQuaternion;
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
        Quaternion rotated = rotation.leftQuaternion * v * rotation.rightQuaternion;
        return QuaternionToVector4(rotated);
    }

    private static void GetQuaternionPairForPlane(RotationPlane plane, float angle, out Quaternion a, out Quaternion b)
    {
        float sin = Mathf.Sin(angle / 2f);
        float cos = Mathf.Cos(angle / 2f);

        switch (plane)
        {
            case RotationPlane.XW:
                a = new Quaternion(sin, 0, 0, cos);
                b = new Quaternion(sin, 0, 0, cos);
                break;
            case RotationPlane.YW:
                a = new Quaternion(0, sin, 0, cos);
                b = new Quaternion(0, sin, 0, cos);
                break;
            case RotationPlane.ZW:
                a = new Quaternion(0, 0, sin, cos);
                b = new Quaternion(0, 0, sin, cos);
                break;
            case RotationPlane.XY:
                a = new Quaternion(-sin, 0, 0, cos);
                b = new Quaternion(sin, 0, 0, cos);
                break;
            case RotationPlane.XZ:
                a = new Quaternion(0, -sin, 0, cos);
                b = new Quaternion(0, sin, 0, cos);
                break;
            case RotationPlane.YZ:
                a = new Quaternion(0, 0, -sin, cos);
                b = new Quaternion(0, 0, sin, cos);
                break;
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