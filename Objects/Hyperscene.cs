using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

/// <summary>
/// Base class for a hyperscene, which contains a collection of hyperobjects.
/// </summary>
public abstract class Hyperscene
{
    /// <summary>
    /// Objects in the hyperscene that are not fixed and can be moved or rotated by the user.<br /><br />
    /// Should be empty if IsFixed is true.<br />
    /// </summary>
    public abstract List<Hyperobject> Objects { get; }
    /// <summary>
    /// Fixed objects in the hyperscene that only rotate but do not move with the camera position.
    /// </summary>
    public abstract List<Hyperobject> FixedObjects { get; }

    /// <summary>
    /// Whether or not the entire hyperscene should be seen as being fixed in space (reverts rotation direction).
    /// </summary>
    public virtual bool IsFixed => false;
    /// <summary>
    /// Whether or not the hyperscene is orthographic (i.e. no perspective distortion).<br /><br />
    /// Currently only works if IsFixed is true.
    /// </summary>
    public virtual bool IsOrthographic => false;

    public virtual Vector4 StartingPosition => Vector4.zero;
    public virtual void Start() { }
    /// <returns>Refresh objects</returns>
    public virtual List<Hyperobject>? Update() { return null; }
}
