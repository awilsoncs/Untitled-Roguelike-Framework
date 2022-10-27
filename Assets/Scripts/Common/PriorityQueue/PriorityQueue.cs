using System.Collections.Generic;

public class PriorityQueue<T, U> : IPriorityQueue<T, U>
{
    public int Count => elements.Count;
    private SortedList<T, U> elements;

    public PriorityQueue() {
        elements = new SortedList<T, U>();
    }

    public T Dequeue()
    {
        throw new System.NotImplementedException();
    }

    public void Enqueue(T element, U priority)
    {
        throw new System.NotImplementedException();
    }
}