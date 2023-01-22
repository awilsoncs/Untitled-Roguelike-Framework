namespace URF.Common.GameEvents {
  using System;
  using URF.Server.Resolvables;

  public class ResolvableEvent : EventArgs, IGameEvent {

    public enum ResolvableEventStep {
      // Notify the targeting system that we need to deploy a target request
      TargetDetermination,
      // All requirements are fulfilled, we can generate effect events.
      Resolved,
      // The user canceled this action.
      Cancelled
    }

    public IResolvable Resolvable {
      get;
    }

    public ResolvableEventStep Step {
      get;
    }

    public ResolvableEvent(IResolvable resolvable, ResolvableEventStep step) {
      this.Resolvable = resolvable;
      this.Step = step;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleResolvableEvent(this);
    }
  }
}
