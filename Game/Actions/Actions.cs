using System.Collections.Generic;
using System.Linq;

public static class ActionPool {
    private static readonly List<GoapAction> farmerActions;
    private static readonly List<GoapAction> mercenaryActions;

    static ActionPool() {
        GoapAction farmAction = new GoapAction("FarmProduce", new Dictionary<WorldStateVariables, object>(), new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_PRODUCE, 10 }
        }, 1, (agent) => {
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

        GoapAction sellProduceAction = new GoapAction("SellProduce", new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_PRODUCE, 10 }
        }, new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_MONEY, 100 }
        }, 5, (agent) => {
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

        GoapAction buyHouseAction = new GoapAction("BuyHouse", new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_MONEY, 1000 }
        }, new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_HOUSE, true }
        }, 10, (agent) => {
            return true;
        }, (agent) => {
            return true;
        }, (agent) => {

        });

        farmerActions = new List<GoapAction>(new GoapAction[] {
            farmAction,
            sellProduceAction,
            buyHouseAction }
        ).OrderBy(action => action.cost).ToList();

        GoapAction buyWeaponAction = new GoapAction("BuyWeapon", new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_MONEY, 100 }
        }, new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_WEAPON, true }
        }, 5, (agent) => {
            return true;
        }, (agent) => {
            return true;
        }, (agent) => {

        });

        GoapAction attackTargetAction = new GoapAction("AttackTarget", new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.HAS_WEAPON, true }
        }, new Dictionary<WorldStateVariables, object>() {
            { WorldStateVariables.TARGET_IS_DEAD, true }
        }, 5, (agent) => {
            return true;
        }, (agent) => {
            return true;
        }, (agent) => {

        });

        mercenaryActions = new List<GoapAction>(new GoapAction[] {
            buyWeaponAction,
            attackTargetAction
        }).OrderBy(action => action.cost).ToList();
    }

    public static List<GoapAction> ActionsFor(NpcTypes npcType) {
        if (npcType == NpcTypes.FARMER) {
            return farmerActions;
        } else if (npcType == NpcTypes.MERCENARY) {
            return mercenaryActions;
        } else {
            return new List<GoapAction>();
        }
    }
}