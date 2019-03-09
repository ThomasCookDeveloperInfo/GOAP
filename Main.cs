using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Main : MonoBehaviour {

    // Use this for initialization
    void Start() {
        CartesianNode a = new CartesianNode("a", 0, 0);
        CartesianNode b = new CartesianNode("b", 10, 0);
        CartesianNode c = new CartesianNode("c", 20, 0);
        CartesianNode d = new CartesianNode("d", -5, 0);
        CartesianNode e = new CartesianNode("e", 5, 10);
        CartesianNode f = new CartesianNode("f", 15, 10);
        CartesianNode g = new CartesianNode("g", 15, 20);
        CartesianNode h = new CartesianNode("h", 15, 30);

        a.AddNeighbour(b);
        a.AddNeighbour(d);
        a.AddNeighbour(e);

        b.AddNeighbour(a);
        b.AddNeighbour(c);
        b.AddNeighbour(f);

        c.AddNeighbour(b);
        c.AddNeighbour(f);

        d.AddNeighbour(a);
        d.AddNeighbour(h);

        e.AddNeighbour(a);
        e.AddNeighbour(f);

        f.AddNeighbour(b);
        f.AddNeighbour(c);
        f.AddNeighbour(e);
        f.AddNeighbour(g);

        g.AddNeighbour(f);
        g.AddNeighbour(h);

        h.AddNeighbour(d);
        h.AddNeighbour(g);

        AStar<string> aStar = new AStar<string>();
        List<AStarNode<string>> path = aStar.getPath(a, h, CartesianHeuristic, CartesianHeuristic);
        Debug.Log("Path search finished. Path length: " + path.Count + ", Path is: \n");
        foreach (AStarNode<string> node in path) {
            Debug.Log("Node: " + node.Format());
        }

        Debug.Log("\n");

        AStar<GoapAction> goap = new AStar<GoapAction>();

        GoapNode start = new GoapNode(null, new WorldState(new Dictionary<WorldStateVariables, object>()));

        Dictionary<WorldStateVariables, object> goalState = new Dictionary<WorldStateVariables, object>();
        goalState[WorldStateVariables.HAS_GUN] = true;
        goalState[WorldStateVariables.HAS_AMMO] = 400;
        goalState[WorldStateVariables.HAS_MONEY] = 200;

        GoapNode goal = new GoapNode(null, new WorldState(goalState));

        Debug.Log("Finding plan from world state: " + start.worldState.ToString() + "\nto world state: " + goal.worldState.ToString() + "\n");
        List<AStarNode<GoapAction>> plan = goap.getPath(start, goal, GoapHeuristic, GoapCost);
        Debug.Log("Plan search finished. Plan length: " + plan.Count + ", Plan is: ");
        foreach (AStarNode<GoapAction> node in plan) {
            Debug.Log("Action: " + node.Format());
        }
    }

    private int CartesianHeuristic(AStarNode<string> a, AStarNode<string> b) {
        CartesianNode aCartesian = (CartesianNode)a;
        CartesianNode bCartesian = (CartesianNode)b;
        return aCartesian.DistanceTo(bCartesian);
    }

    private int GoapHeuristic(AStarNode<GoapAction> a, AStarNode<GoapAction> b) {
        GoapNode aGoap = (GoapNode)a;
        GoapNode bGoap = (GoapNode)b;
        return aGoap.worldState.Diff(bGoap.worldState);
    }

    private int GoapCost(AStarNode<GoapAction> a, AStarNode<GoapAction> b) {
        return b.data.cost;
    }
}


public class GoapNode : AStarNode<GoapAction> {
    public readonly WorldState worldState;

    public GoapNode(GoapAction action, WorldState worldState) : base(action) {
        this.worldState = worldState;
    }

