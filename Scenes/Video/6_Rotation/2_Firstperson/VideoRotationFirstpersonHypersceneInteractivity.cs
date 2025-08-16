using System.Collections.Generic;
using UnityEngine;

public enum VideoRotationFirstpersonHypersceneState
{
    Start,
    HighlightCell,
    UnhighlightCell,
    End
}

public class VideoRotationFirstpersonHypersceneInteractivity : AnimatedStateMachine<VideoRotationFirstpersonHypersceneState>
{
    public static VideoRotationFirstpersonHypersceneInteractivity Instance { get; private set; }

    public Color highlightedCellColor = new Color(1f, 0f, 1f, 0f);

    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    private readonly Dictionary<VideoRotationFirstpersonHypersceneState, float> _autoSkipStates = new()
    {

    };
    protected override Dictionary<VideoRotationFirstpersonHypersceneState, float> AutoSkipStates => _autoSkipStates;

    protected override HashSet<KeyCode> NextKeys => new() { KeyCode.LeftAlt };
    protected override HashSet<KeyCode> PrevKeys => new() { };

    protected override void OnEnterState(VideoRotationFirstpersonHypersceneState state)
    {
        switch (state)
        {
            case VideoRotationFirstpersonHypersceneState.Start:
                OnStart();
                return;

            case VideoRotationFirstpersonHypersceneState.HighlightCell:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        highlightedCellColor = new Color(highlightedCellColor.r, highlightedCellColor.g, highlightedCellColor.b, Mathf.Lerp(0f, .5f, fadingValue));
                    });
                return;

            case VideoRotationFirstpersonHypersceneState.UnhighlightCell:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        highlightedCellColor = new Color(highlightedCellColor.r, highlightedCellColor.g, highlightedCellColor.b, Mathf.Lerp(.5f, 0f, fadingValue));
                    });
                return;

            case VideoRotationFirstpersonHypersceneState.End:
                SetState(VideoRotationFirstpersonHypersceneState.HighlightCell);
                return;
        }
    }

    protected override void BeforeExitState(VideoRotationFirstpersonHypersceneState state)
    {

    }

    protected override void OnStart()
    {
        highlightedCellColor = new Color(1f, 0f, 1f, 0f);
    }

    private void Awake()
    {
        Instance = this;
    }
}
