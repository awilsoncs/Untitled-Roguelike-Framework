using NUnit.Framework;

namespace Tests.Common
{
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
            Assert.That(queue.Count, Is.EqualTo(0), "Queue count should begin at 0.");
            queue.Enqueue(2, 2);
            Assert.That(queue.Count, Is.EqualTo(1), "Queue count should increment after enqueuing.");
        }

        [Test]
        public void CountDecreasesAfterDequeue() {
            queue.Enqueue(1, 1);
            queue.Enqueue(2, 2);
            Assert.That(queue.Count, Is.EqualTo(2), "Queue count should be at 2 after enqueuing twice.");
            queue.Dequeue();
            Assert.That(queue.Count, Is.EqualTo(1), "Queue should decrement after dequeuing");
            queue.Dequeue();
            Assert.That(queue.Count, Is.EqualTo(0), "Queue should decrement twice after dequeuing twice.");
        }

        [Test]
        public void QueueMaintainsProperOrder() {
            var stringQueue = new PriorityQueue<int, string>();
            // requires 3 to ensure we're not a standard queue or stack
            stringQueue.Enqueue(1, "there");
            stringQueue.Enqueue(2, "world");
            stringQueue.Enqueue(0, "hello");
            var firstString = stringQueue.Dequeue();
            Assert.That(firstString, Is.EqualTo("hello"), "Queue should place lowest priority score item at front.");
            var secondString = stringQueue.Dequeue();
            Assert.That(secondString, Is.EqualTo("there"), "Queue should place middle priority score item in the middle.");
            var thirdString = stringQueue.Dequeue();
            Assert.That(thirdString, Is.EqualTo("world"), "Queue should place highest priority score item at the back.");
        }

        [Test]
        public void QueueCanBeQueriedForElementPriority() {
            queue.Enqueue(1, 1);
            queue.Enqueue(2, 3);
            queue.Enqueue(5, 6);

            Assert.That(queue.PriorityOf(1), Is.EqualTo(1), "Queue should return correct priority when queried.");
            Assert.That(queue.PriorityOf(3), Is.EqualTo(2), "Queue should return correct priority when queried.");
            Assert.That(queue.PriorityOf(6), Is.EqualTo(5), "Queue should return correct priority when queried.");
        }

        [Test]
        public void QueueReportsItContainsContainedElement() {
            queue.Enqueue(2, 3);
            Assert.IsTrue(queue.Contains(3), "Queue should report that it contains an item it contains.");
        }

    
        [Test]
        public void QueueReportsItDoesNotContainMissingElement() {
            queue.Enqueue(2, 3);
            Assert.IsFalse(queue.Contains(5), "Queue should report that it does not contain an item it doesn't contain.");
        }

        [Test]
        public void TopElementCanBePeeked() {
            queue.Enqueue(3, 0);
            Assert.That(queue.Peek(), Is.EqualTo(0), "Queue should be able to be peeked.");
            queue.Enqueue(4, 1);
            Assert.That(queue.Peek(), Is.EqualTo(0), "Peek should return the highest value item.");
            Assert.That(queue.Peek(), Is.EqualTo(0), "Peeking should not alter the queue.");
        }

        [Test]
        public void ElementPriorityCanBeUpdated() {
            queue.Enqueue(3, 1);
            queue.Enqueue(4, 2);
            Assert.That(queue.Peek(), Is.EqualTo(1), "Highest priority item should be peeked.");
            queue.UpdatePriority(0, 2);
            Assert.That(queue.Peek(), Is.EqualTo(2), "Updating element priority should change the result of peek.");
        }
    }
}
