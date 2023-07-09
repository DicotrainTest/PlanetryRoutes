using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class RouteCalculator : MonoBehaviour {

    private List<Vector3> positionSaveList;

    private const string LINERENDERER_NAME = "lineRenderer";

    [Header("settings for route")]
    [SerializeField] private Color color;
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineQuality;
    [SerializeField] private LayerMask groundLayerMask;

    private GenerateRoutes generateRoutes;
    private LineRenderer lineRenderer;

    private Vector3 startingPoint;
    private Vector3 middlePoint;
    private Vector3 endingPoint;

    private Vector3 checkingRouteLength1;
    private Vector3 checkingRouteLength2;

    private float routeLength;

    private int numberOfPoints = 15;

    private enum RouteState {

        bezierCurve,
        groundLine,
    }

    private RouteState routeState;

    private void Awake() {

        routeState = RouteState.groundLine;

        positionSaveList = new List<Vector3>();
    }

    private void Start() {

        lineRenderer = new GameObject(LINERENDERER_NAME).AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(transform, false);
        lineRenderer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        lineRenderer.useWorldSpace = true;

        generateRoutes = GenerateRoutes.Instance.GetComponent<GenerateRoutes>();
        generateRoutes.OnStartingPointAndEndingPointVec3Changed += GenerateRoutes_OnStartingPointAndEndingPointVec3Changed;
        generateRoutes.OnEndingPointGenerated += GenerateRoutes_OnEndingPointGenerated;
    }

    private void GenerateRoutes_OnStartingPointAndEndingPointVec3Changed(object sender, System.EventArgs e) {

        startingPoint = generateRoutes.StartingPointVec3;
        endingPoint = generateRoutes.EndingPointVec3;
    }

    private void GenerateRoutes_OnEndingPointGenerated(object sender, System.EventArgs e) {

        ClearEvents();
    }

    private void Update() {

        switch (routeState) {

            case RouteState.bezierCurve: {

                DrawBezierCurve();
                break;
            }

            case RouteState.groundLine: {

                DrawGroundLine();
                break;
            }
        }

        HandleLineLengthAndQuality();
    }

    private void DrawBezierCurve() {

        float averageOfStartingPointYAndEndingPointY = (startingPoint.y + endingPoint.y) / 2;

        middlePoint.x = (startingPoint.x + endingPoint.x) / 2;
        middlePoint.y = (averageOfStartingPointYAndEndingPointY + height);
        middlePoint.z = (startingPoint.z + endingPoint.z) / 2;

        if (lineRenderer == null || startingPoint == null || middlePoint == null || endingPoint == null) {
            return; // no points specified
        }

        // update line renderer
        UpdateLineRenderer(true, lineMaterial, color, width);

        if (numberOfPoints > 1) {

            lineRenderer.positionCount = numberOfPoints;
        } else {

            return;
        }

        // set points of quadratic Bezier curve
        Vector3 p0 = startingPoint;
        Vector3 p1 = middlePoint;
        Vector3 p2 = endingPoint;

        float t;
        Vector3 position;

        SetLengthOfPositionSaveList();

        //draw line
        for (int i = 0; i < numberOfPoints; i++) {

            t = i / (numberOfPoints - 1.0f);
            position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
            lineRenderer.SetPosition(i, position);

            positionSaveList[i] = position;
        }
    }

    private void DrawGroundLine() {

        if (lineRenderer == null || startingPoint == null || middlePoint == null || endingPoint == null) {
            return; // no points specified
        }

        //update line renderer
        UpdateLineRenderer(true, lineMaterial, color, width);

        if (numberOfPoints > 1) {

            lineRenderer.positionCount = numberOfPoints;
        } else {

            return;
        }

        SetLengthOfPositionSaveList();

        Vector3 p0 = startingPoint;
        Vector3 p1 = endingPoint;

        Vector3 position;

        //draw line
        for (int i = 0; i < numberOfPoints; i++) {

            position = Vector3.Lerp(p0, p1, i / (numberOfPoints - 1));

            position = AttachGroundLineToGround(position);

            lineRenderer.SetPosition(i, position);

            positionSaveList[i] = position;
        }
    }

    private Vector3 AttachGroundLineToGround(Vector3 position) {

        Vector3 checkPosition = position;

        float radius = 0.01f;

        Collider[] colliders = Physics.OverlapSphere(checkPosition, radius, groundLayerMask);

        Debug.Log(Physics.OverlapSphere(checkPosition, radius, groundLayerMask));

        if (colliders.Length <= 0) {
            //no collision detected
            while (colliders.Length <= 0) {

                checkPosition.y -= radius;
                colliders = Physics.OverlapSphere(checkPosition, radius, groundLayerMask);
            }

            return checkPosition;
        } else {

            if (colliders.Length > 0) {
                //collision detected

                while (colliders.Length > 0) {

                    checkPosition.y += radius;
                    colliders = Physics.OverlapSphere(checkPosition, radius, groundLayerMask);
                }

                return checkPosition;
            }

            return checkPosition;
        }
    }

    private void HandleLineLengthAndQuality() {

        routeLength = 0f;

        for (int i = 0; i < numberOfPoints - 1; i++) {

            checkingRouteLength1 = positionSaveList[i];
            checkingRouteLength2 = positionSaveList[i + 1];

            routeLength += Vector3.Distance(checkingRouteLength1, checkingRouteLength2);
        }

        numberOfPoints = (int)(Mathf.Ceil(routeLength * lineQuality));
    }

    private void ClearEvents() {

        generateRoutes.OnStartingPointAndEndingPointVec3Changed -= GenerateRoutes_OnStartingPointAndEndingPointVec3Changed;
        generateRoutes.OnEndingPointGenerated -= GenerateRoutes_OnEndingPointGenerated;
    }

    private void SetLengthOfPositionSaveList() {

        if (positionSaveList.Count < numberOfPoints) {

            for (int i = 0; i < numberOfPoints; i++) {

                positionSaveList.Add(Vector3.zero);
            }
        } else {

            if (positionSaveList.Count > numberOfPoints) {

                int differentBetweenLengthAndNumberOfPoints = positionSaveList.Count - numberOfPoints;
                for (int i = 0; i < differentBetweenLengthAndNumberOfPoints; i++) {

                    positionSaveList.RemoveAt(0);
                }
            }
        }
    }

    public Vector3 GetNextWaypoint(Vector3 currentPosition, bool isFirstTime, bool isMovingForward) {

        if (isMovingForward) {

            if (isFirstTime) {

                return positionSaveList[0];
            } else {

                for (int i = 0; i < numberOfPoints - 1; i++) {

                    if (currentPosition == positionSaveList[i]) {

                        return positionSaveList[i + 1];
                    }
                }
            }

            return positionSaveList[numberOfPoints - 1];
        } else {

            if (isFirstTime) {

                return positionSaveList[numberOfPoints - 1];
            } else {

                for (int i = numberOfPoints - 1; i > 0; i--) {

                    if (currentPosition == positionSaveList[i]) {

                        return positionSaveList[i - 1];
                    }
                }
            }

            return positionSaveList[0];
        }
    }

    private void UpdateLineRenderer(bool lineRendererEnabled, Material lineRendererMaterial, Color lineRendererColor, float lineRendererWidth) {

        lineRenderer.enabled = lineRendererEnabled;
        lineRenderer.material = lineRendererMaterial;
        lineRenderer.startColor = lineRendererColor;
        lineRenderer.endColor = lineRendererColor;
        lineRenderer.startWidth = lineRendererWidth;
        lineRenderer.endWidth = lineRendererWidth;
    }
}
