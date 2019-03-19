using System.Collections.Generic;

public class Blackboard {
    public readonly WorldState worldState = new WorldState(new Dictionary<string, object>());

    public void SetWorldStateVariable(string worldStateVariable, object value) {
        this.worldState.stateVariables[worldStateVariable] = value;
    }
}