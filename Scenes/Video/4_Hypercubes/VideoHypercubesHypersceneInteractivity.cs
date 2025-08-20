using System.Collections.Generic;
using UnityEngine;

public enum VideoHypercubesHypersceneState
{
    Start,
    HighlightFrontCell,
    HighlightBackCell,
    UnhighlightBackCell,
    HighlightAndUnhighlightSideCells,
    HighlightBackCellAgain,
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

    public Color[] highlightedCellColors = new Color[]
    {
        new(1f, 0f, 1f, 0f),
        new(1f, 0f, 1f, 0f),
        new(1f, 0f, 1f, 0f),
        new(1f, 0f, 1f, 0f),
        new(1f, 0f, 1f, 0f),
        new(1f, 0f, 1f, 0f),
    };
    public Color highlightedFrontCellColor = new Color(1f, 0f, 1f, 0f);
    public Color highlightedBackCellColor = new Color(1f, 0f, 1f, 0f);

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

            case VideoHypercubesHypersceneState.HighlightFrontCell:
                // Highlight front
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        highlightedFrontCellColor = new Color(highlightedFrontCellColor.r, highlightedFrontCellColor.g, highlightedFrontCellColor.b, Mathf.Lerp(0f, .5f, fadingValue));
                    });
                return;
            case VideoHypercubesHypersceneState.HighlightBackCell:
                // Unhighlight front
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        highlightedFrontCellColor = new Color(highlightedBackCellColor.r, highlightedBackCellColor.g, highlightedBackCellColor.b, Mathf.Lerp(.5f, 0f, fadingValue));
                    });

                // Highlight back
                Fade(DefaultFading.WithDelay(1f),
                    (fadingValue, isExit) =>
                    {
                        highlightedBackCellColor = new Color(highlightedBackCellColor.r, highlightedBackCellColor.g, highlightedBackCellColor.b, Mathf.Lerp(0f, .5f, fadingValue));
                    });
                return;

            case VideoHypercubesHypersceneState.UnhighlightBackCell:
                // Unhighlight back
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        highlightedBackCellColor = new Color(highlightedBackCellColor.r, highlightedBackCellColor.g, highlightedBackCellColor.b, Mathf.Lerp(.5f, 0f, fadingValue));
                    });
                return;

            case VideoHypercubesHypersceneState.HighlightAndUnhighlightSideCells:
                // Highlight sides
                for (int i = 0; i < highlightedCellColors.Length; i++)
                {
                    int index = i;

                    Fade(DefaultFading.WithDuration(f => f.fadeDuration / 2f).WithDelay(index / 2f),
                    (fadingValue, isExit) =>
                    {
                        highlightedCellColors[index] = new Color(highlightedCellColors[index].r, highlightedCellColors[index].g, highlightedCellColors[index].b, Mathf.Lerp(0f, .5f, fadingValue));

                        if (isExit)
                        {
                            Fade(DefaultFading,
                                (exitFadingValue, exitIsExit) =>
                                {
                                    highlightedCellColors[index] = new Color(highlightedCellColors[index].r, highlightedCellColors[index].g, highlightedCellColors[index].b, Mathf.Lerp(.5f, 0f, exitFadingValue));
                                });
                        }
                    });
                }
                return;

            case VideoHypercubesHypersceneState.HighlightBackCellAgain:
                // Highlight back again
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        highlightedBackCellColor = new Color(highlightedBackCellColor.r, highlightedBackCellColor.g, highlightedBackCellColor.b, Mathf.Lerp(0f, .5f, fadingValue));
                    });
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
                tesseractRotation = Quatpair.Slerp(startRotation, endRotation, fadingValue);
            });
    }

    protected override void BeforeExitState(VideoHypercubesHypersceneState state)
    {

    }

    protected override void OnStart()
    {
        tesseractRotation = Quatpair.identity;

        for (int i = 0; i < highlightedCellColors.Length; i++)
        {
            highlightedCellColors[i] = new Color(1f, 0f, 1f, 0f);
        }
        highlightedFrontCellColor = new Color(1f, 0f, 1f, 0f);
        highlightedBackCellColor = new Color(1f, 0f, 1f, 0f);
    }

    private void Awake()
    {
        Instance = this;
    }
}
