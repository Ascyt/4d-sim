using System.Collections.Generic;
using UnityEngine;

public enum VideoHypercubesCState
{
    Start,
    XAxis,
    AddFirstVertex,
    DuplicateVertex,
    MakeLine,
    YAxis,
    DuplicateLine,
    MakeSquare,
    OrthographicToPerspective,
    ZAxis,
    ActualStart,
    DuplicateSquare,
    MakeCube,
    HighlightAndUnhighlightSideFaces,
    HighlightBackFace,
    RotateCubeToFirstEdge,
    RotateCubeToSide,
    RotateCubeToSecondEdge,
    RotateCubeToFront,
    UnhighlightBackFace,
}

[RequireComponent(typeof(Camera))]
public class VideoHypercubesC : AnimatedStateMachine<VideoHypercubesCState>
{
    [SerializeField]
    private GameObject centerSphereObject;
    [SerializeField]
    private GameObject xAxisObject;
    [SerializeField]
    private GameObject yAxisObject;
    [SerializeField]
    private GameObject zAxisObject;
    [SerializeField]
    private GameObject vertexObjectPrefab;
    [SerializeField]
    private LineRenderer lineRendererPrefab;
    [SerializeField]
    private GameObject highlightableCubePrefab;

    private readonly List<GameObject> currentHypercubeVertices = new();
    private readonly List<LineRenderer> currentHypercubeLines = new();
    private readonly List<GameObject> unconnectedHypercubeVertices = new();
    private GameObject hypercubeParent;

    private GameObject highlightedFaceObject;
    private readonly List<GameObject> highlightedFaceObjects = new();

    private Camera cam;

    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    private readonly Dictionary<VideoHypercubesCState, float> _autoSkipStates = new()
    {
        { VideoHypercubesCState.Start, 0f },
        { VideoHypercubesCState.XAxis, 0f },
        { VideoHypercubesCState.AddFirstVertex, 0f },
        { VideoHypercubesCState.DuplicateVertex, 0f },
        { VideoHypercubesCState.MakeLine, 0f },
        { VideoHypercubesCState.YAxis, 0f },
        { VideoHypercubesCState.DuplicateLine, 0f },
        { VideoHypercubesCState.MakeSquare, 0f },
        { VideoHypercubesCState.OrthographicToPerspective, 0f },
        { VideoHypercubesCState.ZAxis, 0f },
        { VideoHypercubesCState.ActualStart, 0f },
        { VideoHypercubesCState.DuplicateSquare, 0f },
    };

    protected override Dictionary<VideoHypercubesCState, float> AutoSkipStates => _autoSkipStates;


    protected override void OnEnterState(VideoHypercubesCState state)
    {
        switch (state)
        {
            case VideoHypercubesCState.Start:
                OnStart();
                return;

            case VideoHypercubesCState.XAxis:
                xAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        xAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        xAxisObject.transform.localPosition = new Vector3(fadingValue * 4f, 0, 0);
                    });

                return;

            case VideoHypercubesCState.AddFirstVertex:
                GameObject vertex = Instantiate(vertexObjectPrefab, new Vector3(3, 0, 0), Quaternion.identity);

