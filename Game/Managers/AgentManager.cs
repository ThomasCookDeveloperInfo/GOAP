using System.Collections.Generic;

public static class AgentManager {
    private static HashSet<FarmerNpc> farmers = new HashSet<FarmerNpc>();
    private static HashSet<MercenaryNpc> mercenaries = new HashSet<MercenaryNpc>();

    public static void AddAgent(Agent agent) {
        if (agent is FarmerNpc) {
            farmers.Add(agent as FarmerNpc);
        } else if (agent is MercenaryNpc) {
            mercenaries.Add(agent as MercenaryNpc);
        }
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