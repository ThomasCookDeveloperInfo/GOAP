using UnityEngine;

public class Farm : MonoBehaviour, Building {
    void Start() {
        BuildingSensor.AddBuilding(this);
    }

    void Update() {

    }
}