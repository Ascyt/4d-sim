using UnityEngine;

/// <summary>
/// Represents a 4D rotation using two Quaternions.
/// </summary>
[System.Serializable]
public struct Rotation4
{
    public Quaternion leftQuaternion;
    public Quaternion rightQuaternion;

    public static Rotation4 zero => new Rotation4(0, 0, 0, 0, 0, 0);

    public Rotation4(Quaternion rightQuaternion, Quaternion leftQuaternion)
    {
        this.leftQuaternion = leftQuaternion;
        this.rightQuaternion = rightQuaternion;
    }
    public Rotation4(float xw, float yw, float zw, float xy, float xz, float yz)
    {
        rightQuaternion = Quaternion.Euler(xw * Mathf.Rad2Deg, yw * Mathf.Rad2Deg, zw * Mathf.Rad2Deg);
        leftQuaternion = Quaternion.Euler(xy * Mathf.Rad2Deg, xz * Mathf.Rad2Deg, yz * Mathf.Rad2Deg);
    }
    public Rotation4(RotationEuler4 rotationEuler) : this(rotationEuler.xw, rotationEuler.yw, rotationEuler.zw, rotationEuler.xy, rotationEuler.xz, rotationEuler.yz)
    {
    }

    public static Rotation4 operator -(Rotation4 a)
    {
        Rotation4 newRotation = new Rotation4(
            Quaternion.Inverse(a.rightQuaternion),
            Quaternion.Inverse(a.leftQuaternion)
            );

        return newRotation;
    }
}