using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPlanet : MonoBehaviour {

    [Header("reference")]
    [SerializeField] private Transform planet;

    [Header("variable that can be changed")]
    [SerializeField] private float planetViewDistance = 200f;

    [Header("variable that cannot be changed")]
    [SerializeField] private float planetViewDistanceMultiplier = 100f;

    private float planetSize;

    private void Start() {

        planetSize = planet.localScale.x / planetViewDistanceMultiplier;
    }

    private void Update() {

        float dist = Vector3.Distance(planet.position, transform.position);

        transform.LookAt(planet);

        transform.position = Vector3.MoveTowards(transform.position, planet.position, dist - (planetViewDistance * planetSize));
    }
}
