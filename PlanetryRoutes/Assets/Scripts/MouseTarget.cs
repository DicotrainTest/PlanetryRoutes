using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTarget : MonoBehaviour {

    public static MouseTarget Instance { get; private set; }

    public event EventHandler<OnSelectedAirportChangedEventArgs> OnSelectedAirportChanged;
    public class OnSelectedAirportChangedEventArgs : EventArgs {

        public Airport selectedAirport;
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask GroundLayerMask;
    [SerializeField] private LayerMask AirportsLayerMask;
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

        if (Physics.Raycast(ray, out RaycastHit AirportsRayCastHit, float.MaxValue, AirportsLayerMask)) {

            HandleAirportsInteraction(AirportsRayCastHit);
        } else {

            SetSelectedAirport(null);

            if (Physics.Raycast(ray, out RaycastHit GroundRayCastHit, float.MaxValue, GroundLayerMask)) {

                HandleGroundInteraction(GroundRayCastHit);
            }
        }
    }

    private void HandleAddingRoute() {

        if (gameInput.PressedLeftMouseButton()) {

            if (selectedAirport != null) {

                switch (state) {

                    case State.StartingPoint: {
                            state = State.EndingPoint;
                            routeStartingAirport = selectedAirport.transform.position;
                            selectedAirport.StartPointInteract();
                            break;
                        }
                    case State.EndingPoint: {

                        if (selectedAirport.transform.position != routeStartingAirport) {
                            state = State.StartingPoint;
                            routeEndingAirport = selectedAirport.transform.position;
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

    private void HandleAirportsInteraction(RaycastHit AirportsRayCastHit) {

        if (AirportsRayCastHit.transform.TryGetComponent(out Airport airport)) {

            //has airport
            if (airport != selectedAirport) {

                SetSelectedAirport(airport);
            }
        } else {
            SetSelectedAirport(null);
        }

        mousePoint = AirportsRayCastHit.point;

        transform.position = mousePoint;
    }

    private void HandleGroundInteraction(RaycastHit GroundRayCastHit) {

        mousePoint = GroundRayCastHit.point;

        transform.position = mousePoint;
    }

    private void SetSelectedAirport(Airport selectedAirport) {
        this.selectedAirport = selectedAirport;

        OnSelectedAirportChanged?.Invoke(this, new OnSelectedAirportChangedEventArgs {
            selectedAirport = selectedAirport
        });
    }
}