using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Planet : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    [SerializeField] private int groundLayer;

    [SerializeField] private float planetDetectableMaxMouseDistance;

    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColourGenerator colourGenerator = new ColourGenerator();

    GameObject[] meshObjects;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;

    MeshCollider[] meshColliders;
    TerrainFace[] terrainFaces;

    private void Awake() {

        Clear();
        GeneratePlanet();
    }


    void Initialize() {

        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if (meshObjects == null || meshObjects.Length == 0) {

            meshObjects = new GameObject[6];
        }

        if (meshFilters == null || meshFilters.Length == 0) {

            meshFilters = new MeshFilter[6];
        }

        if (meshColliders == null || meshColliders.Length == 0) {

            meshColliders = new MeshCollider[6];
        }

        if (terrainFaces == null || terrainFaces.Length == 0) {

            terrainFaces = new TerrainFace[6];
        }

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {

            if (meshObjects[i] != null) continue;

            meshObjects[i] = new GameObject("mesh");
            meshObjects[i].layer = groundLayer;
            meshObjects[i].transform.parent = transform;
            meshObjects[i].AddComponent<MeshRenderer>();
            meshFilters[i] = meshObjects[i].AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = new Mesh();
            meshColliders[i] = meshObjects[i].AddComponent<MeshCollider>();

            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;
            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, meshColliders[i], resolution, directions[i], transform);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet()
    {

        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColours() {

        colourGenerator.UpdateColours();

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }
    }

    public void Clear() {

        if (meshObjects != null) {

            if (meshObjects.Length > 0) {

                DestroyImmediate(transform.GetChild(0).gameObject);
                DestroyImmediate(transform.GetChild(0).gameObject);
                DestroyImmediate(transform.GetChild(0).gameObject);
                DestroyImmediate(transform.GetChild(0).gameObject);
                DestroyImmediate(transform.GetChild(0).gameObject);
                DestroyImmediate(transform.GetChild(0).gameObject);

                meshObjects = new GameObject[0];
                meshColliders = new MeshCollider[0];
                meshFilters = new MeshFilter[0];
                terrainFaces = new TerrainFace[0];

            }
        }
    }

    public float GetPlanetDetectableMaxMouseDistance() {

        return planetDetectableMaxMouseDistance;
    }

    public void MatchRotationWithSurface(Transform body) {

        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;

        body.rotation = targetRotation;
    }
}
