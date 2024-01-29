using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MinHeap<T> where T : IComparable<T>
{
    private List<T> heap;

    public MinHeap()
    {
        heap = new List<T>();
    }

    public int Count
    {
        get { return heap.Count; }
    }

    public void Add(T item)
    {
        heap.Add(item);
        HeapifyUp();
    }

    public T RemoveMin()
    {
        if (heap.Count == 0)
        {
            throw new InvalidOperationException("Heap is empty");
        }

        T min = heap[0];
        int lastIdx = heap.Count - 1;

        heap[0] = heap[lastIdx];
        heap.RemoveAt(lastIdx);

        if (heap.Count > 1)
        {
            HeapifyDown();
        }
        return min;
    }

    public bool Contains(T item)
    {
        return heap.Contains(item);
    }

    private void HeapifyUp()
    {
        int childIndex = heap.Count - 1;

        while (childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;

            if (heap[childIndex].CompareTo(heap[parentIndex]) >= 0)
            {
                break;
            }

            Swap(childIndex, parentIndex);
            childIndex = parentIndex;
        }
    }

    private void HeapifyDown()
    {
        int parentIndex = 0;

        while (true)
        {
            int leftChildIndex = 2 * parentIndex + 1;
            int rightChildIndex = 2 * parentIndex + 2;
            int smallestChildIndex = parentIndex;

            if (leftChildIndex < heap.Count && heap[leftChildIndex].CompareTo(heap[smallestChildIndex]) < 0)
            {
                smallestChildIndex = leftChildIndex;
            }

            if (rightChildIndex < heap.Count && heap[rightChildIndex].CompareTo(heap[smallestChildIndex]) < 0)
            {
                smallestChildIndex = rightChildIndex;
            }

            if (smallestChildIndex == parentIndex)
            {
                break;
            }

            Swap(parentIndex, smallestChildIndex);
            parentIndex = smallestChildIndex;
        }
    }

    private void Swap(int index1, int index2)
    {
        T temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }
}
