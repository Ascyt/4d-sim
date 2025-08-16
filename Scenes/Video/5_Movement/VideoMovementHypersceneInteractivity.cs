using System.Collections.Generic;
using UnityEngine;

public enum VideoMovementHypersceneState
{
    Start,
    HighlightCell,
    UnhighlightCell,
    End
}

public class VideoMovementHypersceneInteractivity : AnimatedStateMachine<VideoMovementHypersceneState>
{
    public static VideoMovementHypersceneInteractivity Instance { get; private set; }

    public Color highlightedCellColor = new Color(1f, 0f, 1f, 0f);

    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    private readonly Dictionary<VideoMovementHypersceneState, float> _autoSkipStates = new()
    {

    };
    protected override Dictionary<VideoMovementHypersceneState, float> AutoSkipStates => _autoSkipStates;

    protected override HashSet<KeyCode> NextKeys => new() { KeyCode.LeftAlt };
    protected override HashSet<KeyCode> PrevKeys => new() { };

    protected override void OnEnterState(VideoMovementHypersceneState state)
    {
        switch (state)
        {
            case VideoMovementHypersceneState.Start:
                OnStart();
                return;

            case VideoMovementHypersceneState.HighlightCell:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        highlightedCellColor = new Color(highlightedCellColor.r, highlightedCellColor.g, highlightedCellColor.b, Mathf.Lerp(0f, .5f, fadingValue));
                    });
                return;

            case VideoMovementHypersceneState.UnhighlightCell:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        highlightedCellColor = new Color(highlightedCellColor.r, highlightedCellColor.g, highlightedCellColor.b, Mathf.Lerp(.5f, 0f, fadingValue));
                    });
                return;

            case VideoMovementHypersceneState.End:
                SetState(VideoMovementHypersceneState.HighlightCell);
                return;
        }
    }

    protected override void BeforeExitState(VideoMovementHypersceneState state)
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
