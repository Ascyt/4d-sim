using UnityEngine;

/// <summary>
/// Represents a 4D rotation using two Quaternions.
/// </summary>
[System.Serializable]
public struct Quatpair
{
    public Quaternion leftQuaternion;
    public Quaternion rightQuaternion;

    public static Quatpair identity => new Quatpair(Quaternion.identity, Quaternion.identity);

    public Quatpair(Quaternion leftQuaternion, Quaternion rightQuaternion)
    {
        this.leftQuaternion = leftQuaternion;
        this.rightQuaternion = rightQuaternion;
    }
    public Quatpair(RotationEuler4 rotationEuler)
    {
        Quatpair newRot = Quatpair.identity.ApplyRotation(rotationEuler, true);
        leftQuaternion = newRot.leftQuaternion;
        rightQuaternion = newRot.rightQuaternion;
    }
    public Quatpair(float xw, float yw, float zw, float xy, float xz, float yz)
    {
        Quatpair newRot = Quatpair.identity.ApplyRotation(new RotationEuler4(xw, yw, zw, xy, xz, yz), true);
        leftQuaternion = newRot.leftQuaternion;
        rightQuaternion = newRot.rightQuaternion;
    }

    public Quatpair Inverse()
    {
        Quatpair newRotation = new Quatpair(
            Quaternion.Inverse(leftQuaternion),
            Quaternion.Inverse(rightQuaternion)
            );

        return newRotation;
    }
    public static Quatpair operator *(Quatpair a, Quatpair b)
    {
        Quatpair newRotation = new Quatpair(
            a.leftQuaternion * b.leftQuaternion,
            b.rightQuaternion * a.rightQuaternion
        );
        return newRotation;
    }
}