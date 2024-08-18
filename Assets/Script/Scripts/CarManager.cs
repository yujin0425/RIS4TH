using System.Collections;
using System.Collections.Generic;
using NRKernal;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    public GameObject CarPrefab;
    public ReticleBehaviour Reticle;
    public CarBehaviour Car;

    private GameObject LockedPlane;

    private void Update()
    {
        if (Car == null && WasTapped() && Reticle.CurrentPlane != null)
        {
            // Spawn our car at the reticle location.
            var obj = Instantiate(CarPrefab);
            Car = obj.GetComponent<CarBehaviour>();
            Car.Reticle = Reticle;
            Car.transform.position = Reticle.transform.position;
        }
    }

    private bool WasTapped()
    {
        return NRInput.GetButtonDown(ControllerButton.TRIGGER);
    }
}