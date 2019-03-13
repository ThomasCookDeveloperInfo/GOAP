using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MercenaryNpc : MonoBehaviour, Agent {
    private readonly float speed = 1.0f;
    private readonly GoapPlanner planner = new GoapPlanner(NpcTypes.MERCENARY);
    private readonly Blackboard blackboard = new Blackboard();
    private PlanExecutor planExecutor;

    public MercenaryNpc() {
        this.blackboard.SetWorldStateVariable(WorldStateVariables.HAS_MONEY, 100);
        this.planExecutor = new PlanExecutor(this);
    }

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
        List<GoapAction> plan = planner.Plan(this.blackboard.worldState, GoalSelector.SelectGoal(NpcTypes.MERCENARY)).Where(action => action != null).ToList();

        Debug.Log("Planning finished. Path length: " + plan.Count + ", Path is: \n");
        foreach (GoapAction action in plan) {
            Debug.Log(action.ToString());
        }
        Debug.Log("\n");

        this.planExecutor.AddNewPlan(new Stack<GoapAction>(plan));
    }
}