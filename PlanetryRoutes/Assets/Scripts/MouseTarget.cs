using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseTarget : MonoBehaviour {

    public static MouseTarget Instance { get; private set; }

    public event EventHandler<OnSelectedAirportChangedEventArgs> OnSelectedAirportChanged;
    public class OnSelectedAirportChangedEventArgs : EventArgs {

        public Airport selectedAirport;
    }

    [SerializeField] private Transform RoutePreviewEndingPointPoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private int groundLayerNumber;
    [SerializeField] private int airportsLayerNumber;
    [SerializeField] private GameInput gameInput;

    public Vector3 mousePoint;

    public Vector3 routeStartingAirport;
    public Vector3 routeEndingAirport;

    private Airport selectedAirport;

    private enum State {

        StartingPoint,
        EndingPoint,
    }

    private State state;

    private void Awake() {

        if (Instance != null) {

            Debug.LogError("There is more than one MouseTarget Instance");
        }
        Instance = this;
    }

    private void Start() {

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

        mousePoint = RoutePreviewEndingPointPoint.position;
    }

    private void HandleGroundInteraction(RaycastHit groundRayCastHit) {

        transform.position = groundRayCastHit.point;

        mousePoint = RoutePreviewEndingPointPoint.position;
    }

    private void SetSelectedAirport(Airport selectedAirport) {
        this.selectedAirport = selectedAirport;

        OnSelectedAirportChanged?.Invoke(this, new OnSelectedAirportChangedEventArgs {
            selectedAirport = selectedAirport
        });
    }
}