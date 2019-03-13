using System;
using System.Collections.Generic;

public class AStar<T> {
    public List<AStarNode<T>> GetPath(AStarNode<T> from, AStarNode<T> to, Func<AStarNode<T>, AStarNode<T>, int> heuristic, Func<AStarNode<T>, AStarNode<T>, int> cost) {
        PriorityQueue<AStarNode<T>> openQueue = new PriorityQueue<AStarNode<T>>();
        HashSet<AStarNode<T>> closedSet = new HashSet<AStarNode<T>>();

        Dictionary<AStarNode<T>, AStarNode<T>> cameFrom = new Dictionary<AStarNode<T>, AStarNode<T>>();
        Dictionary<AStarNode<T>, int> gScores = new Dictionary<AStarNode<T>, int>();
        Dictionary<AStarNode<T>, int> fScores = new Dictionary<AStarNode<T>, int>();

        from.gScore = 0;
        from.fScore = heuristic(from, to);

        openQueue.Enqueue(from);

        while (!openQueue.IsEmpty()) {
            AStarNode<T> current = openQueue.Dequeue();

            if (current.Equals(to)) {
                return reconstructPath(current);
            }

            closedSet.Add(current);

            foreach (AStarNode<T> neighbour in current.GetNeighbours()) {
                if (closedSet.Contains(neighbour)) {
                    continue;
                }

                int tentativeGScore = current.gScore + cost(current, neighbour);

                if (!openQueue.Contains(neighbour)) {
                    openQueue.Enqueue(neighbour);
                } else if (tentativeGScore >= neighbour.gScore) {
                    continue;
                }

                neighbour.cameFrom = current;
                neighbour.gScore = tentativeGScore;
                neighbour.fScore = neighbour.gScore + heuristic(neighbour, to);
            }
        }

        return new List<AStarNode<T>>();
    }

    private List<AStarNode<T>> reconstructPath(AStarNode<T> current) {
        List<AStarNode<T>> path = new List<AStarNode<T>>();
        path.Add(current);
        while (current.cameFrom != null) {
            path.Add(current.cameFrom);
            current = current.cameFrom;
        }
        path.Reverse();
        return path;
    }
}

public abstract class AStarNode<T> : IComparable<AStarNode<T>> {
    public int gScore;
    public int fScore;
    public AStarNode<T> cameFrom;
    public readonly T data;

    public AStarNode(T data) {
        this.gScore = Int32.MaxValue;
        this.fScore = Int32.MaxValue;
        this.data = data;
    }

    public abstract List<AStarNode<T>> GetNeighbours();

    public int CompareTo(AStarNode<T> obj) {
        if (this.fScore < obj.fScore) {
            return -1;
        } else if (this.fScore > obj.fScore) {
            return 1;
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
            if (leftChildIndex > lastIndex) {
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

    public override string ToString() {
        string s = "Priority queue is: ";
        for (int i = 0; i < data.Count; ++i)
            s += data[i].ToString() + ", ";
        s += "count = " + data.Count;
        return s;
    }
}