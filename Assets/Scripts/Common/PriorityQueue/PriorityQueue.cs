using System.Collections.Generic;

public class PriorityQueue<T, U> : IPriorityQueue<T, U>
{
    public int Count => elements.Count;
    private SortedList<T, U> elements;
    private Dictionary<U, T> prioritiesByElement;

    public PriorityQueue() {
        elements = new SortedList<T, U>();
        prioritiesByElement = new Dictionary<U, T>();
    }

    public U Dequeue()
    {
        U result = elements.Values[0];
        elements.RemoveAt(0);
        prioritiesByElement.Remove(result);
        return result;
    }
    // todo support reverse priority

    public void Enqueue(T priority, U element)
    {
        elements.Add(priority, element);
        prioritiesByElement.Add(element, priority);
    }

    public T PriorityOf(U element) {
        // todo element may not be in dict
        return prioritiesByElement[element];
    }

    public bool Contains(U element) {
        return prioritiesByElement.ContainsKey(element);
    }
}