using System.Collections.Generic;

public class PriorityQueue<T, U> : IPriorityQueue<T, U>
{
    public int Count => elements.Count;
    private SortedList<T, U> elements;

    public PriorityQueue() {
        elements = new SortedList<T, U>();
    }

    public U Dequeue()
    {
        U result = elements.Values[0];
        elements.RemoveAt(0);
        return result;
    }

    public void Enqueue(T priority, U element)
    {
        elements.Add(priority, element);
    }
}