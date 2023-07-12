using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Aircraft : MonoBehaviour {

    private RouteCalculator routeCalculator;

    private PlanetHandler planetHandler;

    private Vector3 targetPosition;
    private Vector3 currentPosition;

    [Header("settings for aircraft")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turnDistance = 0.01f; 

    private bool isMovingForward;

    private void Start() {

        routeCalculator = GetComponentInParent<RouteCalculator>();

        planetHandler = PlanetHandler.Instance.GetComponent<PlanetHandler>();

        targetPosition = routeCalculator.GetNextWaypoint(targetPosition, true, isMovingForward);
        transform.position = targetPosition;

        SelectNextWaypoint();
    }

    private void Update() {
        
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < turnDistance) {

            SelectNextWaypoint();
        }
    }

    private void SelectNextWaypoint() {

        currentPosition = targetPosition;

        targetPosition = routeCalculator.GetNextWaypoint(targetPosition, false, isMovingForward);
        //routeCalculator
        transform.LookAt(targetPosition);

        if (currentPosition == targetPosition) {
            //U-Turning

            if (isMovingForward) {

                isMovingForward = false;
            } else {

                isMovingForward = true;
            }
        }
    }

    public void SetIsMovingForward(bool setIsMovingForward) {

        isMovingForward = setIsMovingForward;
    }
}
