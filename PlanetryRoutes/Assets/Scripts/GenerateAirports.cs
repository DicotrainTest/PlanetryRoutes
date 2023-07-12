using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAirports : MonoBehaviour {

    [Header("prefab of airport to spawn")]
    [SerializeField] private Transform airportPrefab;

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

        Transform airportInstantiated = Instantiate(airportPrefab, mouseTarget.GetPlaceAirportPoint(), Quaternion.identity);
        airportInstantiated.gameObject.name = airportName;

        Planet closestPlanet = PlanetHandler.Instance.GetClosestPlanet(airportInstantiated.position);

        airportInstantiated.GetComponent<Airport>().SetPlacedPlanet(closestPlanet);
        closestPlanet.MatchRotationWithSurface(airportInstantiated);
    }
}
