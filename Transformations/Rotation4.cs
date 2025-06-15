using UnityEngine;

// TODO: Use Quaternions instead of this struct for rotations
// ... Somehow (they're already confusing me enough as is for 3D, yet alone for the planar rotational mess that is 4D...)
// Currently, rotation order issues are avoided by using continuously applying rotation deltas, instead of re-applying the entire rotation every frame.
// More advanced features like interpolating between rotations or a physics engine will require Quaternions or some other more advanced rotation representation.
// Idea:
//      Maybe use a Quaternion for XW, YW and ZW as 3D-rotations and a separate one for XY, XZ and YZ as 3D-rotations?
//      Though this might only make sense for the camera, as here there's a clear distinction between those (XW, YW, ZW for 4D rotation, XY, XZ, YZ for camera tilt).
/// <summary>
/// Represents a 4D rotation using 6 planes of rotation (XW, YW, ZW, XY, XZ, YZ). Uses Euler Angles so note the order of rotations.
/// </summary>
[System.Serializable]
public struct Rotation4
{
    public float xw;
    public float yw;
    public float zw;
    public float xy;
    public float xz;
    public float yz;

    public static Rotation4 zero => new Rotation4(0, 0, 0, 0, 0, 0);

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
        xz = Helpers.Mod(xz, tau);
        yz = Helpers.Mod(yz, tau);
    }

    public static Rotation4 operator -(Rotation4 a)
    {
        float tau = Mathf.PI * 2f;
        Rotation4 newRotation = new Rotation4(tau - a.xw, tau - a.yw, tau - a.zw, tau - a.xy, tau - a.xz, tau - a.yz);
    
        newRotation.ModuloPlanes();
        return newRotation;
    }

    public static Rotation4 operator +(Rotation4 a, Rotation4 b)
    {
        Rotation4 newRotation = new Rotation4(a.xw + b.xw, a.yw + b.yw, a.zw + b.zw, a.xy + b.xy, a.xz + b.xz, a.yz + b.yz);
        newRotation.ModuloPlanes();
        return newRotation;
    }

    public static Rotation4 operator -(Rotation4 a, Rotation4 b)
    {
        Rotation4 newRotation = new Rotation4(a.xw - b.xw, a.yw - b.yw, a.zw - b.zw, a.xy - b.xy, a.xz - b.xz, a.yz - b.yz);
        newRotation.ModuloPlanes();
        return newRotation;
    }
}