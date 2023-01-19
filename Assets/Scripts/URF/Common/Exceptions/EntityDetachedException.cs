
namespace URF.Common.Exceptions {
  using System;
  using System.Runtime.Serialization;

  /// <summary>
  /// Thrown by the game state whenever an operation is attempted on an entity that has not been
  /// created.
  /// </summary>
  [Serializable]
  public class EntityDetachedException : Exception {
    public EntityDetachedException() : base() { }
    public EntityDetachedException(string message) : base(message) { }
    public EntityDetachedException(string message, Exception exception)
      : base(message, exception) { }
    protected EntityDetachedException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
  }
}
