using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airport : MonoBehaviour {

    [SerializeField] private Transform aircraftPrefab;
    [SerializeField] private Transform routeGenerationPoint;

    private List<GameObject> routesInThisAirportList;
    private List<bool> isStartingPointList;

    private GenerateRoutes generateRoutes;

    private Transform aircraft;

    private void Awake() {

        routesInThisAirportList = new List<GameObject>();
        isStartingPointList = new List<bool>();
    }

    private void Start() {

        generateRoutes = GenerateRoutes.Instance.GetComponent<GenerateRoutes>();
    }

    public void StartPointInteract() {

        generateRoutes.StartGeneratingRoute(this);
    }

    public void EndPointInteract() {

        generateRoutes.EndGeneratingRoute(this);
    }

    public void AddRoutesInThisAirport(GameObject route, bool isStartingPoint) {

        routesInThisAirportList.Add(route);
        isStartingPointList.Add(isStartingPoint);
    }

    public void SpawnAircraft() {

        if (routesInThisAirportList.Count > 0) {

            if (routesInThisAirportList.Count == isStartingPointList.Count) {

                for (int i = 0; i < routesInThisAirportList.Count; i++) {

                    aircraft = Instantiate(aircraftPrefab);
                    aircraft.SetParent(routesInThisAirportList[i].transform, false);
                    aircraft.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                    Aircraft bezierCurveAircraft = aircraft.GetComponent<Aircraft>();

                    bezierCurveAircraft.SetIsMovingForward(isStartingPointList[i]);
                }
            } else {

                Debug.LogError("routesInThisAirportList.Count & isStartingPointList.Count is not same");
            }
        }
    }

    public Vector3 GetRouteGenerationPoint() {

        return routeGenerationPoint.position;
    }
}
