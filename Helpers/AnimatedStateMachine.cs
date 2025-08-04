using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AnimatedStateMachine<T> : MonoBehaviour where T : Enum
{
    protected abstract void OnEnterState(T state);
    protected abstract void OnExitState(T state);
    protected abstract void OnUpdateState(T state, float fadingValue, float[] additionalFadingValues);
    protected abstract void OnStart();

    protected abstract Fading DefaultFading { get; }


    private readonly HashSet<KeyCode> _nextKeysDefault = new() { KeyCode.RightArrow, KeyCode.Space, KeyCode.PageDown, KeyCode.Return };
    protected virtual HashSet<KeyCode> NextKeys => _nextKeysDefault;

    private readonly HashSet<KeyCode> _prevKeysDefault = new() { KeyCode.LeftArrow, KeyCode.LeftShift, KeyCode.PageUp, KeyCode.Backspace };
    protected virtual HashSet<KeyCode> PrevKeys => _prevKeysDefault;

    protected virtual Dictionary<T, float> AutoSkipStates => new();
    private bool _autoSkipCurrentState = false;
    private float _autoSkipTimeLeft = 0f;

    protected virtual Dictionary<T, Fading[]> AdditionalFadings { get => new(); }


    private T _currentState = default;
    public T CurrentState { get => _currentState; set => SetState(value); }
    public int StateCount => Enum.GetValues(typeof(T)).Length;

    public void SetState(T state)
    {
        T target = state;

        if (!Enum.IsDefined(typeof(T), target))
        {
            Debug.LogError($"Invalid state: {target}.");
            return;
        }
        if (_currentState.Equals(target))
        {
            return;
        }

        // Rush currently running state
        RushStateFadings(_currentState);

        // Exit currently running state
        OnUpdateState(_currentState, DefaultFading.value, SelectAdditionalFadingValues(AdditionalFadings, _currentState));
        OnExitState(_currentState);

        // If jumping to a future state: Enter, rush and exit intermediate states from old to new
        int intermediatePastStatesCount = target.ToInt() - _currentState.ToInt() - 1;
        for (int i = 1; i <= intermediatePastStatesCount; i++)
        {
            T pastState = _currentState.RunOnEnumAsInt(s => s + i);

            // Enter intermediate step
            StartStateFadings(pastState);
            OnEnterState(pastState);

            // Rush intermediate step
            RushStateFadings(pastState);
            OnUpdateState(pastState, DefaultFading.value, SelectAdditionalFadingValues(AdditionalFadings, pastState));

            // Exit intermediate step
            OnExitState(pastState);
        }
        // If jumping to a past state: Reset all states, run again from start to target
        if (target.ToInt() < _currentState.ToInt())
        {
            _currentState = default; 
            OnStart();

            StartStateFadings(_currentState);
            OnEnterState(_currentState);

            RushStateFadings(_currentState);
            OnUpdateState(_currentState, DefaultFading.value, SelectAdditionalFadingValues(AdditionalFadings, _currentState));

            SetState(target);
            RushStateFadings(_currentState);
            return;
        }

        _currentState = target;

        // Start the target state
        StartStateFadings(_currentState);
        OnEnterState(_currentState);
        OnUpdateState(_currentState, DefaultFading.value, SelectAdditionalFadingValues(AdditionalFadings, _currentState));

        if (AutoSkipStates.TryGetValue(_currentState, out float autoSkipTime))
        {
            _autoSkipCurrentState = true;
            _autoSkipTimeLeft = autoSkipTime;
        }
        else
        {
            _autoSkipCurrentState = false;
            _autoSkipTimeLeft = 0f;
        }
    }
    public void NextState()
    {
        SetState(_currentState.RunOnEnumAsInt(s => (s + 1) % StateCount));
    }
    public void PreviousState()
    {
        SetState(_currentState.RunOnEnumAsInt(s => (s - 1 + StateCount) % StateCount));
    }

    private void StartStateFadings(T state)
    {
        DefaultFading.StartFade();

        if (AdditionalFadings.TryGetValue(state, out Fading[] additionalFadings))
        {
            foreach (Fading fading in additionalFadings)
            {
                fading.StartFade();
            }
        }
    }
    private void RushStateFadings(T state)
    {
        DefaultFading.RushFade();

        if (AdditionalFadings.TryGetValue(state, out Fading[] additionalFadings))
        {
            foreach (Fading fading in additionalFadings)
            {
                fading.RushFade();
            }
        }
    }

    private static float[] SelectAdditionalFadingValues(Dictionary<T, Fading[]> additionalFadings, T key)
    {
        if (!additionalFadings.TryGetValue(key, out Fading[] values) || values == null || values.Length == 0)
        {
            return new float[0];
        }

        return values
            .Select(fading => fading.value)
            .ToArray();
    }

    private void Update()
    {
        if (_autoSkipCurrentState)
        {
            _autoSkipTimeLeft -= Time.deltaTime;

            if (_autoSkipTimeLeft <= 0f)
            {
                _autoSkipCurrentState = false;
                NextState();
            }
        }

        if (NextKeys.Any(key => Input.GetKeyDown(key)))
        {
            NextState();
        }
        if (PrevKeys.Any(key => Input.GetKeyDown(key)))
        {
            PreviousState();
        }

        DefaultFading.UpdateFade(Time.deltaTime);
        if (AdditionalFadings.TryGetValue(_currentState, out Fading[] additionalFadings))
        {
            foreach (Fading fading in additionalFadings)
            {
                fading.UpdateFade(Time.deltaTime);
            }
        }

        OnUpdateState(_currentState, DefaultFading.value, SelectAdditionalFadingValues(AdditionalFadings, _currentState));
    }

    private void Start()
    {
        OnStart();
        StartStateFadings(_currentState);
        OnEnterState(_currentState);
    }
}
