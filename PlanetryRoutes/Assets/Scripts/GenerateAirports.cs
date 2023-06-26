using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAirports : MonoBehaviour {

    [SerializeField] private Transform airportPrefab;
    [SerializeField] private GameInput gameInput;

    private MouseTarget mouseTarget;

    private int airportNameIdx;

    private void Start() {

        gameInput.OnPlaceAirportAction += GameInput_OnPlaceAirportAction;

        mouseTarget = MouseTarget.Instance.GetComponent<MouseTarget>();
    }

    private void GameInput_OnPlaceAirportAction(object sender, System.EventArgs e) {

        PlaceAirport("Airport" + " " + "(" + airportNameIdx + ")");

        airportNameIdx++;
    }

    private void PlaceAirport(string airportName) {

        Transform airportInstantiated = Instantiate(airportPrefab, mouseTarget.mousePoint, Quaternion.identity);
        airportInstantiated.gameObject.name = airportName;
    }
}