                vertex.SetActive(true);
                currentHypercubeVertices.Add(vertex);
                currentHypercubeVertices[0].transform.localPosition = new Vector3(3, 0, 0);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        currentHypercubeVertices[0].transform.localScale = new Vector3(.0625f, .0625f, .0625f) * fadingValue;
                    });
                return;

            case VideoHypercubesCState.DuplicateVertex:
                DuplicateCurrentHypercube(new Vector3(2, 0, 0), 1f);
                return;

            case VideoHypercubesCState.MakeLine:
                ConnectHypercubes();
                return;

            case VideoHypercubesCState.YAxis:
                yAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        yAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        yAxisObject.transform.localPosition = new Vector3(0, fadingValue * 4f, 0);
                    });
                return;

            case VideoHypercubesCState.DuplicateLine:
                DuplicateCurrentHypercube(new Vector3(0, 2, 0), 1f);
                return;

            case VideoHypercubesCState.MakeSquare:
                ConnectHypercubes();
                return;

            case VideoHypercubesCState.OrthographicToPerspective:
                Fade(new Fading(0.5f, new Easing(Easing.Type.Expo, Easing.IO.Out)),
                    (fadingValue, isExit) =>
                    {
                        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -3 * (99f * (1 - fadingValue) + 1));
                        cam.fieldOfView = 60f / (99f * (1 - fadingValue) + 1);
                    });
                return;

            case VideoHypercubesCState.ZAxis:
                zAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        zAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        zAxisObject.transform.localPosition = new Vector3(0, 0, fadingValue * 4f);
                    });
                return;

            case VideoHypercubesCState.ActualStart:
                transform.position = new Vector3(4f, 1f, -3f);
                centerSphereObject.SetActive(false);
                return;

            case VideoHypercubesCState.DuplicateSquare:
                DuplicateCurrentHypercube(new Vector3(0, 0, 2), 1f);
                return;

            case VideoHypercubesCState.MakeCube:
                ConnectHypercubes();
                return;

            case VideoHypercubesCState.HighlightAndUnhighlightSideFaces:
                // Hypercube parent
                hypercubeParent = new GameObject("HypercubeParent");
                hypercubeParent.transform.position = new Vector3(4f, 1f, 1f);
                foreach (GameObject vertex1 in currentHypercubeVertices)
                {
                    vertex1.transform.SetParent(hypercubeParent.transform, true);
                }
                foreach (LineRenderer line in currentHypercubeLines)
                {
                    line.transform.SetParent(hypercubeParent.transform, true);
                }

                AddFace(new Vector3(4f, 2f, 1f), new Vector3(2f, 0f, 2f), 0.0f); // top
                AddFace(new Vector3(5f, 1f, 1f), new Vector3(0f, 2f, 2f), 0.5f); // right
                AddFace(new Vector3(4f, 0f, 1f), new Vector3(2f, 0f, 2f), 1.0f); // bottom
                AddFace(new Vector3(3f, 1f, 1f), new Vector3(0f, 2f, 2f), 1.5f); // left

                void AddFace(Vector3 position, Vector3 scale, float delay)
                {
                    GameObject newFace = Instantiate(highlightableCubePrefab, position, Quaternion.identity);
                    newFace.transform.localScale = scale;
                    newFace.SetActive(true);

                    highlightedFaceObjects.Add(newFace);

                    MeshRenderer mr = newFace.GetComponent<MeshRenderer>();
                    mr.material = new Material(mr.material);
                    Material mat = mr.material;

                    Fade(new Fading(0.5f, new Easing(Easing.Type.Sine, Easing.IO.InOut), delay),
                        (fadingValue, isExit) =>
                        {
                            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, fadingValue * .5f);
                        });

                    Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), delay + 0.5f),
                        (fadingValue, isExit) =>
                        {
                            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, (1f - fadingValue) * .5f);

                            if (isExit)
                            {
                                highlightedFaceObjects.Remove(newFace);
                                Destroy(mr.material);
                                Destroy(newFace);
                            }
                        }, runWhileOnDelay:false);
                }
                return;

            case VideoHypercubesCState.HighlightBackFace:
                if (highlightedFaceObject != null)
                {
                    Destroy(highlightedFaceObject);
                }
                highlightedFaceObject = Instantiate(highlightableCubePrefab, new Vector3(4f, 1f, 2f), Quaternion.identity);
                highlightedFaceObject.transform.localScale = new Vector3(2f, 2f, 0f);
                highlightedFaceObject.SetActive(true);
                highlightedFaceObject.transform.SetParent(hypercubeParent.transform, true);

                MeshRenderer mr = highlightedFaceObject.GetComponent<MeshRenderer>();
                mr.material = new Material(mr.material);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        Material mat = mr.material;
                        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, fadingValue * .5f);
                    });
                return;

            case VideoHypercubesCState.RotateCubeToFirstEdge:
                RotateCube(Quaternion.Euler(0, 45, 0));
                return;
            case VideoHypercubesCState.RotateCubeToSide:
                RotateCube(Quaternion.Euler(0, 90, 0));
                return;
            case VideoHypercubesCState.RotateCubeToSecondEdge:
                RotateCube(Quaternion.Euler(0, 135, 0));
                return;
            case VideoHypercubesCState.RotateCubeToFront:
                RotateCube(Quaternion.Euler(0, 180, 0));
                return;

            case VideoHypercubesCState.UnhighlightBackFace:
                MeshRenderer mr1 = highlightedFaceObject.GetComponent<MeshRenderer>();
                mr1.material = new Material(mr1.material);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        Material mat = mr1.material;
                        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, (1f - fadingValue) * .5f);

                        if (isExit)
                        {
                            Destroy(mr1.material);
                            Destroy(highlightedFaceObject);
                        }
                    });

                return;
        }
    }

    private void RotateCube(Quaternion endRotation)
    {
        Quaternion startRotation = hypercubeParent.transform.rotation;

        Fade(new Fading(2f, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
            (fadingValue, isExit) =>
            {
                hypercubeParent.transform.rotation = Quaternion.Slerp(startRotation, endRotation, fadingValue);
            });
    }

    protected override void BeforeExitState(VideoHypercubesCState state)
    {

    }

    private void DuplicateCurrentHypercube(Vector3 positionDelta, float scaleMultiplier)
    {
        List<GameObject> objsToMove = new();

        Vector3 total = Vector3.zero;
        int count = 0;
        foreach (GameObject vertex in currentHypercubeVertices)
        {
            GameObject newVertex = Instantiate(vertex, vertex.transform.position, Quaternion.identity);

            unconnectedHypercubeVertices.Add(newVertex);

            objsToMove.Add(newVertex);

            total += newVertex.transform.position;
            count++;
        }
        Vector3 averagePosition = count > 0 ? total / count : Vector3.zero;

        foreach (LineRenderer line in currentHypercubeLines.ToArray())
        {
            LineRenderer newLine = Instantiate(line.gameObject, line.gameObject.transform.position, Quaternion.identity).GetComponent<LineRenderer>();

            currentHypercubeLines.Add(newLine);

            objsToMove.Add(newLine.gameObject);
        }

        GameObject parent = new("TempParent");
        parent.transform.position = averagePosition;
        foreach (GameObject obj in objsToMove)
        {
            obj.transform.SetParent(parent.transform, true);
        }

        Fade(DefaultFading,
            (fadingValue, isExit) =>
            {
                parent.transform.position = averagePosition + positionDelta * fadingValue;
                parent.transform.localScale = Vector3.one + (fadingValue * (scaleMultiplier - 1f) * Vector3.one);

                if (isExit)
                {
                    while (parent.transform.childCount > 0)
                    {
                        Transform child = parent.transform.GetChild(0);
                        child.SetParent(null, true);
                    }

                    Destroy(parent);
                }
            });
    }
    private void ConnectHypercubes()
    {
        List<LineRenderer> lines = new();

        // First positions
        foreach (GameObject vertex in currentHypercubeVertices)
        {
            LineRenderer newLine = Instantiate(lineRendererPrefab.gameObject).GetComponent<LineRenderer>();
            newLine.gameObject.transform.position = vertex.transform.position;
            newLine.positionCount = 2;
            newLine.SetPosition(0, Vector3.zero);

            lines.Add(newLine);
        }
        // Second positions
        int index = 0;
        foreach (GameObject vertex in unconnectedHypercubeVertices)
        {
            Vector3 secondPosition = vertex.transform.position;
            LineRenderer currentLine = lines[index];

            GameObject obj = currentLine.gameObject;

            Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), (float)index / unconnectedHypercubeVertices.Count),
                (fadingValue, isExit) =>
                {
                    LineRenderer lr = obj.GetComponent<LineRenderer>();

                    currentLine.SetPosition(1, Vector3.Lerp(Vector3.zero, secondPosition - lr.transform.position, fadingValue));
                });

            index++;
        }

        currentHypercubeLines.AddRange(lines);
        currentHypercubeVertices.AddRange(unconnectedHypercubeVertices);
        unconnectedHypercubeVertices.Clear();
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    protected override void OnStart()
    {
        centerSphereObject.SetActive(true);
        xAxisObject.SetActive(false);
        yAxisObject.SetActive(false);
        zAxisObject.SetActive(false);

        vertexObjectPrefab.SetActive(false);

        cam.transform.SetPositionAndRotation(new Vector3(2.5f, 1, -3 * 100f), Quaternion.identity);
        cam.fieldOfView = 60f / 100f;

        foreach (GameObject obj in currentHypercubeVertices)
        {
            Destroy(obj);
        }
        currentHypercubeVertices.Clear();
        foreach (LineRenderer lr in currentHypercubeLines)
        {
            Destroy(lr.gameObject);
        }
        currentHypercubeLines.Clear();
        foreach (GameObject obj in unconnectedHypercubeVertices)
        {
            Destroy(obj);
        }
        unconnectedHypercubeVertices.Clear();

        foreach (GameObject obj in highlightedFaceObjects)
        {
            Destroy(obj);
        }
        highlightedFaceObjects.Clear();

        if (highlightedFaceObject != null)
        {
            Destroy(highlightedFaceObject);
            highlightedFaceObject = null;
        }

        if (hypercubeParent != null)
        {
            Destroy(hypercubeParent);
            hypercubeParent = null;
        }
    }
}
