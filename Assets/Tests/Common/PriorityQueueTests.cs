using NUnit.Framework;
using URF.Common.PriorityQueue;

namespace Tests.Common {
  public class PriorityQueueTests {
    private PriorityQueue<int, int> _queue;

    [SetUp]
    public void Setup() {
      _queue = new PriorityQueue<int, int>();
    }

    [Test]
    public void CountIncreasesAfterEnqueue() {
      Assert.That(_queue.Count, Is.EqualTo(0), "Queue count should begin at 0.");
      _queue.Enqueue(2, 2);
      Assert.That(_queue.Count, Is.EqualTo(1), "Queue count should increment after enqueuing.");
    }

    [Test]
    public void CountDecreasesAfterDequeue() {
      _queue.Enqueue(1, 1);
      _queue.Enqueue(2, 2);
      Assert.That(_queue.Count, Is.EqualTo(2), "Queue count should be at 2 after enqueuing twice.");
      _queue.Dequeue();
      Assert.That(_queue.Count, Is.EqualTo(1), "Queue should decrement after dequeuing");
      _queue.Dequeue();
      Assert.That(_queue.Count, Is.EqualTo(0),
        "Queue should decrement twice after dequeuing twice.");
    }

    [Test]
    public void QueueMaintainsProperOrder() {
      var stringQueue = new PriorityQueue<int, string>();
      // requires 3 to ensure we're not a standard queue or stack
      stringQueue.Enqueue(1, "there");
      stringQueue.Enqueue(2, "world");
      stringQueue.Enqueue(0, "hello");
      string firstString = stringQueue.Dequeue();
      Assert.That(firstString, Is.EqualTo("hello"),
        "Queue should place lowest priority score item at front.");
      string secondString = stringQueue.Dequeue();
      Assert.That(secondString, Is.EqualTo("there"),
        "Queue should place middle priority score item in the middle.");
      string thirdString = stringQueue.Dequeue();
      Assert.That(thirdString, Is.EqualTo("world"),
        "Queue should place highest priority score item at the back.");
    }

    [Test]
    public void QueueCanBeQueriedForElementPriority() {
      _queue.Enqueue(1, 1);
      _queue.Enqueue(2, 3);
      _queue.Enqueue(5, 6);

      Assert.That(_queue.PriorityOf(1), Is.EqualTo(1),
        "Queue should return correct priority when queried.");
      Assert.That(_queue.PriorityOf(3), Is.EqualTo(2),
        "Queue should return correct priority when queried.");
      Assert.That(_queue.PriorityOf(6), Is.EqualTo(5),
        "Queue should return correct priority when queried.");
    }

    [Test]
    public void QueueReportsItContainsContainedElement() {
      _queue.Enqueue(2, 3);
      Assert.IsTrue(_queue.Contains(3),
        "Queue should report that it contains an item it contains.");
    }


    [Test]
    public void QueueReportsItDoesNotContainMissingElement() {
      _queue.Enqueue(2, 3);
      Assert.IsFalse(_queue.Contains(5),
        "Queue should report that it does not contain an item it doesn't contain.");
    }

    [Test]
    public void TopElementCanBePeeked() {
      _queue.Enqueue(3, 0);
      Assert.That(_queue.Peek(), Is.EqualTo(0), "Queue should be able to be peeked.");
      _queue.Enqueue(4, 1);
      Assert.That(_queue.Peek(), Is.EqualTo(0), "Peek should return the highest value item.");
      Assert.That(_queue.Peek(), Is.EqualTo(0), "Peeking should not alter the queue.");
    }

    [Test]
    public void ElementPriorityCanBeUpdated() {
      _queue.Enqueue(3, 1);
      _queue.Enqueue(4, 2);
      Assert.That(_queue.Peek(), Is.EqualTo(1), "Highest priority item should be peeked.");
      _queue.UpdatePriority(0, 2);
      Assert.That(_queue.Peek(), Is.EqualTo(2),
        "Updating element priority should change the result of peek.");
    }
  }
}
