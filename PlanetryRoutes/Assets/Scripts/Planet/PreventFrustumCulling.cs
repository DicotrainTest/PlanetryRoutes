using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventFrustumCulling : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;

    private MeshRenderer meshRenderer;

    private void Start() {
        
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update() {
        
        if (mainCamera == null || meshRenderer == null) return;

        Bounds adjustedBounds = meshRenderer.bounds;
        adjustedBounds.center = mainCamera.transform.position + (mainCamera.transform.forward * (mainCamera.farClipPlane - mainCamera.nearClipPlane) * 0.5f);
        adjustedBounds.extents = new Vector3(0.1f, 0.1f, 0.1f);

        meshRenderer.bounds = adjustedBounds;
    }
}
