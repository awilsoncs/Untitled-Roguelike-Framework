
using System;
using URF.Effects;
/// <summary>
/// Thrown by malformed effects.
/// </summary>
public class GameEffectException : Exception {

  // the offending game event
  public IEffect Effect {
    get;
  }

  public GameEffectException(IEffect effect, string message) : base(message) {
    this.Effect = effect;
  }
}
