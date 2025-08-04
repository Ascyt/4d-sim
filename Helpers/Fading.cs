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
    public readonly Easing fadeEasing;
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
}
