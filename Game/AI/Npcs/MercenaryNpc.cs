using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MercenaryNpc : MonoBehaviour, Npc, GoapAgent {
    private float speed;
    private PlanExecutor planExecutor;
    private readonly GoapPlanner planner = new GoapPlanner(NpcType.MERCENARY.ToString());

    private readonly Blackboard blackboard = new Blackboard();
    Blackboard GoapAgent.blackboard {
        get { return blackboard; }
    }

    public MercenaryNpc() {
        this.blackboard.SetWorldStateVariable(WorldStateVariable.HAS_MONEY.ToString(), 100);
        this.planExecutor = new PlanExecutor(this);
    }

    void Start() {
        NpcManager.AddNpc(this);
        this.speed = UnityEngine.Random.Range(0.5f, 1.0f);
    }

    void Update() {
        this.planExecutor.Execute();
    }

    public bool HasPlan() {
        return this.planExecutor.HasPlan();
    }

    public void Plan() {
        Goal selectedGoal = GoalPool.GoalsFor(NpcType.MERCENARY).OrderBy(goal => goal.priority).ToList().FirstOrDefault();
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
            if (goalType == GoalType.HAVE_DEAD_TARGET) {
                return 1;
            } else {
                return Int32.MaxValue;
            }
        }
        );
    }

    public void OnBuyWeaponCommandIssued() {
        Building nearestPossibleShop = BuildingSensor.FindNearest(BuildingTypes.SHOP, this.transform);
        if (nearestPossibleShop != null && nearestPossibleShop is Shop && nearestPossibleShop is MonoBehaviour) {
            MonoBehaviour shopBehaviour = nearestPossibleShop as MonoBehaviour;
            float step = this.speed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, shopBehaviour.transform.position, step);
        }
    }

    public bool IsBuyWeaponCommandCompleted() {
        Building nearestPossibleShop = BuildingSensor.FindNearest(BuildingTypes.SHOP, this.transform);
        if (nearestPossibleShop != null && nearestPossibleShop is Shop && nearestPossibleShop is MonoBehaviour) {
            MonoBehaviour shopBehaviour = nearestPossibleShop as MonoBehaviour;
            Vector3 vectorToShop = transform.position - shopBehaviour.transform.position;
            return vectorToShop.magnitude <= 1.0f;
        }
        return false;
    }

    public bool IsAttackTargetCommandCompleted() {
        Npc nearestFarmer = NpcManager.FindNearest(NpcType.FARMER, this.transform);
        if (nearestFarmer != null && nearestFarmer is FarmerNpc && nearestFarmer is MonoBehaviour) {
            MonoBehaviour farmerBehaviour = nearestFarmer as MonoBehaviour;
            Vector3 vectorToFarmer = transform.position - farmerBehaviour.transform.position;
            return vectorToFarmer.magnitude <= 1.0f;
        }
        return false;
    }

    public void OnAttackTargetCommandIssued() {
        Npc nearestFarmer = NpcManager.FindNearest(NpcType.FARMER, this.transform);
        if (nearestFarmer != null && nearestFarmer is FarmerNpc && nearestFarmer is MonoBehaviour) {
            MonoBehaviour farmerBehaviour = nearestFarmer as MonoBehaviour;
            float step = this.speed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, farmerBehaviour.transform.position, step);
        }
    }

    public void OnCommandCompleted(Dictionary<string, object> newState, string actionType) {
        Debug.Log("Mercenary completed command: " + actionType);
        
        foreach (KeyValuePair<string, object> kvPair in newState) {
            blackboard.SetWorldStateVariable(kvPair.Key, kvPair.Value);
        }

        if (actionType == ActionType.ATTACK_TARGET.ToString()) {
            Npc nearestFarmer = NpcManager.FindNearest(NpcType.FARMER, this.transform);
            if (nearestFarmer is FarmerNpc) {
                (nearestFarmer as FarmerNpc).Kill();
                this.blackboard.SetWorldStateVariable(WorldStateVariable.TARGET_IS_DEAD.ToString(), false);
            }
        }
    }
}