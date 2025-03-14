using UnityEngine;

[System.Serializable]
public struct Rotation4
{
    public float xw;
    public float yw;
    public float zw;
    public float xy;
    public float xz;
    public float yz;

    public Rotation4(float xw, float yw, float zw, float xy, float xz, float yz)
    {
        this.xw = xw; this.yw = yw; this.zw = zw; this.xy = xy; this.xz = xz; this.yz = yz; 
    }

    public static Rotation4 operator -(Rotation4 a)
    {
        float tau = Mathf.PI * 2;
        return new Rotation4((tau - a.xw) % tau, (tau - a.yw) % tau, (tau - a.zw) % tau, (tau - a.xy) % tau, (tau - a.xz) % tau, (tau - a.yz) % tau);
    }
}