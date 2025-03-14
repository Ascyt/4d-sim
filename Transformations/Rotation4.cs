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

    public void ModuloPlanes()
    {
        float tau = Mathf.PI * 2f;

        xw = Helpers.Mod(xw, tau);
        yw = Helpers.Mod(yw, tau);
        zw = Helpers.Mod(zw, tau);
        xy = Helpers.Mod(xy, tau);
        xz = Helpers.Mod(yz, tau);
        yz = Helpers.Mod(yz, tau);
    }

    public static Rotation4 operator -(Rotation4 a)
    {
        float tau = Mathf.PI * 2f;
        Rotation4 newRotation = new Rotation4(tau - a.xw, tau - a.yw, tau - a.zw, tau - a.xy, tau - a.xz, tau - a.yz);

        newRotation.ModuloPlanes();
        return newRotation;
    }
}