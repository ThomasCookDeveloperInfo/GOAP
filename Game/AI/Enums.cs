public enum WorldStateVariable {
    HAS_PRODUCE,
    HAS_MONEY,
    HAS_HOUSE_BLUEPRINT,
    HAS_HOUSE,
    HAS_WEAPON,
    TARGET_IS_DEAD
}

public enum NpcType {
    FARMER,
    MERCENARY
}

public enum ActionType {
    FARM_PRODUCE,
    SELL_PRODUCE,
    BUY_HOUSE,
    BUILD_HOUSE,
    BUY_WEAPON,
    ATTACK_TARGET
}

public enum GoalType {
    HAVE_HOUSE,
    HAVE_DEAD_TARGET
}