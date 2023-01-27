namespace URF.Common.GameEvents {
  using System;
  using System.Collections.Generic;
  using URF.Common.Entities;
  using URF.Server.Resolvables;

  public class TargetEvent : EventArgs, IGameEvent {

    public enum TargetEventMethod {
      Request,
      Response,
      Cancelled
    }

    public TargetEventMethod Method {
      get;
    }

    public IResolvable Resolvable {
      get;
    }

    public IEnumerable<IEntity> Targets => this.Resolvable.LegalTargets;

    public IEnumerable<IEntity> SelectedTargets => this.Resolvable.ResolvedTargets;

    public TargetEvent(IResolvable resolvable) {
      this.Resolvable = resolvable;
      this.Method = TargetEventMethod.Request;
    }

    public TargetEvent(TargetEventMethod method, IResolvable resolvable) {
      this.Method = method;
      this.Resolvable = resolvable;
    }

    public TargetEvent Select(IEntity target) {
      return new(
        TargetEventMethod.Response,
        this.Resolvable
      );
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleTargetEvent(this);
    }
  }
}
