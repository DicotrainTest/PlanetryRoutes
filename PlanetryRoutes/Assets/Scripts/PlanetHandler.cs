using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetHandler : MonoBehaviour {

    public static PlanetHandler Instance { get; private set; }

    [Header("every planet that you have in game")]
    [SerializeField] private Planet[] planets;

    private void Awake() {

        if (Instance != null) {

            Debug.LogError("There is more than one PlanetHandler Instance");
        }
        Instance = this;
    }

    public Planet GetClosestPlanet(Vector3 position) {

        Planet closestPlanet = null;

        float max = float.MaxValue;

        foreach (Planet planet in planets) {

            float dist = Vector3.Distance(position, planet.transform.position);

            if (dist < max) {

                closestPlanet = planet;
                max = dist;
            }
        }

        return closestPlanet;
    }

    public float GetClosestPlanetDistance(Vector3 position) {

        Planet closestPlanet = null;

        float max = float.MaxValue;

        foreach (Planet planet in planets) {

            float dist = Vector3.Distance(position, planet.transform.position);

            if (dist < max) {

                closestPlanet = planet;
                max = dist;
            }
        }

        return max;
    }
}
