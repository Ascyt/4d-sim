using System.Collections.Generic;
using UnityEngine;

public enum VideoHypercubesHypersceneState
{
    Start,
    RotateTesseractToFirstEdge,
    RotateTesseractToSide,
    RotateTesseractToSecondEdge,
    RotateTesseractToFront,
    RotateTesseractBackToStart
}

public class VideoHypercubesHypersceneInteractivity : AnimatedStateMachine<VideoHypercubesHypersceneState>
{
    public static VideoHypercubesHypersceneInteractivity Instance { get; private set; }

    public Quatpair tesseractRotation = Quatpair.identity;

    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    private readonly Dictionary<VideoHypercubesHypersceneState, float> _autoSkipStates = new()
    {

    };
    protected override Dictionary<VideoHypercubesHypersceneState, float> AutoSkipStates => _autoSkipStates;

    protected override void OnEnterState(VideoHypercubesHypersceneState state)
    {
        switch (state)
        {
            case VideoHypercubesHypersceneState.Start:
                OnStart();
                return;

            case VideoHypercubesHypersceneState.RotateTesseractToFirstEdge:
                RotateTesseract(Quatpair.Euler(45, 0, 0, 0, 0, 0));
                return;
            case VideoHypercubesHypersceneState.RotateTesseractToSide:
                RotateTesseract(Quatpair.Euler(90, 0, 0, 0, 0, 0));
                return;
            case VideoHypercubesHypersceneState.RotateTesseractToSecondEdge:
                RotateTesseract(Quatpair.Euler(135, 0, 0, 0, 0, 0));
                return;
            case VideoHypercubesHypersceneState.RotateTesseractToFront:
                RotateTesseract(Quatpair.Euler(180, 0, 0, 0, 0, 0));
                return;
            case VideoHypercubesHypersceneState.RotateTesseractBackToStart:
                RotateTesseract(Quatpair.Euler(0, 0, 0, 0, 0, 0), time:4f);
                return;

        }
    }

    private void RotateTesseract(Quatpair endRotation, float time=2f)
    {
        Quatpair startRotation = tesseractRotation;

        Fade(new Fading(time, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
            (fadingValue, isExit) =>
            {
                tesseractRotation = Quatpair.Lerp(startRotation, endRotation, fadingValue);
            });
    }

    protected override void BeforeExitState(VideoHypercubesHypersceneState state)
    {

    }

    protected override void OnStart()
    {
        tesseractRotation = Quatpair.identity;
    }

    private void Awake()
    {
        Instance = this;
    }
}
