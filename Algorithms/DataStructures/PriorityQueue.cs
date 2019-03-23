using System;
using System.Collections.Generic;

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