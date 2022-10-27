public interface IPriorityQueue<T, U> {
    int Count {get;}
    void Enqueue(T element, U priority);
    T Dequeue();
}