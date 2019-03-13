using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FarmerNpc : MonoBehaviour, Agent {
    private float speed;
    private readonly GoapPlanner planner = new GoapPlanner(NpcTypes.FARMER);
    private readonly Blackboard blackboard = new Blackboard();
    private PlanExecutor planExecutor;

    public FarmerNpc() {
        this.planExecutor = new PlanExecutor(this);
    }

    void Start() {
        AgentManager.AddAgent(this);
        this.speed = Random.Range(1.0f, 6.0f);
    }
    
    void Update() {
        this.planExecutor.Execute();
    }

    public bool HasPlan() {
        return this.planExecutor.HasPlan();
    }

    public void Plan() {
        List<GoapAction> plan = planner.Plan(this.blackboard.worldState, GoalSelector.SelectGoal(NpcTypes.FARMER)).Where(action => action != null).ToList();

        Debug.Log("Planning finished. Path length: " + plan.Count + ", Path is: \n");
        foreach (GoapAction action in plan) {
            Debug.Log(action.ToString());
        }
        Debug.Log("\n");

        plan.Reverse();

        this.planExecutor.AddNewPlan(new Stack<GoapAction>(plan));
    }

    public void OnFarmCommandIssued() {
        Building nearestPossibleFarm = BuildingSensor.FindNearest(BuildingTypes.FARM, this.transform);
        if (nearestPossibleFarm != null && nearestPossibleFarm is Farm && nearestPossibleFarm is MonoBehaviour) {
            MonoBehaviour farmBehaviour = nearestPossibleFarm as MonoBehaviour;
            float step = this.speed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, farmBehaviour.transform.position, step);
        }
    }

    public bool IsFarmCommandComplete() {
        Building nearestPossibleFarm = BuildingSensor.FindNearest(BuildingTypes.FARM, this.transform);
        if (nearestPossibleFarm != null && nearestPossibleFarm is Farm && nearestPossibleFarm is MonoBehaviour) {
            MonoBehaviour farmBehaviour = nearestPossibleFarm as MonoBehaviour;
            Vector3 vectorToFarm = transform.position - farmBehaviour.transform.position;
            return vectorToFarm.magnitude <= 1.0f;
        }
        return false;
    }

    public void OnSellProduceCommandIssued() {
        Building nearestPossibleShop = BuildingSensor.FindNearest(BuildingTypes.SHOP, this.transform);
        if (nearestPossibleShop != null && nearestPossibleShop is Shop && nearestPossibleShop is MonoBehaviour) {
            MonoBehaviour shopBehaviour = nearestPossibleShop as MonoBehaviour;
            float step = this.speed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, shopBehaviour.transform.position, step);
        }
    }

    public bool IsSellProduceCommandCompleted() {
        Building nearestPossibleShop = BuildingSensor.FindNearest(BuildingTypes.SHOP, this.transform);
        if (nearestPossibleShop != null && nearestPossibleShop is Shop && nearestPossibleShop is MonoBehaviour) {
            MonoBehaviour shopBehaviour = nearestPossibleShop as MonoBehaviour;
            Vector3 vectorToFarm = transform.position - shopBehaviour.transform.position;
            return vectorToFarm.magnitude <= 1.0f;
        }
        return false;
    }
}