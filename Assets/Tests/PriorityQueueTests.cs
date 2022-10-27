using NUnit.Framework;

public class PriorityQueueTests
{
    PriorityQueue<int, int> queue;
    [SetUp]
    public void Setup() {
        queue = new PriorityQueue<int, int>();
    }

    [Test]
    public void PriorityQueueCanEnqueue()
    {
        queue.Enqueue(1, 1);
    }

    [Test]
    public void CountIncreasesAfterEnqueue()
    {
        Assert.That(queue.Count, Is.EqualTo(0));
        queue.Enqueue(2, 2);
        Assert.That(queue.Count, Is.EqualTo(1));
    }

    [Test]
    public void CountDecreasesAfterDequeue() {
        queue.Enqueue(1, 1);
        queue.Enqueue(2, 2);
        Assert.That(queue.Count, Is.EqualTo(2));
        queue.Dequeue();
        Assert.That(queue.Count, Is.EqualTo(1));
        queue.Dequeue();
        Assert.That(queue.Count, Is.EqualTo(0));
    }

    [Test]
    public void QueueMaintainsProperOrder() {
        var stringQueue = new PriorityQueue<int, string>();
        // requires 3 to ensure we're not a standard queue or stack
        stringQueue.Enqueue(1, "there");
        stringQueue.Enqueue(2, "world");
        stringQueue.Enqueue(0, "hello");
        var firstString = stringQueue.Dequeue();
        Assert.That(firstString, Is.EqualTo("hello"));
        var secondString = stringQueue.Dequeue();
        Assert.That(secondString, Is.EqualTo("there"));
        var thirdString = stringQueue.Dequeue();
        Assert.That(thirdString, Is.EqualTo("world"));
    }
}
