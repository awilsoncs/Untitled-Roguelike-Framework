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

    public IEnumerable<IEntity> SelectedTargets => this.selectedTargets;
    private readonly List<IEntity> selectedTargets = new();

    public TargetEvent(IResolvable resolvable) {
      this.Resolvable = resolvable;
      this.Method = TargetEventMethod.Request;
    }

    public TargetEvent(TargetEventMethod method, IResolvable resolvable) {
      this.Method = method;
      this.Resolvable = resolvable;
    }

    public TargetEvent Select(IEntity target) {
      TargetEvent ev = new(
        TargetEventMethod.Response,
        this.Resolvable
      );
      ev.selectedTargets.Add(target);
      return ev;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleTargetEvent(this);
    }
  }
}
