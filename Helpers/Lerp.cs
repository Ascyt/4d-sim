using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerpable<T>
{
    public delegate T LerpDelegate(T[] array, float t);
    private LerpDelegate lerpMethod;

    public Lerpable(LerpDelegate lerpMethod)
    {
        this.lerpMethod = lerpMethod;
    }

    public T Lerp(T[] array, float t)
    {
        return lerpMethod(array, t);
    }
}
public static class LerpMethods
{
    private static readonly Lerpable<float> floatLerpable = new Lerpable<float>(Lerp); 
    private static readonly Lerpable<Vector3> vector3Lerpable = new Lerpable<Vector3>(Lerp); 
    private static readonly Lerpable<Color> colorLerpable = new Lerpable<Color>(Lerp); 
    private static readonly Lerpable<Quaternion> quaternionLerpable = new Lerpable<Quaternion>(Lerp);

    public static Lerpable<T> GetLerpable<T>()
    {
        if (typeof(T) == typeof(float))
            return floatLerpable as Lerpable<T>;
        if (typeof(T) == typeof(Vector3))
            return vector3Lerpable as Lerpable<T>;
        if (typeof(T) == typeof(Color))
            return colorLerpable as Lerpable<T>;
        if (typeof(T) == typeof(Quaternion))
            return quaternionLerpable as Lerpable<T>;

        throw new NotSupportedException($"Lerpable<{typeof(T)}> is not supported");
    }

    public static float Lerp(this float[] array, float t)
    {
        GetValues(array.Length, ref t, out int from, out int to);

        return (array[from] * Mathf.Clamp01(1 - t)) + (array[to] * t);
    }

    public static Vector3 Lerp(this Vector3[] array, float t)
    {
        GetValues(array.Length, ref t, out int from, out int to);

        return (array[from] * Mathf.Clamp01(1 - t)) + (array[to] * t);
    }
    public static Color Lerp(this Color[] array, float t)
    {
        GetValues(array.Length, ref t, out int from, out int to);

        return Color.Lerp(array[from], array[to], t);
    }
    public static Quaternion Lerp(this Quaternion[] array, float t)
    {
        GetValues(array.Length, ref t, out int from, out int to);

        return Quaternion.Lerp(array[from], array[to], t);
    }

    private static void GetValues(int arrayLength, ref float t, out int from, out int to)
    {
        from = Mathf.Max((int)((arrayLength - 1) * t), 0);
        to = Mathf.Min(from + 1, arrayLength - 1);

        float amount = from / (float)arrayLength;

        t = (t - amount) * (1 - amount);
    }
}
