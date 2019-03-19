using System.Collections.Generic;
using UnityEngine;

public static class NpcManager {
    private static HashSet<FarmerNpc> farmers = new HashSet<FarmerNpc>();
    private static HashSet<MercenaryNpc> mercenaries = new HashSet<MercenaryNpc>();

    public static void AddNpc(Npc npc) {
        if (npc is FarmerNpc) {
            farmers.Add(npc as FarmerNpc);
        } else if (npc is MercenaryNpc) {
            mercenaries.Add(npc as MercenaryNpc);
        }
    }

    public static void RemoveAgent(Npc npc) {
        if (npc is FarmerNpc) {
            farmers.Remove(npc as FarmerNpc);
        } else if (npc is MercenaryNpc) {
            mercenaries.Remove(npc as MercenaryNpc);
        }
    }

    public static Npc FindNearest(NpcType npcType, Transform transform) {
        if (npcType == NpcType.FARMER) {
            FarmerNpc closestFarmer = null;
            foreach (FarmerNpc farmer in farmers) {
                if (closestFarmer == null) {
                    closestFarmer = farmer;
                } else {
                    Vector3 vectorToClosestFarmer = transform.position - closestFarmer.transform.position;
                    Vector3 vectorToFarmer = transform.position - farmer.transform.position;
                    if (vectorToFarmer.magnitude < vectorToClosestFarmer.magnitude) {
                        closestFarmer = farmer;
                    }
                }
            }
            return closestFarmer;
        }
        return null;
    }

    public static void Update() {
        foreach (FarmerNpc farmer in farmers) {
            if (!farmer.HasPlan()) {
                farmer.Plan();
            }
        }

        foreach (MercenaryNpc mercenary in mercenaries) {
            if (!mercenary.HasPlan()) {
                mercenary.Plan();
            }
        }
    }
}