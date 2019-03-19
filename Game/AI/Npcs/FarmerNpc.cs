using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FarmerNpc : MonoBehaviour, Npc, GoapAgent {
    private float speed;
    private readonly GoapPlanner planner = new GoapPlanner(NpcType.FARMER.ToString());    
    private PlanExecutor planExecutor;

    private readonly Blackboard blackboard = new Blackboard();
    Blackboard GoapAgent.blackboard {
        get { return blackboard; }
    }

    public FarmerNpc() {
        this.planExecutor = new PlanExecutor(this);
    }

    void Start() {
        NpcManager.AddNpc(this);
        this.speed = UnityEngine.Random.Range(3.0f, 6.0f);
    }

    void Update() {
        this.planExecutor.Execute();
    }

    public bool HasPlan() {
        return this.planExecutor.HasPlan();
    }

    public void Plan() {
        Goal selectedGoal = GoalPool.GoalsFor(NpcType.FARMER).OrderBy(goal => goal.priority).ToList().FirstOrDefault();
        List<GoapAction> plan = planner.Plan(ActionPool.ActionsFor, this.blackboard.worldState, selectedGoal.goalState).Where(action => action != null).ToList();
       
        StringBuilder sb = new StringBuilder("Planning finished. Path length: " + plan.Count + ", Path is: \n");

        foreach (GoapAction action in plan) {
            sb.Append(action.ToString() + "\n");
        }
        sb.Append("\n");

        Debug.Log(sb.ToString());

        plan.Reverse();

        this.planExecutor.AddNewPlan(new Stack<GoapAction>(plan));
    }

    public static Func<int> PriorityForGoal(GoalType goalType) {
        return new Func<int>(() => {
                if (goalType == GoalType.HAVE_HOUSE) {
                    return 1;
                } else {
                    return Int32.MaxValue;
                }
            }
        );
    }

    public void Kill() {
        NpcManager.RemoveAgent(this);
        Destroy(this.gameObject);
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
            Vector3 vectorToShop = transform.position - shopBehaviour.transform.position;
            return vectorToShop.magnitude <= 1.0f;
        }
        return false;
    }

    public void OnBuyHouseCommandIssued() {
        Building nearestPossibleShop = BuildingSensor.FindNearest(BuildingTypes.SHOP, this.transform);
        if (nearestPossibleShop != null && nearestPossibleShop is Shop && nearestPossibleShop is MonoBehaviour) {
            MonoBehaviour shopBehaviour = nearestPossibleShop as MonoBehaviour;
            float step = this.speed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, shopBehaviour.transform.position, step);
        }
    }

    public bool IsBuyHouseCommandCompleted() {
        Building nearestPossibleShop = BuildingSensor.FindNearest(BuildingTypes.SHOP, this.transform);
        if (nearestPossibleShop != null && nearestPossibleShop is Shop && nearestPossibleShop is MonoBehaviour) {
            MonoBehaviour shopBehaviour = nearestPossibleShop as MonoBehaviour;
            Vector3 vectorToShop = transform.position - shopBehaviour.transform.position;
            return vectorToShop.magnitude <= 1.0f;
        }
        return false;
    }

    public void OnBuildHouseCommandIssued() {
        Vector3 nearestPossibleNewHouseLocation = BuildingSensor.FindNearestBuildSpot(BuildingTypes.HOUSE, this.transform);
        if (nearestPossibleNewHouseLocation != null) {
            float step = this.speed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, nearestPossibleNewHouseLocation, step);
        }
    }

    public bool IsBuildHouseCommandCompleted() {
        Vector3 nearestPossibleNewHouseLocation = BuildingSensor.FindNearestBuildSpot(BuildingTypes.HOUSE, this.transform);
        if (nearestPossibleNewHouseLocation != null) {
            Vector3 vectorToHouseLocation = transform.position - nearestPossibleNewHouseLocation;
            return vectorToHouseLocation.magnitude <= 1.0f;
        }
        return false;
    }

    public void OnCommandCompleted(Dictionary<string, object> newState, string actionType) {
        Debug.Log("Farmer completed command: " + actionType);
        
        foreach (KeyValuePair<string, object> kvPair in newState) {
            blackboard.SetWorldStateVariable(kvPair.Key, kvPair.Value);
        }

        if (actionType == ActionType.BUILD_HOUSE.ToString()) {
            GameObject housePrefab = Resources.Load("Prefabs/House") as GameObject;
            GameObject.Instantiate(housePrefab, this.transform.position, this.transform.rotation);
            this.blackboard.SetWorldStateVariable(WorldStateVariable.HAS_HOUSE.ToString(), false);
        }
    }
}