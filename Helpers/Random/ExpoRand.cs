using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExpoRand
{
    public static int Get(int maximum, float probability)
    {
        int number = 0;

        while (Random.value < probability)
        {
            number++;
        }
        return number;
    }
}
