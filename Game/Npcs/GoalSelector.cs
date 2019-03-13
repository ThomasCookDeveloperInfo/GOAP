
public static class GoalSelector {
    public static WorldState SelectGoal(NpcTypes npcType) {
        return GoalPool.GoalsFor(npcType)[0];
    }
}