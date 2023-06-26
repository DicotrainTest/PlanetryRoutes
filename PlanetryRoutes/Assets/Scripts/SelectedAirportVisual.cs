using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedAirportVisual : MonoBehaviour {

    [SerializeField] private Airport airport;
    [SerializeField] private GameObject visualGameObject;

    private void Start() {

        MouseTarget.Instance.OnSelectedAirportChanged += MouseTarget_OnSelectedAirportChanged;
    }

    private void MouseTarget_OnSelectedAirportChanged(object sender, MouseTarget.OnSelectedAirportChangedEventArgs e) {

        if (e.selectedAirport == airport) {

            Show();
        } else {

            Hide();
        }
    }

    private void Show() {

        visualGameObject.SetActive(true);
    }

    private void Hide() {

        visualGameObject.SetActive(false);
    }
}
