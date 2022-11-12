using System;
using System.Collections.Generic;

namespace URF.Common.PriorityQueue {
  public class PriorityQueue<TPriority, TValue> where TPriority : IComparable {

    public int Count => _elements.Count;

    private readonly List<(TPriority, TValue)> _elements;

    private readonly Dictionary<TValue, TPriority> _prioritiesByElement;

    public PriorityQueue() {
      _elements = new List<(TPriority, TValue)>();
      _prioritiesByElement = new Dictionary<TValue, TPriority>();
    }

    public TValue Dequeue() {
      TValue result = _elements[0].Item2;
      _elements.RemoveAt(0);
      _prioritiesByElement.Remove(result);
      return result;
    }
    // todo support reverse priority

    public void Enqueue(TPriority priority, TValue element) {
      if(_elements.Count == 0) {
        // only item can just drop in
        _elements.Add((priority, element));
      }
      else {
        // otherwise, find the right index
        for(int i = 0; i <= _elements.Count; i++) {
          if(i == _elements.Count) {
            // we're at the end, just add
            _elements.Add((priority, element));
            break;
          }
          if(_elements[i].Item1.CompareTo(priority) > 0) {
            // we need to insert at this index
            _elements.Insert(i, (priority, element));
            break;
          }
        }
      }
      _prioritiesByElement.Add(element, priority);
    }

    public TPriority PriorityOf(TValue element) {
      // todo element may not be in dict
      return _prioritiesByElement[element];
    }

    public bool Contains(TValue element) {
      return _prioritiesByElement.ContainsKey(element);
    }

    public TValue Peek() {
      // todo handle empty peek case
      return _elements[0].Item2;
    }

    public void UpdatePriority(TPriority priority, TValue element) {
      (TPriority, TValue element) elementKey = (_prioritiesByElement[element], element);
      // todo could binary search
      int index = _elements.IndexOf(elementKey);
      _elements.RemoveAt(index);
      _prioritiesByElement.Remove(element);
      Enqueue(priority, element);
    }

  }
}
