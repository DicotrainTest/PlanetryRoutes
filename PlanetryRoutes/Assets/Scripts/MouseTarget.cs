using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseTarget : MonoBehaviour {

    public static MouseTarget Instance { get; private set; }

    public event EventHandler<OnSelectedAirportChangedEventArgs> OnSelectedAirportChanged;
    public class OnSelectedAirportChangedEventArgs : EventArgs {

        public Airport selectedAirport;
    }

    [Header("where previewing route ends")]
    [SerializeField] private Transform RoutePreviewEndingPointPoint;

    [Header("main camera")]
    [SerializeField] private Camera mainCamera;

    [Header("layers")]
    [SerializeField] private int groundLayerNumber;
    [SerializeField] private int airportsLayerNumber;

    private GameInput gameInput;

    private PlanetHandler planetHandler;

    private Vector3 routePreviewPoint;
    private Vector3 placeAirportPoint;

    private Vector3 routeStartingAirport;
    private Vector3 routeEndingAirport;

    private Airport selectedAirport;

    private enum State {

        StartingPoint,
        EndingPoint,
    }

    private State state;

    private RaycastHit mousePositionRaycastHit;

    private void Awake() {

        if (Instance != null) {

            Debug.LogError("There is more than one MouseTarget Instance");
        }
        Instance = this;
    }

    private void Start() {

        gameInput = GameInput.Instance.GetComponent<GameInput>();

        planetHandler = PlanetHandler.Instance.GetComponent<PlanetHandler>();

        gameInput.OnSpawnAircraftAction += GameInput_OnSpawnAircraftAction;
    }

    private void GameInput_OnSpawnAircraftAction(object sender, EventArgs e) {

        if (selectedAirport != null) {

            selectedAirport.SpawnAircraft();
        }
    }

    private void Update() {

        HandleInteraction();
        HandleAddingRoute();
    }

    private void HandleInteraction() {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit)) {

            mousePositionRaycastHit = raycastHit;

            if (raycastHit.transform.gameObject.layer == airportsLayerNumber) {
                //this is airport

                HandleAirportsInteraction(raycastHit);
            } else {

                if (raycastHit.transform.gameObject.layer == groundLayerNumber) {
                    //this is ground

                    SetSelectedAirport(null);

                    HandleGroundInteraction(raycastHit);
                }
            }
        }
    }

    private void HandleAddingRoute() {

        if (gameInput.LeftMouseButtonDown()) {

            if (selectedAirport != null) {

                switch (state) {

                    case State.StartingPoint: {
                            state = State.EndingPoint;
                            routeStartingAirport = selectedAirport.GetRouteGenerationPoint();
                            selectedAirport.StartPointInteract();
                            break;
                        }
                    case State.EndingPoint: {

                            if (selectedAirport.GetRouteGenerationPoint() != routeStartingAirport) {
                                state = State.StartingPoint;
                                routeEndingAirport = selectedAirport.GetRouteGenerationPoint();
                                selectedAirport.EndPointInteract();
                                break;
                            } else {

                                break;
                            }
                        }
                }
            }
        }
    }

    private void HandleAirportsInteraction(RaycastHit airportsRayCastHit) {

        if (airportsRayCastHit.transform.TryGetComponent(out Airport airport)) {

            //has airport
            if (airport != selectedAirport) {

                SetSelectedAirport(airport);
            }
        } else {

            SetSelectedAirport(null);
        }

        transform.position = airportsRayCastHit.point;

        placeAirportPoint = transform.position;
        routePreviewPoint = RoutePreviewEndingPointPoint.position;
    }

    private void HandleGroundInteraction(RaycastHit groundRayCastHit) {

        transform.position = groundRayCastHit.point;

        placeAirportPoint = transform.position;
        routePreviewPoint = RoutePreviewEndingPointPoint.position;
    }

    private void SetSelectedAirport(Airport selectedAirport) {

        this.selectedAirport = selectedAirport;

        OnSelectedAirportChanged?.Invoke(this, new OnSelectedAirportChangedEventArgs {
            selectedAirport = selectedAirport
        });
    }

    public bool IsGameObjectThatsTouchingMousePointerIsInGroundLayer() {

        if (mousePositionRaycastHit.transform != null) {

            return mousePositionRaycastHit.transform.gameObject.layer == groundLayerNumber;
        }

        return false;
    }

    public Planet GetPlanetThatMouseIsOn() {

        float closestPlanetDistance = planetHandler.GetClosestPlanetDistance(RoutePreviewEndingPointPoint.position);
        Planet closestPlanet = planetHandler.GetClosestPlanet(RoutePreviewEndingPointPoint.position);

        Debug.Log(closestPlanetDistance - closestPlanet.shapeSettings.planetRadius <= closestPlanet.GetPlanetDetectableMaxMouseDistance());
        if (closestPlanetDistance - closestPlanet.shapeSettings.planetRadius <= closestPlanet.GetPlanetDetectableMaxMouseDistance()) {

            return closestPlanet;
        } else {

            return null;
        }
    }

    public Vector3 GetRoutePreviewPoint() {

        return routePreviewPoint;
    }

    public Vector3 GetRouteStartingAirport() {

        return routeStartingAirport;
    }

    public Vector3 GetRouteEndingAirport() {

        return routeEndingAirport;
    }

    public Vector3 GetPlaceAirportPoint() {

        return placeAirportPoint;
    }
}