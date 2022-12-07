
using System;
/// <summary>
/// Thrown by the game state whenever an operation is attempted on an entity that has not been
/// created.
/// </summary>
public class EntityDetachedException : Exception {
  public EntityDetachedException(string message) : base(message) { }
}
