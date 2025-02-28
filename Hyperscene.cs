using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hyperscene : MonoBehaviour
{
    public static Hyperscene instance { get; private set; }

    public readonly List<Hyperobject> objects = new();
    private List<GameObject> instantiatedObjects = new();

    private CameraPosition cameraPos;

    private void Awake()
    {
        instance = this;
        cameraPos = GetComponent<CameraPosition>();
        cameraPos.onValuesUpdate += RenderObjects;
    }
    private void Start()
    {
        objects.Add(new Tesseract());
    }

    public void RenderObjects()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Hyperobject obj = objects[i];

            for (int ii = 0; ii < obj.hypermesh.Length; ii++)
            {
                Tetrahedron t = obj.hypermesh[ii];
                // TODO: Continue code
            }
        }
    }
}
