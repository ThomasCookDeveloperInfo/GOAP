using System;
using System.Collections.Generic;

public class Entry {
  static void Main(string[] args) {
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

    List<AStarNode<string>> graph = new List<AStarNode<string>>();
    graph.Add(a);
    graph.Add(b);
    graph.Add(c);
    graph.Add(d);
    graph.Add(e);
    graph.Add(f);
    graph.Add(g);
    graph.Add(h);

    AStar<string> aStar = new AStar<string>();

    List<AStarNode<string>> path = aStar.getPath(graph, a, h, CartesianHeuristic);

    System.Console.WriteLine("Path search finished. Path length: " + path.Count + ", Path is: ");

    foreach (AStarNode<string> node in path) {
      System.Console.WriteLine("Node: " + node.Format());
    }


  }

  private static int CartesianHeuristic(AStarNode<string> a, AStarNode<string> b) {
    CartesianNode aCartesian = (CartesianNode) a;
    CartesianNode bCartesian = (CartesianNode) b;
    return aCartesian.DistanceTo(bCartesian);
  }
}

public class GoapNode : AStarNode<GoapAction> {

  public GoapNode(GoapAction action) : base(action) {}

  override public List<AStarNode<GoapAction>> GetNeighbours() {
    throw new NotImplementedException("");
  }
}

public class GoapAction {
  public readonly string name;
  public readonly Dictionary<string, object> preconditions;
  public readonly Dictionary<string, object> postconditions;

  public GoapAction(string name, Dictionary<string, object> preconditions, Dictionary<string, object> postconditions) {
    this.name = name;
    this.preconditions = preconditions;
    this.postconditions = postconditions;
  }

  public bool IsValid(WorldState forWorldState) {
    foreach (KeyValuePair<string, object> kvPair in forWorldState.stateVariables) {
      if (preconditions.ContainsKey(kvPair.Key) && preconditions[kvPair.Key] == kvPair.Value) {
        continue;
      } else {
        return false;
      }
    }
    return true;
  }
}

public class WorldState {
  public readonly Dictionary<string, object> stateVariables;

  public WorldState(Dictionary<string, object> stateVariables) {
    this.stateVariables = stateVariables;
  }

  public WorldState applyAction(GoapAction action) {
    Dictionary<string, object> newStateVariables = this.stateVariables;
    foreach (KeyValuePair<string, object> kvPair in action.postconditions) {
      newStateVariables[kvPair.Key] = kvPair.Value;
    }
    return new WorldState(newStateVariables);
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
    return (int) Math.Round(Math.Sqrt(Math.Pow(dx, 2.0) + Math.Pow(dy, 2.0)));
  }

  override public List<AStarNode<string>> GetNeighbours() {
    return this.neighbours;
  }
}

public class AStar<T> {
  public List<AStarNode<T>> getPath(List<AStarNode<T>> graph, AStarNode<T> from, AStarNode<T> to, Func<AStarNode<T>, AStarNode<T>, int> heuristic) {
    PriorityQueue<AStarNode<T>> openQueue = new PriorityQueue<AStarNode<T>>();
    HashSet<AStarNode<T>> closedSet = new HashSet<AStarNode<T>>();

    Dictionary<AStarNode<T>, AStarNode<T>> cameFrom = new Dictionary<AStarNode<T>, AStarNode<T>>();

    Dictionary<AStarNode<T>, int> gScores = new Dictionary<AStarNode<T>, int>();
    foreach (AStarNode<T> node in graph) {
      gScores[node] = int.MaxValue;
    }

    Dictionary<AStarNode<T>, int> fScores = new Dictionary<AStarNode<T>, int>();
    foreach (AStarNode<T> node in graph) {
      fScores[node] = int.MaxValue;
    }

    openQueue.Enqueue(from);
    gScores[from] = 0;
    fScores[from] = heuristic(from, to);

    while (!openQueue.IsEmpty()) {
      AStarNode<T> current = openQueue.Dequeue();
      if (current == to) {
        return reconstructPath(cameFrom, current);
      }

      closedSet.Add(current);

      foreach (AStarNode<T> neighbour in current.GetNeighbours()) {
        if (closedSet.Contains(neighbour)) {
          continue;
        }

        int tentativeGScore = gScores[current] + heuristic(current, neighbour);

        if (!openQueue.Contains(neighbour)) {
          openQueue.Enqueue(neighbour);
        } else if (tentativeGScore >= gScores[neighbour]) {
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
    return data.ToString();
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
