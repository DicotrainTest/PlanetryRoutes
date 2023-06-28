using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPlanet : MonoBehaviour {

    [Header("reference")]
    [SerializeField] private Transform planet;

    [Header("variable that can be changed")]
    [SerializeField] private float planetViewDistance = 10f;

    private float planetRadius;

        private void Start() {

        planetRadius = planet.gameObject.GetComponent<Planet>().shapeSettings.planetRadius;
    }

    private void Update() {

        float dist = Vector3.Distance(planet.position, transform.position);

        transform.LookAt(planet);

        transform.position = Vector3.MoveTowards(transform.position, planet.position, dist - (planetViewDistance + planetRadius));
    }
}
