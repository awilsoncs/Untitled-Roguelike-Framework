using System;
using System.Collections.Generic;

public class PriorityQueue<T, U> : IPriorityQueue<T, U>
    where T : IComparable 
{
    public int Count => elements.Count;
    private List<(T, U)> elements;
    private Dictionary<U, T> prioritiesByElement;

    public PriorityQueue() {
        elements = new List<(T, U)>();
        prioritiesByElement = new Dictionary<U, T>();
    }

    public U Dequeue()
    {
        U result = elements[0].Item2;
        elements.RemoveAt(0);
        prioritiesByElement.Remove(result);
        return result;
    }
    // todo support reverse priority

    public void Enqueue(T priority, U element)
    {
        if (elements.Count == 0) {
            // only item can just drop in
            elements.Add((priority, element));
        } else {
            // otherwise, find the right index
            for (int i = 0; i <= elements.Count; i++) {
                if (i == elements.Count) {
                    // we're at the end, just add
                    elements.Add((priority, element));
                    break;
                } else if (elements[i].Item1.CompareTo(priority) > 0) {
                    // we need to insert at this index
                    elements.Insert(i, (priority, element));
                    break;
                }
            }
        }
        prioritiesByElement.Add(element, priority);
    }

    public T PriorityOf(U element) {
        // todo element may not be in dict
        return prioritiesByElement[element];
    }

    public bool Contains(U element) {
        return prioritiesByElement.ContainsKey(element);
    }

    public U Peek()
    {
        // todo handle empty peek case
        return elements[0].Item2;
    }

    public void UpdatePriority(T priority, U element)
    {
        var elementKey = (prioritiesByElement[element], element);
        // todo could binary search
        int index = elements.IndexOf(elementKey);
        elements.RemoveAt(index);
        prioritiesByElement.Remove(element);
        Enqueue(priority, element);
    }
}