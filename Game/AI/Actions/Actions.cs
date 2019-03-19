using System.Collections.Generic;
using System.Linq;

public static class ActionPool {
    private static readonly List<GoapAction> farmerActions;
    private static readonly List<GoapAction> mercenaryActions;

    static ActionPool() {
        GoapAction farmAction = new GoapAction(ActionType.FARM_PRODUCE.ToString(), new Dictionary<string, object>(), new Dictionary<string, object>() {
            { WorldStateVariable.HAS_PRODUCE.ToString(), 10 }
        }, 5, (agent) => {
            return true;
        }, (agent) => {
            if (agent is FarmerNpc) {
                FarmerNpc farmer = agent as FarmerNpc;
                return farmer.IsFarmCommandComplete();
            } else {
                return false;
            }
        }, (agent) => {
            if (agent is FarmerNpc) {
                FarmerNpc farmer = agent as FarmerNpc;
                farmer.OnFarmCommandIssued();
            }
        });

        GoapAction sellProduceAction = new GoapAction(ActionType.SELL_PRODUCE.ToString(), new Dictionary<string, object>() {
            { WorldStateVariable.HAS_PRODUCE.ToString(), 10 }
        }, new Dictionary<string, object>() {
            { WorldStateVariable.HAS_PRODUCE.ToString(), -10 },
            { WorldStateVariable.HAS_MONEY.ToString(), 100 }
        }, 2, (agent) => {
            return true;
        }, (agent) => {
            if (agent is FarmerNpc) {
                FarmerNpc farmer = agent as FarmerNpc;
                return farmer.IsSellProduceCommandCompleted();
            } else {
                return false;
            }
        }, (agent) => {
            if (agent is FarmerNpc) {
                FarmerNpc farmer = agent as FarmerNpc;
                farmer.OnSellProduceCommandIssued();
            }
        });

        GoapAction buyHouseAction = new GoapAction(ActionType.BUY_HOUSE.ToString(), new Dictionary<string, object>() {
            { WorldStateVariable.HAS_MONEY.ToString(), 100 }
        }, new Dictionary<string, object>() {
            { WorldStateVariable.HAS_MONEY.ToString(), -100 },
            { WorldStateVariable.HAS_HOUSE_BLUEPRINT.ToString(), true }
        }, 1, (agent) => {
            return true;
        }, (agent) => {
            if (agent is FarmerNpc) {
                FarmerNpc farmer = agent as FarmerNpc;
                return farmer.IsBuyHouseCommandCompleted();
            } else {
                return false;
            }
        }, (agent) => {
            if (agent is FarmerNpc) {
                FarmerNpc farmer = agent as FarmerNpc;
                farmer.OnBuyHouseCommandIssued();
            }
        });

        GoapAction buildHouseAction = new GoapAction(ActionType.BUILD_HOUSE.ToString(), new Dictionary<string, object>() {
            { WorldStateVariable.HAS_HOUSE_BLUEPRINT.ToString(), true }
        }, new Dictionary<string, object>() {
            { WorldStateVariable.HAS_HOUSE_BLUEPRINT.ToString(), false },
            { WorldStateVariable.HAS_HOUSE.ToString(), true }
        }, 10, (agent) => {
            return true;
        }, (agent) => {
            if (agent is FarmerNpc) {
                FarmerNpc farmer = agent as FarmerNpc;
                return farmer.IsBuildHouseCommandCompleted();
            } else {
                return false;
            }
        }, (agent) => {
            if (agent is FarmerNpc) {
                FarmerNpc farmer = agent as FarmerNpc;
                farmer.OnBuildHouseCommandIssued();
            }
        });

        farmerActions = new List<GoapAction>(new GoapAction[] {
            farmAction,
            sellProduceAction,
            buyHouseAction,
            buildHouseAction }
        ).OrderBy(action => action.cost).ToList();

        GoapAction buyWeaponAction = new GoapAction(ActionType.BUY_WEAPON.ToString(), new Dictionary<string, object>() {
            { WorldStateVariable.HAS_MONEY.ToString(), 100 }
        }, new Dictionary<string, object>() {
            { WorldStateVariable.HAS_MONEY.ToString(), -100 },
            { WorldStateVariable.HAS_WEAPON.ToString(), true }
        }, 5, (agent) => {
            return true;
        }, (agent) => {
            if (agent is MercenaryNpc) {
                MercenaryNpc mercenary = agent as MercenaryNpc;
                return mercenary.IsBuyWeaponCommandCompleted();
            } else {
                return false;
            }
        }, (agent) => {
            if (agent is MercenaryNpc) {
                MercenaryNpc mercenary = agent as MercenaryNpc;
                mercenary.OnBuyWeaponCommandIssued();
            }
        });

        GoapAction attackTargetAction = new GoapAction(ActionType.ATTACK_TARGET.ToString(), new Dictionary<string, object>() {
            { WorldStateVariable.HAS_WEAPON.ToString(), true }
        }, new Dictionary<string, object>() {
            { WorldStateVariable.HAS_WEAPON.ToString(), false },
            { WorldStateVariable.TARGET_IS_DEAD.ToString(), true }
        }, 1, (agent) => {
            return true;
        }, (agent) => {
            if (agent is MercenaryNpc) {
                MercenaryNpc mercenary = agent as MercenaryNpc;
                return mercenary.IsAttackTargetCommandCompleted();
            } else {
                return false;
            }
        }, (agent) => {
            if (agent is MercenaryNpc) {
                MercenaryNpc mercenary = agent as MercenaryNpc;
                mercenary.OnAttackTargetCommandIssued();
            }
        });

        mercenaryActions = new List<GoapAction>(new GoapAction[] {
            buyWeaponAction,
            attackTargetAction
        }).OrderBy(action => action.cost).ToList();
    }

    public static List<GoapAction> ActionsFor(string npcType) {
        if (npcType == NpcType.FARMER.ToString()) {
            return farmerActions;
        } else if (npcType == NpcType.MERCENARY.ToString()) {
            return mercenaryActions;
        } else {
            return new List<GoapAction>();
        }
    }
}