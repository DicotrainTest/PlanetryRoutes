using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

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

    public bool LeftMouseButtonDown() {

        if (Input.GetMouseButtonDown(0)) {

            return true;
        } else {

            return false;
        }
    }

    public bool RightMouseButtonDown() {

        if (Input.GetMouseButtonDown(1)) {

            return true;
        } else {

            return false;
        }
    }

    public bool HoldingRightMouseButton() {

        if (Input.GetMouseButton(1)) {

            return true;
        } else {

            return false;
        }
    }

    public Vector2 GetWASDInputVector() {

        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();

        return inputVector;
    }

    public Vector2 GetMouseAxisRaw() {

        Vector2 mouseInputVector;

        mouseInputVector.x = Input.GetAxisRaw("Mouse X");
        mouseInputVector.y = Input.GetAxisRaw("Mouse Y");

        return mouseInputVector;
    }
}
