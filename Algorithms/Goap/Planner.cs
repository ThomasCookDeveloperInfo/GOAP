using System.Linq;
using System.Collections.Generic;
using System;

public class GoapPlanner {
    private readonly string npcType;
    private readonly AStar<GoapAction> astar = new AStar<GoapAction>();
    
    public GoapPlanner(string npcType) {
        this.npcType = npcType;
    }

    public List<GoapAction> Plan(Func<string, List<GoapAction>> actionPool, WorldState fromState, WorldState toState) {
        GoapNode start = new GoapNode(actionPool, null, fromState, this.npcType);
        GoapNode goal = new GoapNode(actionPool, null, toState, this.npcType);
        return astar.GetPath(start, goal, GoapHeuristic, GoapCost).Select(goapNode => goapNode.data as GoapAction).ToList();
    }

    private static int GoapHeuristic(AStarNode<GoapAction> a, AStarNode<GoapAction> b) {
        GoapNode aGoap = (GoapNode)a;
        GoapNode bGoap = (GoapNode)b;
        return aGoap.worldState.Diff(bGoap.worldState);
    }

    private static int GoapCost(AStarNode<GoapAction> a, AStarNode<GoapAction> b) {
        return b.data.cost;
    }
}