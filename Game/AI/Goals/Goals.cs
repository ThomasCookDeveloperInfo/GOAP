using System;
using System.Collections.Generic;

public class Goal {
    public readonly Func<int> priority;
    public readonly WorldState goalState;

    public Goal() {
        this.priority = () => Int32.MaxValue;
        goalState = new WorldState(new Dictionary<string, object>());
    }

    public Goal(Func<int> priority, WorldState goalState) {
        this.priority = priority;
        this.goalState = goalState;
    }
}

public static class GoalPool {
    private static readonly List<Goal> farmerGoals;
    private static readonly List<Goal> mercenaryGoals;

    static GoalPool() {
        Dictionary<string, object> buildHouseGoal = new Dictionary<string, object>() {
            { WorldStateVariable.HAS_HOUSE.ToString(), true }
        };

        farmerGoals = new List<Goal>(new Goal[] {
            new Goal(FarmerNpc.PriorityForGoal(GoalType.HAVE_HOUSE), new WorldState(buildHouseGoal))
        });

        Dictionary<string, object> killTargetGoal = new Dictionary<string, object>() {
            { WorldStateVariable.TARGET_IS_DEAD.ToString(), true }
        };

        mercenaryGoals = new List<Goal>(new Goal[] {
            new Goal(MercenaryNpc.PriorityForGoal(GoalType.HAVE_DEAD_TARGET), new WorldState(killTargetGoal))
        });
    }

    public static List<Goal> GoalsFor(NpcType npcType) {
        if (npcType == NpcType.FARMER) {
            return farmerGoals;
        } else if (npcType == NpcType.MERCENARY) {
            return mercenaryGoals;
        } else {
            return new List<Goal>();
        }
    }
}