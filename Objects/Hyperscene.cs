using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hyperscene
{
    public abstract List<Hyperobject> Objects { get; }
    public abstract bool ShowFixedAxes { get; }
}
