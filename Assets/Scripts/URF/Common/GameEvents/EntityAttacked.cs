namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  public class EntityAttacked : EventArgs, IGameEvent {

    /// <summary>The attacking entity's entity ID</summary>
    public IEntity Attacker {
      get;
    }

    /// <summary>The defending entity's entity ID</summary>
    public IEntity Defender {
      get;
    }

    /// <summary>True if the attack was successful</summary>
    public bool Success {
      get;
    }

    public int Damage {
      get;
    }

    public EntityAttacked(IEntity attacker, IEntity defender, bool success, int damage) {
      this.Attacker = attacker;
      this.Defender = defender;
      this.Success = success;
      this.Damage = damage;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEntityAttacked(this);
    }
  }
}
