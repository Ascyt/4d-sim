using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

public abstract class AnimatedStateMachine<T> : MonoBehaviour where T : Enum
{
    protected abstract void OnEnterState(T state);
    protected abstract void BeforeExitState(T state);
    protected abstract void OnStart();

    protected virtual void OnUpdate() { }


    private readonly HashSet<KeyCode> _nextKeysDefault = new() { KeyCode.RightArrow, KeyCode.Space, KeyCode.PageDown, KeyCode.Return };
    protected virtual HashSet<KeyCode> NextKeys => _nextKeysDefault;

    private readonly HashSet<KeyCode> _prevKeysDefault = new() { KeyCode.LeftArrow, KeyCode.LeftShift, KeyCode.PageUp, KeyCode.Backspace };
    protected virtual HashSet<KeyCode> PrevKeys => _prevKeysDefault;

    protected virtual Dictionary<T, float> AutoSkipStates => new();
    private bool _autoSkipCurrentState = false;
    private float _autoSkipTimeLeft = 0f;

    private T _currentState = default!;
    public T CurrentState { get => _currentState; set => SetState(value); }
    public int StateCount => Enum.GetValues(typeof(T)).Length;


    private struct FadeData
    {
        public Fading fading;
        public FadeUpdate func;
        public bool runWhileOnDelay;
    }
    private readonly HashSet<FadeData> _singleFades = new();
    public delegate void FadeUpdate(float fadingValue, bool isExit);
    protected void Fade(Fading fading, FadeUpdate func, bool runWhileOnDelay=true)
    {
        FadeData newData = new()
        {
            fading = fading,
            func = func,
            runWhileOnDelay = runWhileOnDelay
        };

        _singleFades.Add(newData);

        fading.StartFade();

        if (runWhileOnDelay || fading.delayTimeLeft <= 0f)
            func(newData.fading.value, isExit: false);
    }


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
            // Restart current state
            RestartCurrentState();

            return;
        }

        // Exit currently running state
        PrivateBeforeExitState(_currentState);

        // If jumping to a future state: Enter, rush and exit intermediate states from old to new
        int intermediatePastStatesCount = target.ToInt() - _currentState.ToInt() - 1;
        for (int i = 1; i <= intermediatePastStatesCount; i++)
        {
            T pastState = _currentState.RunOnEnumAsInt(s => s + i);

            // Enter intermediate step
            PrivateOnEnterState(pastState);

            // Exit intermediate step
            PrivateBeforeExitState(pastState);
        }
        // If jumping to a past state: Reset all states, run again from start to target
        if (target.ToInt() < _currentState.ToInt())
        {
            // Start first state
            _currentState = default!;
            OnStart();
            PrivateOnEnterState(_currentState);

            // Run through all states until target
            SetState(target);
            return;
        }

        _currentState = target;

        // Start the target state
        PrivateOnEnterState(_currentState);

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
    public void PrevState()
    {
        SetState(_currentState.RunOnEnumAsInt(s => (s - 1 + StateCount) % StateCount));

        // Rush the current state and prevent it from being skipped automatically
        RushCurrentState();
        _autoSkipCurrentState = false;
    }

    public void RestartCurrentState()
    {
        foreach (FadeData data in _singleFades)
        {
            data.fading.StartFade();

            if (data.runWhileOnDelay || data.fading.delayTimeLeft <= 0f)
                data.func(data.fading.value, isExit: false);
        }

        OnEnterState(_currentState);
    }
    public void RushCurrentState()
    {
        foreach (FadeData data in _singleFades)
        {
            data.fading.RushFade();
            data.func(data.fading.value, isExit: true);
        }
        _singleFades.Clear();
    }
    public void UpdateCurrentState(float deltaTime)
    {
        foreach (FadeData data in _singleFades.ToArray())
        {
            if (data.runWhileOnDelay || data.fading.delayTimeLeft <= 0f)
                data.func(data.fading.value, isExit: false);

            data.fading.UpdateFade(deltaTime);

            if (!data.fading.isFading)
            {
                // If fading is done, remove it from the set
                data.func(data.fading.value, isExit: true);
                _singleFades.Remove(data);
            }
        }
    }
    private void RunCurrentState()
    {
        foreach (FadeData data in _singleFades)
        {
            if (data.runWhileOnDelay || data.fading.delayTimeLeft <= 0f)
                data.func(data.fading.value, isExit: false);
        }
    }

    private void PrivateOnEnterState(T state)
    {
        OnEnterState(state);
        RunCurrentState();
    }
    private void PrivateBeforeExitState(T state)
    {
        RushCurrentState();
        BeforeExitState(state);
    }

    private void Update()
    {
        if (_autoSkipCurrentState)
        {
            _autoSkipTimeLeft -= Time.deltaTime;

            while (_autoSkipTimeLeft <= 0f && _autoSkipCurrentState)
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
            PrevState();
        }

        UpdateCurrentState(Time.deltaTime);

        OnUpdate();
    }

    private void Start()
    {
        OnStart();
        PrivateOnEnterState(_currentState);

        if (AutoSkipStates.TryGetValue(_currentState, out float autoSkipTime))
        {
            _autoSkipCurrentState = true;
            _autoSkipTimeLeft = autoSkipTime;
        }
    }
}
