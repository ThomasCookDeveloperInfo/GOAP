using System.Linq;
using System.Text;
using System.Collections.Generic;
using System;

public enum WorldStateVariables {
    HAS_PRODUCE,
    HAS_MONEY,
    HAS_HOUSE,
    HAS_WEAPON,
    TARGET_IS_DEAD
}

public enum NpcTypes {
    FARMER,
    MERCENARY
}

public class GoapPlanner {
    private readonly NpcTypes npcType;
    private readonly AStar<GoapAction> astar = new AStar<GoapAction>();

    public GoapPlanner() {
        npcType = NpcTypes.FARMER;
    }

    public GoapPlanner(NpcTypes npcType) {
        this.npcType = npcType;
    }

    public List<GoapAction> Plan(WorldState fromState, WorldState toState) {
        GoapNode start = new GoapNode(null, fromState, this.npcType);
        GoapNode goal = new GoapNode(null, toState, this.npcType);
        return astar.GetPath(start, goal, GoapHeuristic, GoapCost).Select(goapNode => goapNode.data as GoapAction).ToList();
    }

    private static int GoapHeuristic(AStarNode<GoapAction> a, AStarNode<GoapAction> b) {
        GoapNode aGoap = (GoapNode)a;
        GoapNode bGoap = (GoapNode)b;
        return aGoap.worldState.Diff(bGoap.worldState);
    }

    private static int GoapCost(AStarNode<GoapAction> a, AStarNode<GoapAction> b) {
        return b.data.cost;
    }
}

public class GoapNode : AStarNode<GoapAction> {
    public readonly WorldState worldState;
    private readonly NpcTypes npcType;

    public GoapNode(GoapAction action, WorldState worldState, NpcTypes npcType) : base(action) {
        this.worldState = worldState;
        this.npcType = npcType;
    }

