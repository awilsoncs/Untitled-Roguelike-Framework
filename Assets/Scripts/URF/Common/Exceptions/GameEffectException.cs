
namespace URF.Common.Exceptions {
  using System;
  using System.Runtime.Serialization;
  using URF.Effects;

  /// <summary>
  /// Thrown by malformed effects.
  /// </summary>
  [Serializable]
  public class GameEffectException : Exception {

    // the offending game effect
    public IEffect Effect {
      get;
    }

    public GameEffectException(IEffect effect, string message) : base(message) {
      this.Effect = effect;
    }
    public GameEffectException() : base() { }
    public GameEffectException(string message) : base(message) { }
    public GameEffectException(string message, Exception exception)
      : base(message, exception) { }
    protected GameEffectException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
  }
}
