/// <summary>
/// A basic priority queue. Although the implementation is not required to do
/// so, implementing classes are expected to use a lowest-priority-score first
/// model, e.g. p(x) < p(y) where x has higher priority than y.
/// </summary>
/// <typeparam name="T">The type of the priority value.</typeparam>
/// <typeparam name="U">The type of the queued element.</typeparam>
public interface IPriorityQueue<T, U> {
    /// <summary>
    /// Return the number of elements in the queue.
    /// </summary>
    int Count {get;}

    /// <summary>
    /// Add an item to the queue.
    /// </summary>
    /// <param name="priority">The priority at which to add the item.</param>
    /// <param name="element">The element to be enqueued.</param>
    void Enqueue(T priority, U element);

    /// <summary>
    /// Remove the item with the highest priority.
    /// </summary>
    /// <returns>An element of type U.</returns>
    U Dequeue();

    /// <summary>
    /// Get the priority value of an element. The interface does not define
    /// behavior for cases where the element is not contained in the
    /// dictionary.
    /// </summary>
    /// <param name="element">The element for which to query.</param>
    /// <returns>The priority object related to the element.</returns>
    /// //todo specify what should happen if the object is not in the queue
    T PriorityOf(U element);

    /// <summary>
    /// Return whether the queue contains the given element.
    /// </summary>
    /// <param name="element">The element to query for.</param>
    /// <returns>True if the queue contains the given element, false otherwise.</returns>
    bool Contains(U element);
}