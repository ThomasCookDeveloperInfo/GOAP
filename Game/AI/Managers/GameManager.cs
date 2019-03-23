using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private void Start() {
        Fortunes fortunes = new Fortunes();

        List<FortuneSite> sites = new List<FortuneSite> {
            new FortuneSite(100, 200),
            new FortuneSite(500, 200),
            new FortuneSite(300, 300)
        };

        LinkedList<Edge> edges = fortunes.GenerateVoronoi(sites, 0, 0, 800, 800);

        Debug.Log("Edges size: " + edges.Count);
    }

    void Update() {
        NpcManager.Update();
    }
}

