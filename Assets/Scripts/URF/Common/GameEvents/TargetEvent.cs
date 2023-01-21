namespace URF.Common.GameEvents {
  using System;
  using System.Collections.Generic;
  using URF.Common.Entities;

  public class TargetEvent : EventArgs, IGameEvent {

    public enum TargetEventMethod {
      Request,
      Response,
      Cancelled
    }

    public TargetEventMethod Method {
      get;
    }

    public IEnumerable<IEntity> Targets {
      get;
    } = new List<IEntity>();

    public TargetEvent(TargetEventMethod method) {
      this.Method = method;
    }

    public TargetEvent(TargetEventMethod method, IEnumerable<IEntity> targets) {
      this.Method = method;
      this.Targets = targets;
    }

    public TargetEvent Select(IEntity target) {
      return new(
        TargetEventMethod.Response,
        new List<IEntity>() {
          target
        }
      );
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleTargetEvent(this);
    }
  }
}
