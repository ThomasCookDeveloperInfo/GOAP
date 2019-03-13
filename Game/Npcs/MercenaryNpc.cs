using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MercenaryNpc : MonoBehaviour, Agent {
    private readonly float speed = 1.0f;
    private readonly GoapPlanner planner = new GoapPlanner(NpcTypes.MERCENARY);
    private readonly Blackboard blackboard = new Blackboard();
    private List<GoapAction> currentPlan = null;
    private Action currentAction = null;

    void Start() {
        AgentManager.AddAgent(this);
    }

    void Update() {
        if (currentAction != null) {
            currentAction();
        }
    }

    public MercenaryNpc() {
        this.blackboard.SetWorldStateVariable(WorldStateVariables.HAS_MONEY, 100);
    }

    public bool HasPlan() {
        return currentPlan != null;
    }

    public void Plan() {
        this.currentPlan = planner.Plan(this.blackboard.worldState, GoalSelector.SelectGoal(NpcTypes.MERCENARY)).Where(action => action != null).ToList();

        Debug.Log("Planning finished. Path length: " + currentPlan.Count + ", Path is: \n");
        foreach (GoapAction action in currentPlan) {
            Debug.Log(action.ToString());
        }
        Debug.Log("\n");
    }
}