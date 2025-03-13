using UnityEngine;

[System.Serializable]
public struct Rotation4
{
    public float xy;
    public float xz;
    public float yz;
    public float xw;
    public float yw;
    public float zw;

    public Rotation4(float xy, float xz, float yz, float xw, float yw, float zw)
    {
        this.xy = xy; this.xz = xz; this.yz = yz; this.xw = xw; this.yw = yw; this.zw = zw;
    }

    public static Rotation4 operator -(Rotation4 a)
    {
        float tau = Mathf.PI * 2;
        return new Rotation4((tau - a.xy) % tau, (tau - a.xz) % tau, (tau - a.yz) % tau, (tau - a.xw) % tau, (tau - a.yw) % tau, (tau - a.zw) % tau);
    }
}