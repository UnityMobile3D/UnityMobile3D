using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private readonly List<T> _heap;
    private readonly IComparer<T> _comparer;

    public PriorityQueue() : 
        this(Comparer<T>.Default) { }

    public PriorityQueue(IComparer<T> comparer)
    {
        _heap = new List<T>();
        _comparer = comparer ?? Comparer<T>.Default;
    }

    public int Count => _heap.Count;
    public bool Any() => _heap.Count > 0;
    public void Clear() { _heap.Clear(); }

    public void Enqueue(T item)
    {
        _heap.Add(item);
        SiftUp(_heap.Count - 1);
    }

    public bool IsEmpty() => _heap.Count == 0;

    public T Peek()
    {
        if (_heap.Count == 0) 
            throw new InvalidOperationException("Empty PQ");

        return _heap[0];
    }

    public T Dequeue()
    {
        if (_heap.Count == 0) 
            throw new InvalidOperationException("Empty PQ");

        T root = _heap[0];
        int last = _heap.Count - 1;

        //�ֻ��� �� �� ����������(O(1) ���� ����)
        _heap[0] = _heap[last];
        _heap.RemoveAt(last);

        //0�� ��� �´� ��ġ���� ������
        if (_heap.Count > 0) 
            SiftDown(0);

        return root;
    }

    private void SiftUp(int idx)
    {
        while (idx > 0)
        {
            //�� �θ� ���� ���� ũ�ų� ������ �ø���
            int parent = (idx - 1) / 2;
            if (_comparer.Compare(_heap[idx], _heap[parent]) >= 0) 
                break;

            Swap(idx, parent);
            idx = parent;
        }
    }

    //O(log n)
    private void SiftDown(int idx)
    {
        int n = _heap.Count;
        while (true)
        {
            int left = idx * 2 + 1;
            int right = left + 1;

            int smallest = idx;

            //�� �ڽ� ����� ���� �� ���� �ڽİ� ��ü
            if (left < n && _comparer.Compare(_heap[left], _heap[smallest]) < 0) 
                smallest = left;
            if (right < n && _comparer.Compare(_heap[right], _heap[smallest]) < 0) 
                smallest = right;

            //�� ������ ���ٸ� ���� ��ġ�� ���
            if (smallest == idx) 
                break;

            Swap(idx, smallest);
            idx = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        T t = _heap[a];
        _heap[a] = _heap[b];
        _heap[b] = t;
    }
}
