using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAirports : MonoBehaviour {

    private Transform closestPlanet;

    [SerializeField] private Transform airportPrefab;
    [SerializeField] private Transform[] planets;

    private GameInput gameInput;

    private MouseTarget mouseTarget;

    private int airportNameIdx;

    private void Start() {

        gameInput = GameInput.Instance.GetComponent<GameInput>();
        mouseTarget = MouseTarget.Instance.GetComponent<MouseTarget>();

        gameInput.OnPlaceAirportAction += GameInput_OnPlaceAirportAction;
    }

    private void GameInput_OnPlaceAirportAction(object sender, System.EventArgs e) {

        if (mouseTarget.IsGameObjectThatsTouchingMousePointerIsInGroundLayer()) {

            PlaceAirport("Airport" + " " + "(" + airportNameIdx + ")");

            airportNameIdx++;
        }
    }

    private void PlaceAirport(string airportName) {

        Transform airportInstantiated = Instantiate(airportPrefab, mouseTarget.placeAirportPoint, Quaternion.identity);
        airportInstantiated.gameObject.name = airportName;
        float max = float.MaxValue;

        foreach (Transform t in planets) {

            float dist = Vector3.Distance(airportInstantiated.position, t.position);

            if (dist < max) {

                closestPlanet = t;
                max = dist;
            }
        }

        airportInstantiated.LookAt(closestPlanet);
        airportInstantiated.localEulerAngles += Vector3.left * 90;
    }
}
