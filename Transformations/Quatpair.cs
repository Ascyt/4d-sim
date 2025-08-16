using System.Runtime.CompilerServices;
using System;
using UnityEngine;

/// <summary>
/// Represents a 4D rotation using two Quaternions.
/// </summary>
[System.Serializable]
public struct Quatpair
{
    public Quaternion l;
    public Quaternion r;

    public Quatpair(Quaternion l, Quaternion r)
    {
        this.l = l;
        this.r = r;
    }
    public Quatpair(RotationEuler4 rotationEuler)
    {
        Quatpair newRot = identity.ApplyRotation(rotationEuler, true);
        l = newRot.l;
        r = newRot.r;
    }
    public Quatpair(float xw, float yw, float zw, float xy, float xz, float yz)
    {
        Quatpair newRot = identity.ApplyRotation(new RotationEuler4(xw, yw, zw, xy, xz, yz), true);
        l = newRot.l;
        r = newRot.r;
    }

    public Quaternion this[int index]
    {
        get
        {
            return index switch
            {
                0 => l,
                1 => r,
                _ => throw new IndexOutOfRangeException("Invalid Quatpair index!"),
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    l = value;
                    break;
                case 1:
                    r = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Quatpair index!");
            }
        }
    }

    public static Quatpair identity => new(
        Quaternion.identity,
        Quaternion.identity
        );

    public Quatpair normalized => new(
        l.normalized,
        r.normalized
        );

    public static Quatpair Inverse(Quatpair rotation)
        => new(
            Quaternion.Inverse(rotation.l),
            Quaternion.Inverse(rotation.r)
            );

    public static Quatpair Euler(float xw, float yw, float zw, float xy, float xz, float yz)
    {
        return identity.ApplyRotation(new RotationEuler4(xw * Mathf.Deg2Rad, yw * Mathf.Deg2Rad, zw * Mathf.Deg2Rad, xy * Mathf.Deg2Rad, xz * Mathf.Deg2Rad, yz * Mathf.Deg2Rad), true);
    }

    public static Quatpair Slerp(Quatpair a, Quatpair b, float t) => new(
        Quaternion.Slerp(a.l, b.l, t),
        Quaternion.Slerp(a.r, b.r, t)
        );
    public static Quatpair SlerpUnclamped(Quatpair a, Quatpair b, float t) => new(
        Quaternion.SlerpUnclamped(a.l, b.l, t),
        Quaternion.SlerpUnclamped(a.r, b.r, t)
        );
    public static Quatpair Lerp(Quatpair a, Quatpair b, float t) => new(
        Quaternion.Lerp(a.l, b.l, t),
        Quaternion.Lerp(a.r, b.r, t)
        );
    public static Quatpair LerpUnclamped(Quatpair a, Quatpair b, float t) => new(
        Quaternion.LerpUnclamped(a.l, b.l, t),
        Quaternion.LerpUnclamped(a.r, b.r, t)
        );

    public void Set(Quaternion newL, Quaternion newR)
    {
        l = newL;
        r = newR;
    }

    public static Quatpair operator *(Quatpair a, Quatpair b)
        => new(
            a.l * b.l,
            b.r * a.r
            );

    public static Vector4 operator *(Quatpair rotation, Vector4 point)
    {
        Quaternion v = Vector4ToQuaternion(point);
        Quaternion rotated = rotation.l * v * rotation.r;
        return QuaternionToVector4(rotated);
    }
    private static Quaternion Vector4ToQuaternion(Vector4 vector)
        => new(vector.x, vector.y, vector.z, vector.w);
    private static Vector4 QuaternionToVector4(Quaternion quaternion)
        => new(quaternion.x, quaternion.y, quaternion.z, quaternion.w);

    public static bool operator ==(Quatpair lhs, Quatpair rhs)
        => lhs.l == rhs.l && lhs.r == rhs.r;
    public static bool operator !=(Quatpair lhs, Quatpair rhs)
        => !(lhs == rhs);

    public override bool Equals(object obj)
    {
        if (obj is Quatpair other2)
        {
            return Equals(other2);
        }
        return false;
    }
    public bool Equals(Quatpair other)
        =>  l.Equals(other.l) && r.Equals(other.r);

    public override int GetHashCode()
        => (l.GetHashCode() >> 1) ^ r.GetHashCode();

    public static float Dot(Quatpair a, Quatpair b)
        => Quaternion.Dot(a.l, b.l) + Quaternion.Dot(a.r, b.r);

    public static Vector2 Angles(Quatpair a, Quatpair b)
        => new(
            Quaternion.Angle(a.l, b.l),
            Quaternion.Angle(a.r, b.r)
        );

    public override string ToString()
        => ToString(null, null);
    public string ToString(string format=null, IFormatProvider formatProvider = null)
    {
        return $"({l.ToString(format, formatProvider)}, {r.ToString(format, formatProvider)})";
    }
}