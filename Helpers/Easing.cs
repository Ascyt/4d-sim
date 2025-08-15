using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Easing
{
    public Easing(Type type = Type.Linear, IO io = IO.InOut)
    {
        this.type = type;
        this.io = io;
    }

    public enum Type
    {
        Linear, Jump, Sine, Quad, Cubic, Expo, Circ, Back, Elastic, Bounce
    }
    public enum IO
    {
        In, Out, InOut
    }
    public Type type;
    public IO io;

    public readonly float Get(float x) => Get(x, type, io);
    public static float Get(float x, Easing easing) => Get(x, easing.type, easing.io);

    public static float Get(float x, Type type, IO io)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;
        const float c3 = c1 + 1;
        const float c4 = 2f * Mathf.PI / 3f;
        const float c5 = 2f * Mathf.PI / 4.5f;
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        switch (type)
        {
            case Type.Linear:
                return x;
            case Type.Jump:
                return io switch
                {
                    IO.In => 1,
                    IO.Out => x >= 1 ? 1 : 0,
                    _ => (float)(x > 0.5f ? 1 : 0),
                };
            case Type.Sine:
                return io switch
                {
                    IO.In => 1 - Mathf.Cos(x * Mathf.PI / 2f),
                    IO.Out => Mathf.Sin(x * Mathf.PI / 2f),
                    _ => -(Mathf.Cos(Mathf.PI * x) - 1) / 2f,
                };
            case Type.Quad:
                return io switch
                {
                    IO.In => x * x,
                    IO.Out => 1 - (1 - x) * (1 - x),
                    _ => x < 0.5f ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2f,
                };
            case Type.Cubic:
                return io switch
                {
                    IO.In => x * x * x,
                    IO.Out => 1 - Mathf.Pow(1 - x, 3),
                    _ => x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2f,
                };
            case Type.Expo:
                return io switch
                {
                    IO.In => x == 0 ? 0 : Mathf.Pow(2f, 10f * x - 10),
                    IO.Out => x == 1 ? 1 : 1 - Mathf.Pow(2f, -10f * x),
                    _ => x <= 0 ?
                        0 : x >= 1 ?
                        1 : x < 0.5f ?
                        Mathf.Pow(2f, 20f * x - 10) / 2f : (2 - Mathf.Pow(2, -20 * x + 10)) / 2f,
                };
            case Type.Circ:
                return io switch
                {
                    IO.In => 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2)),
                    IO.Out => Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2)),
                    _ => x < 0.5f ?
                                                (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2 : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2f,
                };
            case Type.Back:
                return io switch
                {
                    IO.In => c3 * x * x * x - c1 * x * x,
                    IO.Out => 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2),
                    _ => x < 0.5 ? Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2) / 2f : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2f,
                };
            case Type.Elastic:
                return io switch
                {
                    IO.In => x <= 0 ? 0 :
                        x >= 1 ?
                        1 : -Mathf.Pow(2, 10f * x - 10) * Mathf.Sin((x * 10f - 10.75f) * c4),
                    IO.Out => x <= 0 ?
                        0 : x >= 1 ?
                        1 : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1,
                    _ => x <= 0 ?
                        0 : x >= 1 ?
                        1 : x < 0.5f ?
                        -(Mathf.Pow(2, 20f * x - 10) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f : (Mathf.Pow(2, -20f * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2f + 1,
                };
            case Type.Bounce:
                switch (io)
                {
                    case IO.In:
                        return 1 - Get(1 - x, Type.Bounce, IO.Out);
                    case IO.Out:
                        if (x < 1f / d1)
                            return n1 * x * x;
                        if (x < 2f / d1)
                            return n1 * (x -= 1.5f / d1) * x + 0.75f;
                        if (x < 2.5f / d1)
                            return n1 * (x -= 2.25f / d1) * x + 0.9375f;

                        return n1 * (x -= 2.625f / d1) * x + 0.984375f;
                    default:
                        return x < 0.5f ? (1 - Get(1 - 2 * x, Type.Bounce, IO.Out)) / 2f : (1 + Get(2 * x - 1, Type.Bounce, IO.Out) / 2f);
                }
        }
        return x;
    }

    public readonly Easing WithType(Type newType)
    {
        return new Easing(newType, io);
    }
    public readonly Easing WithIO(IO newIO)
    {
        return new Easing(type, newIO);
    }
}
