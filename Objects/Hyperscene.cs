using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hyperscene
{
    public abstract List<Hyperobject> Objects { get; }
    public abstract List<Hyperobject> FixedObjects { get; }
    public virtual bool IsFixed => false;
    public virtual bool IsOrthographic => false; // currently only works if fixed
}
