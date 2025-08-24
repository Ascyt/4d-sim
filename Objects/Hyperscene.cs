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
    public abstract HashSet<Hyperobject> Objects { get; }
    /// <summary>
    /// Fixed objects in the hyperscene that only rotate but do not move with the camera position.
    /// </summary>
    public abstract HashSet<Hyperobject> FixedObjects { get; }

    /// <summary>
    /// Whether or not the entire hyperscene should be seen as being fixed in space (reverts rotationDelta direction).
    /// </summary>
    public virtual bool IsFixed => false;
    /// <summary>
    /// Whether or not the hyperscene is orthographic (i.e. no perspective distortion).<br /><br />
    /// Currently only works if IsFixed is true.
    /// </summary>
    public virtual bool IsOrthographic => false;

    public virtual Vector4 StartingPosition => Vector4.zero;
    public virtual void Start() { }
    /// <returns>(normal objects to rerender, fixed objects to rerender)</returns>
    public virtual (HashSet<Hyperobject>?, HashSet<Hyperobject>?) Update() { return (null, null); }

    public virtual bool ShowSceneSlider => false;
    /// <returns>(normal objects to rerender, fixed objects to rerender)</returns>
    public virtual (HashSet<Hyperobject>?, HashSet<Hyperobject>?) OnSceneSliderUpdate(float value) { return (null, null); }
}
