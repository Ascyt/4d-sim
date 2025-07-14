using UnityEngine;

/// <summary>
/// Represents a 4D rotation using two Quaternions.
/// </summary>
[System.Serializable]
public struct Rotation4
{
    public Quaternion leftQuaternion;
    public Quaternion rightQuaternion;

    public static Rotation4 identity => new Rotation4(Quaternion.identity, Quaternion.identity);

    public Rotation4(Quaternion leftQuaternion, Quaternion rightQuaternion)
    {
        this.leftQuaternion = leftQuaternion;
        this.rightQuaternion = rightQuaternion;
    }
    public Rotation4(RotationEuler4 rotationEuler)
    {
        Rotation4 newRot = Rotation4.identity.ApplyRotation(rotationEuler, true);
        leftQuaternion = newRot.leftQuaternion;
        rightQuaternion = newRot.rightQuaternion;
    }
    public Rotation4(float xw, float yw, float zw, float xy, float xz, float yz)
    {
        Rotation4 newRot = Rotation4.identity.ApplyRotation(new RotationEuler4(xw, yw, zw, xy, xz, yz), true);
        leftQuaternion = newRot.leftQuaternion;
        rightQuaternion = newRot.rightQuaternion;
    }

    public Rotation4 Inverse()
    {
        Rotation4 newRotation = new Rotation4(
            Quaternion.Inverse(leftQuaternion),
            Quaternion.Inverse(rightQuaternion)
            );

        return newRotation;
    }
    public static Rotation4 operator *(Rotation4 a, Rotation4 b)
    {
        Rotation4 newRotation = new Rotation4(
            a.leftQuaternion * b.leftQuaternion,
            b.rightQuaternion * a.rightQuaternion
        );
        return newRotation;
    }
}