using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour {

    private GameInput gameInput;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private float normalCameraStateMouseSensitivity = 100f;
    [SerializeField] private float orbitingCameraStateMouseSensitivity = 100f;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float planetViewDistance = 50f;
    [SerializeField] private Transform viewingPlanet;

    private float XRotation = 0f;
    private float YRotation = 0f;

    private float mouseX;
    private float mouseY;

    private enum CameraMovementState {

        Normal,
        Orbiting,
    }

    private CameraMovementState cameraMovementState;

    private void Awake() {
        
        cameraMovementState = CameraMovementState.Orbiting;
    }

    private void Start() {

        gameInput = GameInput.Instance.GetComponent<GameInput>();
    }

    private void Update() {

        switch (cameraMovementState) {

            case CameraMovementState.Normal: {

                HandlerNormalCameraMovement();
                break;
            }

            case CameraMovementState.Orbiting: {

                HandleOrbitingCameraMovement();
                break;
            }
        }
    }

    //-normal-
    private void HandlerNormalCameraMovement() {

        HandleCameraRotation();
        HandlerCameraMovement();
    }

    private void HandleCameraRotation() {

        if (gameInput.HoldingRightMouseButton()) {
            //holding right mouse button

            Cursor.lockState = CursorLockMode.Locked;

            Vector2 mouseInputVector = gameInput.GetMouseAxisRaw();

            mouseX = mouseInputVector.x * normalCameraStateMouseSensitivity * Time.deltaTime;
            mouseY = mouseInputVector.y * normalCameraStateMouseSensitivity * Time.deltaTime;

            YRotation += mouseX;
            XRotation -= mouseY;

            XRotation = Mathf.Clamp(XRotation, -90, 90);

            //camera
            transform.rotation = Quaternion.Euler(XRotation, YRotation, 0);
        } else {
            //not holding right mouse button

            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void HandlerCameraMovement() {

        Vector2 inputVector = gameInput.GetWASDInputVector();

        Vector3 moveDir = transform.right * inputVector.x + transform.forward * inputVector.y;

        moveDir = moveDir.normalized;

        transform.position += moveDir * speed * Time.deltaTime;
    }

    //-orbiting-
    private void HandleOrbitingCameraMovement() {

        Cursor.lockState = CursorLockMode.Locked;

        if (gameInput.HoldingRightMouseButton()) {

            Vector2 inputVector = gameInput.GetMouseAxisRaw();

            mouseX = inputVector.x * orbitingCameraStateMouseSensitivity * Time.deltaTime;
            mouseY = inputVector.y * orbitingCameraStateMouseSensitivity * Time.deltaTime;
        } else {

            Cursor.lockState = CursorLockMode.None;

            mouseX = 0;
            mouseY = 0;
        }

        YRotation += mouseX;
        XRotation -= mouseY;

        XRotation = Mathf.Clamp(XRotation, -90, 90);

        float viewingPlanetRadius = viewingPlanet.gameObject.GetComponent<Planet>().shapeSettings.planetRadius;

        transform.localEulerAngles = new Vector3(XRotation, YRotation, 0);

        transform.position = viewingPlanet.position - transform.forward * (viewingPlanetRadius + planetViewDistance);
    }
}
