using System.Collections.Generic;

public static class GoalPool {
    private static readonly List<WorldState> farmerGoals;
    private static readonly List<WorldState> mercenaryGoals;

    static GoalPool() {
        Dictionary<WorldStateVariables, object> buildHouseGoal = new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_HOUSE, true }
        };

        farmerGoals = new List<WorldState>(new WorldState[] {
            new WorldState(buildHouseGoal)
        });

        Dictionary<WorldStateVariables, object> killTargetGoal = new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.TARGET_IS_DEAD, true }
        };

        mercenaryGoals = new List<WorldState>(new WorldState[] {
            new WorldState(killTargetGoal)
        });
    }

    public static List<WorldState> GoalsFor(NpcTypes npcType) {
        if (npcType == NpcTypes.FARMER) {
            return farmerGoals;
        } else if (npcType == NpcTypes.MERCENARY) {
            return mercenaryGoals;
        } else {
            return new List<WorldState>();
        }
    }
}