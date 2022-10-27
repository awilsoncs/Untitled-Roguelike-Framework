public interface IPriorityQueue<T, U> {
    int Count {get;}
    void Enqueue(T priority, U element);
    U Dequeue();
}