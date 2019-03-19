using UnityEngine;

public class House : MonoBehaviour, Building {
    void Start() {
        BuildingSensor.AddBuilding(this);

        GameObject farmerPrefab = Resources.Load("Prefabs/Farmer") as GameObject;
        GameObject.Instantiate(farmerPrefab, this.transform.position, this.transform.rotation);
    }

    void Update() {

    }
}