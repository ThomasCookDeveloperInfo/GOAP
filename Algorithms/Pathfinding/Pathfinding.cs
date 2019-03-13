using System;
using System.Collections.Generic;
using System.Linq;

public class CartesianNode : AStarNode<string> {
    public readonly int x;
    public readonly int y;

    private List<AStarNode<string>> neighbours;

    public CartesianNode(string name, int x, int y) : base(name) {
        this.neighbours = new List<AStarNode<string>>();
        this.x = x;
        this.y = y;
    }

    public void AddNeighbour(CartesianNode other) {
        this.neighbours.Add(other);
    }

    public int DistanceTo(CartesianNode other) {
        int dx = Math.Abs(this.x - other.x);
        int dy = Math.Abs(this.y - other.y);
        return (int)Math.Round(Math.Sqrt(Math.Pow(dx, 2.0) + Math.Pow(dy, 2.0)));
    }

    override public List<AStarNode<string>> GetNeighbours() {
        return this.neighbours;
    }

    override public string ToString() {
        return "([" + x + ", " + y + "], " + fScore + ")";
    }
}

public class Pathfinder {
    private readonly AStar<string> astar = new AStar<string>();

    public List<string> FindPath(CartesianNode from, CartesianNode to) {
        return astar.GetPath(from, to, CartesianHeuristic, CartesianHeuristic).Select(node => node.data).ToList();
    }

    private static int CartesianHeuristic(AStarNode<string> a, AStarNode<string> b) {
        CartesianNode aCartesian = (CartesianNode)a;
        CartesianNode bCartesian = (CartesianNode)b;
        return aCartesian.DistanceTo(bCartesian);
    }
}