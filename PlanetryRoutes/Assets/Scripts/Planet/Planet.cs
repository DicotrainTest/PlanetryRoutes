using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class Planet : MonoBehaviour {

    [Range(2, 256)]
    [SerializeField] private int resolution = 10;

    public bool autoUpdate = true;

    public enum FaceRenderMask {

        All,
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    }

    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    ShapeGenerator shapeGenerator;
    ColorGenerator colorGenerator;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilter;
    TerrainFace[] terrainFace;

    void Initialize() {

        shapeGenerator = new ShapeGenerator(shapeSettings);
        colorGenerator = new ColorGenerator(colorSettings);

        if (meshFilter == null || meshFilter.Length == 0) {

            meshFilter = new MeshFilter[6];
        }

        terrainFace = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++) {

            if (meshFilter[i] == null) {

                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilter[i] = meshObj.AddComponent<MeshFilter>();
                meshFilter[i].sharedMesh = new Mesh();
            }

            meshFilter[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            terrainFace[i] = new TerrainFace(shapeGenerator, meshFilter[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilter[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet() {

        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated() {

        if (autoUpdate) {

            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated() {

        if (autoUpdate) {

            Initialize();
            GenerateColors();
        }
    }

    void GenerateMesh() {

        for (int i = 0; i < 6; i++) {

            if (meshFilter[i].gameObject.activeSelf) {

                terrainFace[i].ConstructMesh();
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColors() {

        foreach (MeshFilter m in meshFilter) {

            m.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColor;
        }
    }
}
