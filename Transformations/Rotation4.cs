using UnityEngine;

/// <summary>
/// Represents a 4D rotation using two Quaternions.
/// </summary>
[System.Serializable]
public struct Rotation4
{
    public Quaternion rotationXW_YW_ZW;
    public Quaternion rotationXY_XZ_YZ;

    public static Rotation4 zero => new Rotation4(0, 0, 0, 0, 0, 0);

    public Rotation4(Quaternion rotationXW_YW_ZW, Quaternion rotationXY_XZ_YZ)
    {
        this.rotationXW_YW_ZW = rotationXW_YW_ZW;
        this.rotationXY_XZ_YZ = rotationXY_XZ_YZ;
    }
    public Rotation4(float xw, float yw, float zw, float xy, float xz, float yz)
    {
        rotationXW_YW_ZW = Quaternion.Euler(xw * Mathf.Rad2Deg, yw * Mathf.Rad2Deg, zw * Mathf.Rad2Deg);
        rotationXY_XZ_YZ = Quaternion.Euler(xy * Mathf.Rad2Deg, xz * Mathf.Rad2Deg, yz * Mathf.Rad2Deg);
    }
    public Rotation4(RotationEuler4 rotationEuler) : this(rotationEuler.xw, rotationEuler.yw, rotationEuler.zw, rotationEuler.xy, rotationEuler.xz, rotationEuler.yz)
    {
    }

    public static Rotation4 operator -(Rotation4 a)
    {
        Rotation4 newRotation = new Rotation4(
            Quaternion.Inverse(a.rotationXW_YW_ZW),
            Quaternion.Inverse(a.rotationXY_XZ_YZ)
            );

        return newRotation;
    }
}