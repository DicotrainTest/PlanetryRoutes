using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class GenerateRoutes : MonoBehaviour {

    public static GenerateRoutes Instance { get; private set; }

    public event EventHandler OnEndingPointGenerated;
    public event EventHandler OnStartingPointAndEndingPointVec3Changed;

    private List<Airport> startingPointInteractedAirportHistoryList;
    private List<Airport> endingPointInteractedAirportHistoryList;

    [SerializeField] private GameObject routeCalculatorPrefab;

    public Vector3 StartingPointVec3;
    public Vector3 EndingPointVec3;

    private MouseTarget mouseTarget;

    private Airport startingPointInteractedAirport;
    private Airport endingPointInteractedAirport;

    private GameObject routeCalculator;

    private int routeNum;

    private enum State {

        Idle,
        PlacingEndingPoint,
    }

    private State state;

    private void Awake() {

        if (Instance != null) {

            Debug.Log("There is more than one GenerateRoutes Instance");
        }

        Instance = this;

        startingPointInteractedAirportHistoryList = new List<Airport>();
        endingPointInteractedAirportHistoryList = new List<Airport>();
    }

    private void Start() {
        
        mouseTarget = MouseTarget.Instance.GetComponent<MouseTarget>();
    }

    private void Update() {

        switch (state) {

            case State.Idle: {
                    break;
                }

            case State.PlacingEndingPoint: {

                    EndingPointVec3 = mouseTarget.routePreviewPoint;
                    OnStartingPointAndEndingPointVec3Changed?.Invoke(this, EventArgs.Empty);
                    break;
                }
        }
    }

    private void GenerateStartingPoint() {

        StartingPointVec3 = mouseTarget.routeStartingAirport;

        state = State.PlacingEndingPoint;

        Debug.Log("starting point");

        routeNum++;

        routeCalculator = Instantiate(routeCalculatorPrefab, Vector3.zero, Quaternion.identity);

        routeCalculator.name = "route (" + routeNum + ")";

        OnStartingPointAndEndingPointVec3Changed?.Invoke(this, EventArgs.Empty);
    }
    private void GenerateEndingPoint() {

        bool hasFoundOverlapping = false;

        if (startingPointInteractedAirportHistoryList.Count == endingPointInteractedAirportHistoryList.Count) {

            for (int i = 0; i < startingPointInteractedAirportHistoryList.Count; i++) {

                if (startingPointInteractedAirport == startingPointInteractedAirportHistoryList[i] && endingPointInteractedAirport == endingPointInteractedAirportHistoryList[i] || startingPointInteractedAirport == endingPointInteractedAirportHistoryList[i] && endingPointInteractedAirport == startingPointInteractedAirportHistoryList[i]) {

                    hasFoundOverlapping = true;
                }
            }

            EndingPointVec3 = mouseTarget.routeEndingAirport;

            OnStartingPointAndEndingPointVec3Changed?.Invoke(this, EventArgs.Empty);

            OnEndingPointGenerated?.Invoke(this, EventArgs.Empty);

            state = State.Idle;

            if (!hasFoundOverlapping) {

                //add this route to startingAirport and endingAirport
                startingPointInteractedAirport.AddRoutesInThisAirport(routeCalculator, true);
                startingPointInteractedAirportHistoryList.Add(startingPointInteractedAirport);

                endingPointInteractedAirport.AddRoutesInThisAirport(routeCalculator, false);
                endingPointInteractedAirportHistoryList.Add(endingPointInteractedAirport);
            } else {

                Destroy(routeCalculator);
            }
        } else {

            Debug.LogError("startingPointInteractedAirportHistoryList.Count & endingPointInteractedAirportHistoryList.Count is not same");
        }
    }

    public void StartGeneratingRoute(Airport interactedAirport) {

        startingPointInteractedAirport = interactedAirport;
        GenerateStartingPoint();
    }

    public void EndGeneratingRoute(Airport interactedAirport) {

        endingPointInteractedAirport = interactedAirport;
        GenerateEndingPoint();
    }
}
