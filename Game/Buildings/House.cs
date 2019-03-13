using UnityEngine;

public class House : MonoBehaviour, Building {
    void Start() {
        BuildingSensor.AddBuilding(this);
    }

    void Update() {

    }
}