    override public List<AStarNode<GoapAction>> GetNeighbours() {
        List<AStarNode<GoapAction>> neighbours = new List<AStarNode<GoapAction>>();
        foreach (GoapAction action in ActionPool.ActionsFor(this.npcType)) {
            if (action.IsValid(this.worldState)) {
                WorldState neighbourWorldState = this.worldState.ApplyAction(action);
                if (this.worldState.Diff(neighbourWorldState) > 0) {
                    GoapNode neighbour = new GoapNode(action, neighbourWorldState, npcType);
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    override public bool Equals(object obj) {
        if (obj is GoapNode) {
            return this.worldState.Diff(((GoapNode)obj).worldState) == 0;
        }
        return false;
    }

    override public string ToString() {
        if (this.data == null) {
            return "";
        } else {
            StringBuilder sb = new StringBuilder();
            sb.Append("(Took action: " + (this.data as GoapAction).name + " to reach state: " + this.worldState.ToString() + ", fScore: " + this.fScore + ")");
            return sb.ToString();
        }
    }

    public override int GetHashCode() {
        var hashCode = 297563868;
        hashCode = hashCode * -1521134295 + EqualityComparer<WorldState>.Default.GetHashCode(worldState);
        hashCode = hashCode * -1521134295 + npcType.GetHashCode();
        return hashCode;
    }
}

public class GoapAction {
    public readonly string name;
    public readonly Dictionary<WorldStateVariables, object> preconditions;
    public readonly Dictionary<WorldStateVariables, object> postconditions;
    public readonly int cost;

    public readonly Func<Agent, bool> IsProceduralyValid;
    public readonly Func<Agent, bool> IsComplete;
    public readonly Action<Agent> Continue;

    public GoapAction(string name, Dictionary<WorldStateVariables, object> preconditions,
                      Dictionary<WorldStateVariables, object> postconditions, int cost,
                      Func<Agent, bool> IsProceduralyValid, Func<Agent, bool> isComplete, Action<Agent> Continue) {
        this.name = name;
        this.preconditions = preconditions;
        this.postconditions = postconditions;
        this.cost = cost;
        this.IsProceduralyValid = IsProceduralyValid;
        this.IsComplete = isComplete;
        this.Continue = Continue;
    }

    public bool IsValid(WorldState forWorldState) {
        foreach (KeyValuePair<WorldStateVariables, object> kvPair in preconditions) {
            if (forWorldState.stateVariables.ContainsKey(kvPair.Key)) {
                if (kvPair.Value is int && forWorldState.stateVariables[kvPair.Key] is int) {
                    if ((int)forWorldState.stateVariables[kvPair.Key] >= (int)kvPair.Value) {
                        continue;
                    } else {
                        return false;
                    }
                } else {
                    if (forWorldState.stateVariables[kvPair.Key].Equals(kvPair.Value)) {
                        continue;
                    } else {
                        return false;
                    }
                }
            } else {
                return false;
            }
        }
        return true;
    }

    public override string ToString() {
        return name;
    }
}

public interface Agent {
    bool HasPlan();
    void Plan();
}

public class WorldState {
    public readonly Dictionary<WorldStateVariables, object> stateVariables;

    public WorldState(Dictionary<WorldStateVariables, object> stateVariables) {
        this.stateVariables = stateVariables;
    }

    public WorldState ApplyAction(GoapAction action) {
        Dictionary<WorldStateVariables, object> newStateVariables = new Dictionary<WorldStateVariables, object>();

        foreach (KeyValuePair<WorldStateVariables, object> kvPair in this.stateVariables) {
            newStateVariables[kvPair.Key] = kvPair.Value;
        }
        foreach (KeyValuePair<WorldStateVariables, object> kvPair in action.postconditions) {
            WorldStateVariables key = kvPair.Key;
            object newValue = kvPair.Value;
            if (newStateVariables.ContainsKey(key)) {
                if (newStateVariables[key] is int) {
                    newStateVariables[key] = (int)newStateVariables[key] + (int)newValue;
                } else {
                    newStateVariables[key] = newValue;
                }
            } else {
                newStateVariables[key] = kvPair.Value;
            }
        }

        WorldState newWorldState = new WorldState(newStateVariables);

        return newWorldState;
    }

    public int Diff(WorldState other) {
        Dictionary<WorldStateVariables, object> otherWorldStateVariables = other.stateVariables;
        Dictionary<WorldStateVariables, object> thisWorldStateVariables = this.stateVariables;

        int differences = 0;
        foreach (KeyValuePair<WorldStateVariables, object> kvPair in otherWorldStateVariables) {
            WorldStateVariables key = kvPair.Key;
            object value = kvPair.Value;
            if (thisWorldStateVariables.ContainsKey(key)) {
                if (thisWorldStateVariables[key] is int) {
                    if ((int)thisWorldStateVariables[key] < (int)value) {
                        differences++;
                    }
                } else {
                    if (!thisWorldStateVariables[key].Equals(value)) {
                        differences++;
                    }
                }
            } else {
                differences++;
            }
        }

        return differences;
    }

    override public string ToString() {
        StringBuilder sb = new StringBuilder("[");
        Dictionary<WorldStateVariables, object> thisWorldStateVariables = this.stateVariables;
        foreach (KeyValuePair<WorldStateVariables, object> kvPair in thisWorldStateVariables) {
            WorldStateVariables key = kvPair.Key;
            object value = kvPair.Value;
            sb.Append(key + ": " + value + ", ");
        }
        sb.Append("]");
        return sb.ToString();
    }
}

class PlanExecutor {
    private readonly Agent agent;
    private Stack<GoapAction> plan = new Stack<GoapAction>();

    public PlanExecutor(Agent agent) {
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
        if (this.agent == null) {
            return;
        }
        
        if (this.plan.Peek().IsComplete(agent)) {
            this.plan.Pop();
        } else {
            this.plan.Peek().Continue(agent);
        }
    }
}