using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public enum VideoVisionState
{
    Start,
    AddSquareGrid,
    AddZVectorGrid,
    RemoveZVectorGrid,
    SquaresToCubes,
    AddMoreCubes,
    AddWVectorGrid,
    RemoveWVectorGrid
}

public class VideoVision : AnimatedStateMachine<VideoVisionState>
{
    [SerializeField]
    private GameObject cubeWireframePrefab;
    [SerializeField]
    private GameObject zVectorPrefab;
    [SerializeField]
    private GameObject wVectorPrefab;

    private readonly Fading defaultFading = new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));

    private readonly Dictionary<VideoVisionState, float> _autoSkipStates = new()
    {
        { VideoVisionState.SquaresToCubes, 1f }
    };
    protected override Dictionary<VideoVisionState, float> AutoSkipStates => _autoSkipStates;

    private const int GRID_SIZE = 6;
    private List<GameObject> _pixelObjects = new();
    private List<GameObject> _vectorObjects = new();

    protected override void OnEnterState(VideoVisionState state)
    {
        float offset = GRID_SIZE / 2f - 0.5f; // Center the grid around (0, 0)

        switch (state)
        {
            case VideoVisionState.Start:
                OnStart();
                return;
            case VideoVisionState.AddSquareGrid:
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    for (int x = 0; x < GRID_SIZE; x++)
                    {
                        GameObject newPixel = Instantiate(cubeWireframePrefab, new Vector3(x - offset, y - offset, 0), Quaternion.identity);

                        _ = InnerWireframeCubeRandomColor(newPixel);

                        _pixelObjects.Add(newPixel);

                        Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), (x + ((GRID_SIZE - 1) - y)) / (float)(GRID_SIZE) / 2f),
                            (fadingValue, isExit) =>
                            {
                                newPixel.transform.localScale = new Vector3(1, 1, 0) * fadingValue / 10f;
                            });
                    }
                }
                return;
            case VideoVisionState.AddZVectorGrid:
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    for (int x = 0; x < GRID_SIZE; x++)
                    {
                        GameObject newArrow = Instantiate(zVectorPrefab, new Vector3(x - offset, y - offset), Quaternion.Euler(-90f, 0, 0));

                        _vectorObjects.Add(newArrow);

                        Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), (x + ((GRID_SIZE - 1) - y)) / (float)(GRID_SIZE) / 2f),
                            (fadingValue, isExit) =>
                            {
                                newArrow.transform.localScale = new Vector3(.125f, .5f, .125f) * fadingValue / 2f;
                                newArrow.transform.position = new Vector3(newArrow.transform.position.x, newArrow.transform.position.y, -.25f * fadingValue);
                            });
                    }
                }
                return;
            case VideoVisionState.RemoveZVectorGrid:
                foreach (GameObject gameObject in _vectorObjects)
                {
                    GameObject obj = gameObject;

                    Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
                        (fadingValue, isExit) =>
                        {
                            obj.transform.localScale = new Vector3(.125f, .5f, .125f) * (1f - fadingValue) / 2f;
                            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -.25f * (1f - fadingValue));
                            if (isExit)
                            {
                                foreach (GameObject arrow in _vectorObjects)
                                {
                                    Destroy(arrow);
                                }
                                _vectorObjects.Clear();
                            }
                        });
                }
                return;
            case VideoVisionState.SquaresToCubes:
                foreach (GameObject gameObject in _pixelObjects)
                {
                    GameObject obj = gameObject;

                    Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
                        (fadingValue, isExit) =>
                        {
                            obj.transform.localScale = new Vector3(1, 1, fadingValue) / 10f;
                        });
                }
                return;
            case VideoVisionState.AddMoreCubes:
                for (int z = 0; z < GRID_SIZE - 1; z++)
                {
                    for (int y = 0; y < GRID_SIZE; y++)
                    {
                        for (int x = 0; x < GRID_SIZE; x++)
                        {
                            GameObject newCube = Instantiate(cubeWireframePrefab, new Vector3(x - offset, y - offset, 0), Quaternion.identity);

                            MeshRenderer innerCubeRenderer = InnerWireframeCubeRandomColor(newCube);
                            innerCubeRenderer.gameObject.transform.localScale = (1f - 1f / 16f) * 10f * Vector3.one; 
                            int localZ = z;

                            _pixelObjects.Add(newCube);

                            Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), (float)localZ),
                                (fadingValue, isExit) =>
                                {
                                    innerCubeRenderer.material.color = new Color(
                                        innerCubeRenderer.material.color.r,
                                        innerCubeRenderer.material.color.g,
                                        innerCubeRenderer.material.color.b,
                                        fadingValue);

                                    innerCubeRenderer.gameObject.transform.localScale = (1f - ((1f - fadingValue) / 16f)) * 10f * Vector3.one; // Prevent z-fighting

                                    if (fadingValue > 0.0f)
                                    {
                                        newCube.transform.position = new Vector3(newCube.transform.position.x, newCube.transform.position.y, localZ + fadingValue);
                                    }
                                });
                        }
                    }
                }
                return;
            case VideoVisionState.AddWVectorGrid:
                for (int z = 0; z < GRID_SIZE; z++)
                {
                    for (int y = 0; y < GRID_SIZE; y++)
                    {
                        for (int x = 0; x < GRID_SIZE; x++)
                        {
                            GameObject newArrow = Instantiate(wVectorPrefab, new Vector3(x - offset, y - offset, z), Quaternion.Euler(10, 45, 0));

                            _vectorObjects.Add(newArrow);

                            Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), (x + ((GRID_SIZE - 1) - y)) / (float)(GRID_SIZE) / 2f),
                                (fadingValue, isExit) =>
                                {
                                    newArrow.transform.localScale = fadingValue / 2f * Vector3.one;
                                });
                        }
                    }
                }
                return;
            case VideoVisionState.RemoveWVectorGrid:
                foreach (GameObject gameObject in _vectorObjects)
                {
                    GameObject obj = gameObject;

                    Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
                        (fadingValue, isExit) =>
                        {
                            obj.transform.localScale = (1 - fadingValue) / 2f * Vector3.one;

                            if (isExit)
                            {
                                foreach (GameObject arrow in _vectorObjects)
                                {
                                    Destroy(arrow);
                                }
                                _vectorObjects.Clear();
                            }
                        });
                }
                return;
        }
    }

    private MeshRenderer InnerWireframeCubeRandomColor(GameObject parent)
    {
        MeshRenderer meshRenderer = parent.GetComponentInChildren<MeshRenderer>();

        if (Random.value > 1f / 8f)
        {
            meshRenderer.gameObject.SetActive(false);
            return meshRenderer;
        }

        meshRenderer.material.color = new Color(
            (float)Random.value,
            (float)Random.value,
            (float)Random.value,
            1f);

        return meshRenderer;
    }

    protected override void BeforeExitState(VideoVisionState state)
    {

    }

    protected override void OnStart()
    {
        foreach (GameObject gameObject in _pixelObjects)
        {
            Destroy(gameObject);
        }
        _pixelObjects.Clear();
        foreach (GameObject gameObject in _vectorObjects)
        {
            Destroy(gameObject);
        }
        _vectorObjects.Clear();

        Random.InitState(5);
    }

    private void LateUpdate()
    { 
        if (CurrentState == VideoVisionState.AddWVectorGrid || CurrentState == VideoVisionState.RemoveWVectorGrid)
        {
            foreach (GameObject gameObject in _vectorObjects)
            {
                gameObject.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(new Vector3(360 / 16f, 0, 0) * Time.deltaTime);
            }
        }
    }
}
