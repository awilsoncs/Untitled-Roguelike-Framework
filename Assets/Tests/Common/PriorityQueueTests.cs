namespace Tests.Common {
  using NUnit.Framework;
  using URF.Common.PriorityQueue;

  public class PriorityQueueTests {
    private PriorityQueue<int, int> queue;

    [SetUp]
    public void Setup() {
      this.queue = new PriorityQueue<int, int>();
    }

    [Test]
    public void CountIncreasesAfterEnqueue() {
      Assert.That(this.queue.Count, Is.EqualTo(0), "Queue count should begin at 0.");
      this.queue.Enqueue(2, 2);
      Assert.That(this.queue.Count, Is.EqualTo(1), "Queue count should increment after enqueuing.");
    }

    [Test]
    public void CountDecreasesAfterDequeue() {
      this.queue.Enqueue(1, 1);
      this.queue.Enqueue(2, 2);
      Assert.That(
        this.queue.Count, Is.EqualTo(2), "Queue count should be at 2 after enqueuing twice.");
      _ = this.queue.Dequeue();
      Assert.That(this.queue.Count, Is.EqualTo(1), "Queue should decrement after dequeuing");
      _ = this.queue.Dequeue();
      Assert.That(this.queue.Count, Is.EqualTo(0),
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
      this.queue.Enqueue(1, 1);
      this.queue.Enqueue(2, 3);
      this.queue.Enqueue(5, 6);

      Assert.That(this.queue.PriorityOf(1), Is.EqualTo(1),
        "Queue should return correct priority when queried.");
      Assert.That(this.queue.PriorityOf(3), Is.EqualTo(2),
        "Queue should return correct priority when queried.");
      Assert.That(this.queue.PriorityOf(6), Is.EqualTo(5),
        "Queue should return correct priority when queried.");
    }

    [Test]
    public void QueueReportsItContainsContainedElement() {
      this.queue.Enqueue(2, 3);
      Assert.IsTrue(this.queue.Contains(3),
        "Queue should report that it contains an item it contains.");
    }


    [Test]
    public void QueueReportsItDoesNotContainMissingElement() {
      this.queue.Enqueue(2, 3);
      Assert.IsFalse(this.queue.Contains(5),
        "Queue should report that it does not contain an item it doesn't contain.");
    }

    [Test]
    public void TopElementCanBePeeked() {
      this.queue.Enqueue(3, 0);
      Assert.That(this.queue.Peek(), Is.EqualTo(0), "Queue should be able to be peeked.");
      this.queue.Enqueue(4, 1);
      Assert.That(this.queue.Peek(), Is.EqualTo(0), "Peek should return the highest value item.");
      Assert.That(this.queue.Peek(), Is.EqualTo(0), "Peeking should not alter the queue.");
    }

    [Test]
    public void ElementPriorityCanBeUpdated() {
      this.queue.Enqueue(3, 1);
      this.queue.Enqueue(4, 2);
      Assert.That(this.queue.Peek(), Is.EqualTo(1), "Highest priority item should be peeked.");
      this.queue.UpdatePriority(0, 2);
      Assert.That(this.queue.Peek(), Is.EqualTo(2),
        "Updating element priority should change the result of peek.");
    }
  }
}
