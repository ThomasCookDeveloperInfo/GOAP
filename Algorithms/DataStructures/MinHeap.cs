using System;

public class MinHeap<T> where T : IComparable<T> {
    private readonly T[] items;
    public readonly int capacity;
    public int count { get; private set; }

    public MinHeap(int capacity) {
        if (capacity < 2)
            capacity = 2;

        this.capacity = capacity;
        items = new T[this.capacity];
        this.count = 0;
    }

    public bool Insert(T item) {
        if (this.count == this.capacity)
            return false;

        this.items[this.count] = item;
        this.count++;
        PercolateUp(this.count - 1);
        return true;
    }

    public T Pop() {
        if (this.count == 0)
            throw new InvalidOperationException("Min heap is empty");

        if (this.count == 1) {
            this.count--;
            return this.items[this.count];
        }

        T min = this.items[0];
        this.items[0] = this.items[this.count - 1];
        this.count--;
        PercolateDown(0);
        return min;
    }

    public T Peek() {
        if (this.count == 0)
            throw new InvalidOperationException("Min heap is empty");

        return this.items[0];
    }

    public bool Remove(T item) {
        int index = -1;
        for (int i = 0; i < this.count; i++) {
            if (this.items[i].Equals(item)) {
                index = i;
                break;
            }
        }

        if (index == -1)
            return false;

        this.count--;
        Swap(index, this.count);

        if (LeftLessThanRight(index, (index - 1) / 2))
            PercolateUp(index);
        else
            PercolateDown(index);

        return true;
    }

    private void PercolateDown(int index) {
        while (true) {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int largest = index;

            if (left < this.count && LeftLessThanRight(left, largest))
                largest = left;
            if (right < this.count && LeftLessThanRight(right, largest))
                largest = right;
            if (largest == index)
                return;

            Swap(index, largest);
            index = largest;
        }
    }

    private void PercolateUp(int index) {
        while (true) {
            if (index >= this.count || index <= 0)
                return;

            int parent = (index - 1) / 2;

            if (LeftLessThanRight(parent, index))
                return;

            Swap(index, parent);
            index = parent;
        }
    }

    private bool LeftLessThanRight(int left, int right) {
        return this.items[left].CompareTo(this.items[right]) < 0;
    }

    private void Swap(int left, int right) {
        T tmp = this.items[left];
        this.items[left] = this.items[right];
        this.items[right] = tmp;
    }
}