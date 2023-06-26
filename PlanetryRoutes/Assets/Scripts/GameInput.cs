using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public event EventHandler OnPlaceAirportAction;
    public event EventHandler OnSpawnAircraftAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        
        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Enable();

        playerInputActions.Player.PlaceAirport.performed += PlaceAirport_performed;
        playerInputActions.Player.SpawnAircraft.performed += SpawnAircraft_performed;
    }

    private void PlaceAirport_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {

        OnPlaceAirportAction?.Invoke(this, EventArgs.Empty);
    }

    private void SpawnAircraft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {

        OnSpawnAircraftAction?.Invoke(this, EventArgs.Empty);
    }

    public bool PressedLeftMouseButton() {

        if (Input.GetMouseButtonDown(0)) {

            return true;
        } else {

            return false;
        }
    }
}
