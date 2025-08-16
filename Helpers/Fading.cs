using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Fading
{
    public Fading(float fadeDuration, Easing fadeEasing, float delayDuration=0f)
    {
        this.fadeDuration = fadeDuration;
        this.fadeEasing = fadeEasing;
        this.delayDuration = delayDuration;
    }
    public Fading(Fading fading)
    {
        fadeDuration = fading.fadeDuration;
        fadeEasing = fading.fadeEasing;
        delayDuration = fading.delayDuration;
    }

    [SerializeField]
    public float value = default;

    public readonly float fadeDuration;
    public Easing fadeEasing;
    public readonly float delayDuration;

    public bool isFading = false;
    public float fadeTimeLeft;
    public float delayTimeLeft;

    public void StartFade()
    {
        isFading = true;

        value = 0f;

        fadeTimeLeft = fadeDuration;
        delayTimeLeft = delayDuration;
    }
    public void RushFade()
    {
        isFading = false;
        value = 1f;
        fadeTimeLeft = 0f;
        delayTimeLeft = 0f;
    }

    public void UpdateFade(float deltaTime)
    {
        if (!isFading)
            return;

        if (delayTimeLeft > 0f)
        {
            delayTimeLeft -= deltaTime;
            return;
        }

        if (fadeTimeLeft > 0f)
        {
            value = fadeEasing.Get(1f - fadeTimeLeft / fadeDuration);
            fadeTimeLeft -= deltaTime;
            return;
        }

        value = 1f;
        isFading = false;
    }

    public Fading WithEasing(Easing newEasing)
    {
        fadeEasing = newEasing;
        return this;
    }
    public Fading WithEasing(Easing.Type newType, Easing.IO newIO)
    {
        fadeEasing.type = newType;
        fadeEasing.io = newIO;
        return this;
    }
    public Fading WithEasingType(Easing.Type newType)
    {
        fadeEasing.type = newType;
        return this;
    }
    public Fading WithEasingIO(Easing.IO newIO)
    {
        fadeEasing.io = newIO;
        return this;
    }
    public Fading WithDuration(float newDuration)
    {
        return new Fading(newDuration, fadeEasing, delayDuration);
    }
    public Fading WithDuration(Func<Fading, float> func)
    {
        return new Fading(func(this), fadeEasing, delayDuration);
    }
    public Fading WithDelay(float newDelay)
    {
        return new Fading(fadeDuration, fadeEasing, newDelay);
    }
    public Fading WithDelay(Func<Fading, float> func)
    {
        return new Fading(fadeDuration, fadeEasing, func(this));
    }
}