    override public List<AStarNode<GoapAction>> GetNeighbours() {
        List<AStarNode<GoapAction>> neighbours = new List<AStarNode<GoapAction>>();
        foreach (GoapAction action in ActionPool.actions) {
            if (action.IsValid(this.worldState)) {
                GoapNode neighbour = new GoapNode(action, this.worldState.applyAction(action));
                if (this.worldState.Diff(neighbour.worldState) > 0) {
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    override public bool Equals(object obj) {
        if (obj is GoapNode) {
            return this.worldState.Diff(((GoapNode)obj).worldState) == 0;
        }
        return false;
    }
}

public static class ActionPool {
    public static readonly List<GoapAction> actions;

    static ActionPool() {
        actions = new List<GoapAction>();

        Dictionary<WorldStateVariables, object> stealMoneyPreconditions = new Dictionary<WorldStateVariables, object>();
        Dictionary<WorldStateVariables, object> stealMoneyPostconditions = new Dictionary<WorldStateVariables, object>();
        stealMoneyPostconditions[WorldStateVariables.HAS_MONEY] = 400;
        GoapAction stealMoneyAction = new GoapAction("StealMoney", stealMoneyPreconditions, stealMoneyPostconditions, 10);

        Dictionary<WorldStateVariables, object> buyGunPreconditions = new Dictionary<WorldStateVariables, object>();
        buyGunPreconditions[WorldStateVariables.HAS_MONEY] = 200;
        Dictionary<WorldStateVariables, object> buyGunPostconditions = new Dictionary<WorldStateVariables, object>();
        buyGunPostconditions[WorldStateVariables.HAS_GUN] = true;
        buyGunPostconditions[WorldStateVariables.HAS_MONEY] = -200;
        GoapAction buyGunAction = new GoapAction("BuyGun", buyGunPreconditions, buyGunPostconditions, 3);


        Dictionary<WorldStateVariables, object> buyAmmoPreconditions = new Dictionary<WorldStateVariables, object>();
        buyAmmoPreconditions[WorldStateVariables.HAS_MONEY] = 200;
        Dictionary<WorldStateVariables, object> buyAmmoPostconditions = new Dictionary<WorldStateVariables, object>();
        buyAmmoPostconditions[WorldStateVariables.HAS_AMMO] = 100;
        buyAmmoPostconditions[WorldStateVariables.HAS_MONEY] = -200;
        GoapAction buyAmmoAction = new GoapAction("BuyAmmo", buyAmmoPreconditions, buyAmmoPostconditions, 3);

        actions.Add(buyAmmoAction);
        actions.Add(buyGunAction);
        actions.Add(stealMoneyAction);
    }
}

public class GoapAction {
    public readonly string name;
    public readonly Dictionary<WorldStateVariables, object> preconditions;
    public readonly Dictionary<WorldStateVariables, object> postconditions;
    public readonly int cost;

    public GoapAction(string name, Dictionary<WorldStateVariables, object> preconditions, Dictionary<WorldStateVariables, object> postconditions, int cost) {
        this.name = name;
        this.preconditions = preconditions;
        this.postconditions = postconditions;
        this.cost = cost;
    }

    public bool IsValid(WorldState forWorldState) {
        foreach (KeyValuePair<WorldStateVariables, object> kvPair in preconditions) {
            if (forWorldState.stateVariables.ContainsKey(kvPair.Key)) {
                if (kvPair.Value is int && forWorldState.stateVariables[kvPair.Key] is int) {
                    if ((int)forWorldState.stateVariables[kvPair.Key] >= (int)kvPair.Value) {
                        continue;
                    } else {
                        return false;
                    }
                } else {
                    if (forWorldState.stateVariables[kvPair.Key].Equals(kvPair.Value)) {
                        continue;
                    } else {
                        return false;
                    }
                }
            } else {
                return false;
            }
        }
        return true;
    }

    public override string ToString() {
        return name;
    }
}

public class WorldState {
    public readonly Dictionary<WorldStateVariables, object> stateVariables;

    public WorldState(Dictionary<WorldStateVariables, object> stateVariables) {
        this.stateVariables = stateVariables;
    }

    public WorldState applyAction(GoapAction action) {
        Dictionary<WorldStateVariables, object> newStateVariables = new Dictionary<WorldStateVariables, object>();

        foreach (KeyValuePair<WorldStateVariables, object> kvPair in this.stateVariables) {
            newStateVariables[kvPair.Key] = kvPair.Value;
        }
        foreach (KeyValuePair<WorldStateVariables, object> kvPair in action.postconditions) {
            if (kvPair.Value is int && newStateVariables.ContainsKey(kvPair.Key) && newStateVariables[kvPair.Key] is int) {
                int newValue = (int)newStateVariables[kvPair.Key] + (int)kvPair.Value;
                newStateVariables[kvPair.Key] = newValue;
            } else {
                newStateVariables[kvPair.Key] = kvPair.Value;
            }
        }
        return new WorldState(newStateVariables);
    }

    public int Diff(WorldState other) {
        Dictionary<WorldStateVariables, object> otherWorldStateVariables = other.stateVariables;
        Dictionary<WorldStateVariables, object> thisWorldStateVariables = this.stateVariables;

        int differences = 0;
        foreach (KeyValuePair<WorldStateVariables, object> kvPair in otherWorldStateVariables) {
            WorldStateVariables key = kvPair.Key;
            object value = kvPair.Value;
            if (thisWorldStateVariables.ContainsKey(key)) {
                if (thisWorldStateVariables[key] is int) {
                    if ((int)thisWorldStateVariables[key] < (int)value) {
                        differences++;
                    }
                } else {
                    if (!thisWorldStateVariables[key].Equals(value)) {
                        differences++;
                    }
                }
            } else {
                differences++;
            }
        }

        return differences;
    }
}

public enum WorldStateVariables {
    HAS_AMMO,
    HAS_MONEY,
    HAS_GUN,
}

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
}

public class AStar<T> {
    public List<AStarNode<T>> getPath(AStarNode<T> from, AStarNode<T> to, Func<AStarNode<T>, AStarNode<T>, int> heuristic, Func<AStarNode<T>, AStarNode<T>, int> cost) {
        PriorityQueue<AStarNode<T>> openQueue = new PriorityQueue<AStarNode<T>>();
        HashSet<AStarNode<T>> closedSet = new HashSet<AStarNode<T>>();

        Dictionary<AStarNode<T>, AStarNode<T>> cameFrom = new Dictionary<AStarNode<T>, AStarNode<T>>();
        Dictionary<AStarNode<T>, int> gScores = new Dictionary<AStarNode<T>, int>();
        Dictionary<AStarNode<T>, int> fScores = new Dictionary<AStarNode<T>, int>();

        openQueue.Enqueue(from);
        gScores[from] = 0;
        fScores[from] = heuristic(from, to);

        while (!openQueue.IsEmpty()) {
            AStarNode<T> current = openQueue.Dequeue();

            if (current.Equals(to)) {
                return reconstructPath(cameFrom, current);
            }

            closedSet.Add(current);

            foreach (AStarNode<T> neighbour in current.GetNeighbours()) {
                if (closedSet.Contains(neighbour)) {
                    continue;
                }

                int tentativeGScore = gScores[current] + cost(current, neighbour);

                if (!openQueue.Contains(neighbour)) {
                    openQueue.Enqueue(neighbour);
                } else if (gScores.ContainsKey(neighbour) && tentativeGScore >= gScores[neighbour]) {
                    continue;
                }

                cameFrom[neighbour] = current;
                gScores[neighbour] = tentativeGScore;
                fScores[neighbour] = gScores[neighbour] + heuristic(neighbour, to);
                neighbour.fScore = fScores[neighbour];
            }
        }

        return new List<AStarNode<T>>();
    }

    private List<AStarNode<T>> reconstructPath(Dictionary<AStarNode<T>, AStarNode<T>> cameFrom, AStarNode<T> current) {
        List<AStarNode<T>> path = new List<AStarNode<T>>();
        path.Add(current);
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}

public abstract class AStarNode<T> : IComparable<AStarNode<T>> {
    public int fScore;
    public readonly T data;

    public AStarNode(T data) {
        this.data = data;
    }

    public abstract List<AStarNode<T>> GetNeighbours();

    public int CompareTo(AStarNode<T> obj) {
        if (obj.fScore > this.fScore) {
            return 1;
        } else if (obj.fScore < this.fScore) {
            return -1;
        } else {
            return 0;
        }
    }

    public string Format() {
        if (data == null) {
            return "null";
        } else {
            return data.ToString();
        }
    }
}

public class PriorityQueue<T> where T : IComparable<T> {
    private List<T> data;

    public PriorityQueue() {
        this.data = new List<T>();
    }

    public void Enqueue(T item) {
        this.data.Add(item);
        int childIndex = data.Count - 1;
        while (childIndex > 0) {
            int parentIndex = (childIndex - 1) / 2;
            if (this.data[childIndex].CompareTo(this.data[parentIndex]) >= 0) {
                break;
            }
            T tmp = this.data[childIndex];
            this.data[childIndex] = this.data[parentIndex];
            this.data[parentIndex] = tmp;
            childIndex = parentIndex;
        }
    }

    public T Dequeue() {
        int lastIndex = this.data.Count - 1;
        T frontItem = this.data[0];
        this.data[0] = this.data[lastIndex];
        this.data.RemoveAt(lastIndex);

        lastIndex--;
        int parentIndex = 0;
        while (true) {
            int leftChildIndex = parentIndex * 2 + 1;
            if (leftChildIndex > parentIndex) {
                break;
            }
            int rightChildIndex = leftChildIndex + 1;
            if (rightChildIndex <= lastIndex && this.data[rightChildIndex].CompareTo(this.data[leftChildIndex]) < 0) {
                leftChildIndex = rightChildIndex;
            }
            if (this.data[parentIndex].CompareTo(this.data[leftChildIndex]) <= 0) {
                break;
            }
            T tmp = this.data[parentIndex];
            this.data[parentIndex] = this.data[leftChildIndex];
            this.data[leftChildIndex] = tmp;
            parentIndex = leftChildIndex;
        }
        return frontItem;
    }

    public bool IsEmpty() {
        return this.data.Count == 0;
    }

    public bool Contains(T item) {
        return this.data.Contains(item);
    }
}