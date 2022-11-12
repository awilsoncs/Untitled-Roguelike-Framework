using System.Collections.Generic;
using UnityEngine;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Server.RulesSystems;

namespace URF.Client {
  public partial class GameClient {

    [SerializeField] private PawnFactory pawnFactory;

    private readonly List<Pawn> _pawns = new();

    private readonly Dictionary<int, Pawn> _pawnsByID = new();

    private const float gridMultiple = 0.5f;

    public void PostEvent(IGameEvent ev) {
      switch(ev.EventType) {
        case GameEventType.EntityMoved:
          HandleEntityMoved((EntityMovedEvent)ev);
          return;
        case GameEventType.EntityCreated:
          HandleEntityCreated((EntityCreatedEvent)ev);
          return;
        case GameEventType.EntityKilled:
          HandleEntityKilled((EntityKilledEvent)ev);
          return;
        case GameEventType.EntityVisibilityChanged:
          HandleEntityVisibilityChanged((EntityVisibilityChangedEvent)ev);
          return;
        case GameEventType.GameError:
          HandleGameErrorEvent((GameErrorEvent)ev);
          return;
        case GameEventType.MainCharacterChanged:
          HandleMainCharacterChangedEvent((MainCharacterChangedEvent)ev);
          return;
        case GameEventType.EntityAttacked:
          HandleEntityAttacked((EntityAttackedEvent)ev);
          return;
        default:
          Debug.Log($"Unhandled GameEventType {ev.EventType}");
          return;
      }
    }

    private void HandleEntityMoved(EntityMovedEvent ev) {
      Pawn pawn = _pawnsByID[ev.Entity.ID];
      int x = ev.Position.X;
      int y = ev.Position.Y;
      pawn.transform.position = new Vector3(x * gridMultiple, y * gridMultiple, 0f);
      if(_entityPosition.ContainsKey(ev.Entity.ID)) {
        (int x0, int y0) = _entityPosition[ev.Entity.ID];
        _entitiesByPosition[x0][y0].Remove(ev.Entity);
      }
      _entityPosition[ev.Entity.ID] = (x, y);
      _entitiesByPosition[x][y].Add(ev.Entity);

      if(ev.Entity.ID == _mainCharacterId) { _mainCharacterPosition = (x, y); }
    }

    private void HandleEntityCreated(EntityCreatedEvent ev) {
      IEntity entity = ev.Entity;
      int id = entity.ID;
      EntityInfo info = entity.GetComponent<EntityInfo>();
      string appearance = info.Appearance;
      Pawn pawn = pawnFactory.Get(appearance);
      pawn.EntityId = id;
      _pawns.Add(pawn);
      pawn.gameObject.name = $"Pawn::{id} {appearance}";
      Debug.Log($"Pawn created {id}::{appearance}");
      _pawnsByID[id] = pawn;
      if(!entity.IsVisible && _usingFOV) { pawn.gameObject.SetActive(false); }
    }

    private void HandleEntityKilled(EntityKilledEvent ev) {
      EntityInfo info = ev.Entity.GetComponent<EntityInfo>();
      Debug.Log($"Entity {info.Name} has been killed.");
      int id = ev.Entity.ID;
      if(id == _mainCharacterId) {
        Debug.Log("Player died, reloading...");
        ClearGame();
        BeginNewGame();
      }

      Pawn pawn = _pawnsByID[id];
      // todo consider tracking save index
      int index = _pawns.FindIndex(t => t.EntityId == id);
      int lastIndex = _pawns.Count - 1;
      if(index < lastIndex) { _pawns[index] = _pawns[lastIndex]; }
      _pawns.RemoveAt(lastIndex);
      _pawnsByID.Remove(id);
      pawn.Recycle(pawnFactory);

      (int x, int y) = _entityPosition[id];
      _entityPosition.Remove(id);
      _entitiesByPosition[x][y].Remove(ev.Entity);
    }

    private void HandleEntityVisibilityChanged(EntityVisibilityChangedEvent ev) {
      int id = ev.Entity.ID;
      bool newVis = ev.NewVisibility;
      _pawnsByID[id].IsVisible = newVis;
      if(_usingFOV || newVis) { _pawnsByID[id].gameObject.SetActive(newVis); }
    }

    private void HandleGameErrorEvent(GameErrorEvent ev) {
      string message = ev.Message;
      Debug.LogError(message);
    }

    private void HandleMainCharacterChangedEvent(MainCharacterChangedEvent ev) {
      IEntity mainCharacter = ev.Entity;
      _mainCharacterId = ev.Entity.ID;
      _mainCharacterPosition = _entityPosition[_mainCharacterId];
      CombatComponent stats = mainCharacter.GetComponent<CombatComponent>();
      gui.HealthBar.CurrentHealth = stats.CurrentHealth;
      gui.HealthBar.MaximumHealth = stats.MaxHealth;
      // todo should link updates to properties
      gui.HealthBar.UpdateHealthBar();
    }

    private void HandleEntityAttacked(EntityAttackedEvent ev) {
      EntityInfo attackerInfo = ev.Attacker.GetComponent<EntityInfo>();
      EntityInfo defenderInfo = ev.Defender.GetComponent<EntityInfo>();

      string attackerName = attackerInfo.Name;
      string defenderName = defenderInfo.Name;

      gui.MessageBox.AddMessage($"{attackerName} attacked {defenderName} for {ev.Damage} damage!");
      if(ev.Defender.ID != _mainCharacterId || !ev.Success) return;
      gui.HealthBar.CurrentHealth -= ev.Damage;
      gui.HealthBar.UpdateHealthBar();
    }

  }
}
