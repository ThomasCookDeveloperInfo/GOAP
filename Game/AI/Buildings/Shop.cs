using UnityEngine;

public class Shop : MonoBehaviour, Building {
    void Start() {
        BuildingSensor.AddBuilding(this);
    }

    void Update() {

    }
}