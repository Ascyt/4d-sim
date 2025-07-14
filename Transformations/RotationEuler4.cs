using UnityEngine;

[System.Serializable]
public struct RotationEuler4
{
    public float xw;
    public float yw;
    public float zw;
    public float xy;
    public float xz;
    public float yz;

    public static RotationEuler4 zero => new RotationEuler4(0, 0, 0, 0, 0, 0);

    public float this[RotationTransformer.RotationPlane rotationPlane]
    {
        get
        {
            return rotationPlane switch
            {
                RotationTransformer.RotationPlane.XW => xw,
                RotationTransformer.RotationPlane.YW => yw,
                RotationTransformer.RotationPlane.ZW => zw,
                RotationTransformer.RotationPlane.XY => xy,
                RotationTransformer.RotationPlane.XZ => xz,
                RotationTransformer.RotationPlane.YZ => yz,
                _ => throw new System.ArgumentOutOfRangeException(nameof(rotationPlane), rotationPlane, null)
            };
        }
    }

    public RotationEuler4(float xw, float yw, float zw, float xy, float xz, float yz)
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
        xz = Helpers.Mod(xz, tau);
        yz = Helpers.Mod(yz, tau);
    }

    public static RotationEuler4 operator -(RotationEuler4 a)
    {
        float tau = Mathf.PI * 2f;
        RotationEuler4 newRotation = new RotationEuler4(tau - a.xw, tau - a.yw, tau - a.zw, tau - a.xy, tau - a.xz, tau - a.yz);
    
        newRotation.ModuloPlanes();
        return newRotation;
    }

    public static RotationEuler4 operator +(RotationEuler4 a, RotationEuler4 b)
    {
        RotationEuler4 newRotation = new RotationEuler4(a.xw + b.xw, a.yw + b.yw, a.zw + b.zw, a.xy + b.xy, a.xz + b.xz, a.yz + b.yz);
        newRotation.ModuloPlanes();
        return newRotation;
    }

    public static RotationEuler4 operator -(RotationEuler4 a, RotationEuler4 b)
    {
        RotationEuler4 newRotation = new RotationEuler4(a.xw - b.xw, a.yw - b.yw, a.zw - b.zw, a.xy - b.xy, a.xz - b.xz, a.yz - b.yz);
        newRotation.ModuloPlanes();
        return newRotation;
    }
}