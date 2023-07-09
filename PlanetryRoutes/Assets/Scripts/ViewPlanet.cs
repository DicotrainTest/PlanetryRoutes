using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ViewPlanet : MonoBehaviour {

    [Header("reference")]
    [SerializeField] private Transform planet;

    [Header("variable that can be changed")]
    [SerializeField] private float planetViewDistance = 10f;

    [SerializeField] private float XYPlaneDir;
    [SerializeField] private float XZPlaneDir;

    [SerializeField] private float speed = 1f;

    private float planetRadius;

        private void Start() {

        planetRadius = planet.gameObject.GetComponent<Planet>().shapeSettings.planetRadius;
    }

    private void Update() {

        MovePlayerByOrbit();

        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W)) {

            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S)) {

            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.A)) {

            inputVector.x = -1;
        }
        if (Input.GetKey(KeyCode.D)) {

            inputVector.x = +1;
        }

        inputVector = inputVector.normalized;

        XYPlaneDir += inputVector.y * speed * Time.deltaTime;
        XZPlaneDir += inputVector.x * speed * Time.deltaTime;
    }

    private void MovePlayerByOrbit() {

        float r = planetRadius + planetViewDistance;

        float x = (r * (Mathf.Cos(XYPlaneDir) * Mathf.Cos(XZPlaneDir))) + planet.position.x;
        float y = r * Mathf.Sin(XYPlaneDir) + planet.position.y;
        float z = r * (Mathf.Cos(XYPlaneDir) * Mathf.Sin(XZPlaneDir)) + planet.position.z;

        transform.position = new Vector3(x, y, z);

        transform.LookAt(planet, transform.up);
    }
}
