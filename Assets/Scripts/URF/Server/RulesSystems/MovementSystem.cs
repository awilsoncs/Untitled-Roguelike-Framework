namespace URF.Server.RulesSystems {
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;

  public class MovementSystem : BaseRulesSystem {


    public override void HandleMoveAction(MoveAction ev) {
      IEntity entity = ev.Entity;
      Position newPosition = this.GameState.LocateEntityOnMap(entity) + ev.Direction;

      this.GameState.MoveEntity(entity, newPosition);
      this.OnGameEvent(new TurnSpent(entity));
    }

  }

}
