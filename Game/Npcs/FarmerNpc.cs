using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FarmerNpc : MonoBehaviour, Agent {
    private readonly float speed = 1.0f;
    private readonly GoapPlanner planner = new GoapPlanner(NpcTypes.FARMER);
    private readonly Blackboard blackboard = new Blackboard();
    private PlanExecutor planExecutor = new PlanExecutor(this);

    void Start() {
        AgentManager.AddAgent(this);
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

        this.planExecutor.AddNewPlan(plan);
    }

    public void OnFarmCommandIssued() {
        Building nearestPossibleFarm = BuildingSensor.FindNearest(BuildingTypes.FARM, this.transform);
        if (nearestPossibleFarm is Farm && nearestPossibleFarm is MonoBehaviour) {
            MonoBehaviour farmBehaviour = nearestPossibleFarm as MonoBehaviour;
            currentAction = () => {
                float step = this.speed * Time.deltaTime;
                this.transform.position = Vector3.MoveTowards(this.transform.position, farmBehaviour.transform.position, step);
            };
        }
    }
}