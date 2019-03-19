using System.Text;
using System.Collections.Generic;
using System;

public class GoapNode : AStarNode<GoapAction> {
    public readonly WorldState worldState;
    private readonly string npcType;
    private readonly Func<string, List<GoapAction>> actionPool;

    public GoapNode(Func<string, List<GoapAction>> actionPool, GoapAction action, WorldState worldState, string npcType) : base(action) {
        this.worldState = worldState;
        this.npcType = npcType;
        this.actionPool = actionPool;
    }

    override public List<AStarNode<GoapAction>> GetNeighbours() {
        List<AStarNode<GoapAction>> neighbours = new List<AStarNode<GoapAction>>();
        foreach (GoapAction action in this.actionPool(npcType)) {
            if (action.IsValid(this.worldState)) {
                WorldState neighbourWorldState = this.worldState.ApplyAction(action);
                if (this.worldState.Diff(neighbourWorldState) > 0) {
                    GoapNode neighbour = new GoapNode(actionPool, action, neighbourWorldState, npcType);
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
            sb.Append("(Took action: " + (this.data as GoapAction).actionType.ToString() + " to reach state: " + this.worldState.ToString() + ", fScore: " + this.fScore + ")");
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
    public readonly string actionType;
    public readonly Dictionary<string, object> preconditions;
    public readonly Dictionary<string, object> postconditions;
    public readonly int cost;

    public readonly Func<GoapAgent, bool> IsProceduralyValid;
    public readonly Func<GoapAgent, bool> IsComplete;
    public readonly Action<GoapAgent> Continue;

    public GoapAction(string actionType, Dictionary<string, object> preconditions,
                      Dictionary<string, object> postconditions, int cost,
                      Func<GoapAgent, bool> IsProceduralyValid, Func<GoapAgent, bool> IsComplete, Action<GoapAgent> Continue) {
        this.actionType = actionType;
        this.preconditions = preconditions;
        this.postconditions = postconditions;
        this.cost = cost;
        this.IsProceduralyValid = IsProceduralyValid;
        this.IsComplete = IsComplete;
        this.Continue = Continue;
    }

    public bool IsValid(WorldState forWorldState) {
        foreach (KeyValuePair<string, object> kvPair in preconditions) {
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
        return actionType.ToString();
    }
}

public interface GoapAgent {
    Blackboard blackboard { get; }
    bool HasPlan();
    void Plan();
    void OnCommandCompleted(Dictionary<string, object> newState, string actionType);
}

public class WorldState {
    public readonly Dictionary<string, object> stateVariables;

    public WorldState(Dictionary<string, object> stateVariables) {
        this.stateVariables = stateVariables;
    }

    public WorldState ApplyAction(GoapAction action) {
        Dictionary<string, object> newStateVariables = new Dictionary<string, object>();

        foreach (KeyValuePair<string, object> kvPair in this.stateVariables) {
            newStateVariables[kvPair.Key] = kvPair.Value;
        }
        foreach (KeyValuePair<string, object> kvPair in action.postconditions) {
            string key = kvPair.Key;
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
        Dictionary<string, object> otherWorldStateVariables = other.stateVariables;
        Dictionary<string, object> thisWorldStateVariables = this.stateVariables;

        int differences = 0;
        foreach (KeyValuePair<string, object> kvPair in otherWorldStateVariables) {
            string key = kvPair.Key;
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
        Dictionary<string, object> thisWorldStateVariables = this.stateVariables;
        foreach (KeyValuePair<string, object> kvPair in thisWorldStateVariables) {
            string key = kvPair.Key;
            object value = kvPair.Value;
            sb.Append(key + ": " + value + ", ");
        }
        sb.Append("]");
        return sb.ToString();
    }
}