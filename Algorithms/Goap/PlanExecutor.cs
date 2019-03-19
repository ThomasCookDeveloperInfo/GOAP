using System.Collections.Generic;

class PlanExecutor {
    private readonly GoapAgent agent;
    private Stack<GoapAction> plan = new Stack<GoapAction>();

    public PlanExecutor(GoapAgent agent) {
        this.agent = agent;
    }

    public bool HasPlan() {
        return plan.Count > 0;
    }

    public void AddNewPlan(Stack<GoapAction> plan) {
        this.plan.Clear();
        this.plan = plan;
    }

    public void Execute() {
        if (this.plan.Count > 0 && this.agent != null) {
            if (this.plan.Peek().IsComplete(agent)) {
                agent.OnCommandCompleted(this.agent.blackboard.worldState.ApplyAction(this.plan.Peek()).stateVariables, this.plan.Peek().actionType);
                this.plan.Pop();
            } else {
                this.plan.Peek().Continue(agent);
            }
        }
    }
}