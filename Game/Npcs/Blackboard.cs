using System.Collections.Generic;

public class Blackboard {
    public readonly WorldState worldState = new WorldState(new Dictionary<WorldStateVariables, object>());

    public void SetWorldStateVariable(WorldStateVariables worldStateVariable, object value) {
        this.worldState.stateVariables[worldStateVariable] = value;
    }
}