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
    public static Rotation4 AddEuler(this Rotation4 rotation, RotationEuler4 delta, bool worldSpace)
    {
        // Convert the delta to Quaternion and apply it to the existing rotation
        foreach (RotationPlane rotationPlane in Enum.GetValues(typeof(RotationPlane)))
        {
            float angle = delta[rotationPlane];
            if (Mathf.Approximately(angle, 0f))
                continue;

            GetQuaternionPairForPlane(rotationPlane, angle, out Quaternion leftDelta, out Quaternion rightDelta);

            if (worldSpace)
            {
                rotation.leftQuaternion = leftDelta * rotation.leftQuaternion;
                rotation.rightQuaternion = rotation.rightQuaternion * rightDelta;
            }
            else
            {
                rotation.leftQuaternion = rotation.leftQuaternion * leftDelta;
                rotation.rightQuaternion = rightDelta * rotation.rightQuaternion;
            }
        }

        return rotation;
    }

    // Thanks to https://math.stackexchange.com/a/44974
    public static Vector4 ApplyRotation(this Vector4 vector, Rotation4 rotation, Rotation4? alignment=null)
    {
        if (alignment.HasValue)
        {
            // Apply reverse alignment rotation first
            vector = ApplyRotation(vector, -alignment.Value);
        }

        Quaternion v = Vector4ToQuaternion(vector);
        Quaternion rotated = rotation.leftQuaternion * v * rotation.rightQuaternion;
        return QuaternionToVector4(rotated);
    }

    // Thanks to https://math.stackexchange.com/a/44974
    private static void GetQuaternionPairForPlane(RotationPlane plane, float angle, out Quaternion left, out Quaternion right)
    {
        float t = angle / 2f;

        switch (plane)
        {
            case RotationPlane.XW:
                left = new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t));
                right = new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t));
                break;
            case RotationPlane.YW:
                left = new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t));
                right = new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t));
                break;
            case RotationPlane.ZW:
                left = new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t));
                right = new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t));
                break;
            case RotationPlane.XY:
                left = new Quaternion(0, 0, Mathf.Sin(-t), Mathf.Cos(-t));
                right = new Quaternion(0, 0, Mathf.Sin(t), Mathf.Cos(t));
                break;
            case RotationPlane.XZ:
                left = new Quaternion(0, Mathf.Sin(-t), 0, Mathf.Cos(-t));
                right = new Quaternion(0, Mathf.Sin(t), 0, Mathf.Cos(t));
                break;
            case RotationPlane.YZ:
                left = new Quaternion(Mathf.Sin(-t), 0, 0, Mathf.Cos(-t));
                right = new Quaternion(Mathf.Sin(t), 0, 0, Mathf.Cos(t));
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