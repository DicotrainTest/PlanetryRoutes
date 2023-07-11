using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateRoutes : MonoBehaviour {

    public static GenerateRoutes Instance { get; private set; }

    private List<Airport> startingPointInteractedAirportHistoryList;
    private List<Airport> endingPointInteractedAirportHistoryList;

    [Header("prefab of routeCalculator to spawn")]
    [SerializeField] private Transform routeCalculatorPrefab;

    private Vector3 startingPointVec3;
    private Vector3 endingPointVec3;

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

                    endingPointVec3 = mouseTarget.GetRoutePreviewPoint();

                    Planet planetThatMouseIsOn = mouseTarget.GetPlanetThatMouseIsOn();

                    Debug.Log(planetThatMouseIsOn);

                    routeCalculator.GetComponent<RouteCalculator>().SetStartingAndEndingPlanet(startingPointInteractedAirport.GetPlacedPlanet(), planetThatMouseIsOn);
                    routeCalculator.GetComponent<RouteCalculator>().Vec3sChanged(startingPointVec3, endingPointVec3);
                    break;
                }
        }
    }

    private void GenerateStartingPoint() {

        startingPointVec3 = mouseTarget.GetRouteStartingAirport();

        state = State.PlacingEndingPoint;

        routeNum++;

        routeCalculator = Instantiate(routeCalculatorPrefab.gameObject, Vector3.zero, Quaternion.identity);

        routeCalculator.name = "route (" + routeNum + ")";

        Planet planetThatMouseIsOn = mouseTarget.GetPlanetThatMouseIsOn();

        routeCalculator.GetComponent<RouteCalculator>().SetStartingAndEndingPlanet(startingPointInteractedAirport.GetPlacedPlanet(), planetThatMouseIsOn);
        routeCalculator.GetComponent<RouteCalculator>().Vec3sChanged(startingPointVec3, endingPointVec3);
    }
    private void GenerateEndingPoint() {

        bool hasFoundOverlapping = false;

        if (startingPointInteractedAirportHistoryList.Count == endingPointInteractedAirportHistoryList.Count) {

            for (int i = 0; i < startingPointInteractedAirportHistoryList.Count; i++) {

                if (startingPointInteractedAirport == startingPointInteractedAirportHistoryList[i] && endingPointInteractedAirport == endingPointInteractedAirportHistoryList[i] || startingPointInteractedAirport == endingPointInteractedAirportHistoryList[i] && endingPointInteractedAirport == startingPointInteractedAirportHistoryList[i]) {

                    hasFoundOverlapping = true;
                }
            }

            endingPointVec3 = mouseTarget.GetRouteEndingAirport();
            routeCalculator.GetComponent<RouteCalculator>().Vec3sChanged(startingPointVec3, endingPointVec3);

            state = State.Idle;

            routeCalculator.GetComponent<RouteCalculator>().SetStartingAndEndingPlanet(startingPointInteractedAirport.GetPlacedPlanet(), endingPointInteractedAirport.GetPlacedPlanet());

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
