using System.Collections.Generic;
using UnityEngine;

public partial class Game : IGameClient {
    private List<Pawn> pawns;
    private Dictionary<int, Pawn> pawns_by_id;
    [SerializeField] public PawnFactory pawnFactory;
    const float GRID_MULTIPLE = 0.5f;

    public void PostEvent(IGameEvent ev) {
        switch (ev.EventType) {
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
            case GameEventType.FieldOfViewUpdated:
                HandleFieldOfViewUpdatedEvent((FieldOfViewUpdatedEvent)ev);
                return;
            case GameEventType.MainCharacterChanged:
                HandleMainCharacterChangedEvent((MainCharacterChangedEvent)ev);
                return;
            case GameEventType.EntityAttacked:
                HandleEntityAttacked((EntityAttackedEvent) ev);
                return;
            default:
                Debug.Log($"Unhandled GameEventType {ev.EventType}");
                return; 
        }
    }

    private void HandleEntityMoved(EntityMovedEvent ev) {
        Pawn pawn = pawns_by_id[ev.EntityID];
        int x = ev.Position.Item1;
        int y = ev.Position.Item2;
        pawn.transform.position = new Vector3(x*GRID_MULTIPLE, y*GRID_MULTIPLE, 0f);

        // Below code is for smart client actions (bump attack instead of attempt move)
        if (entityPosition.ContainsKey(ev.EntityID)) {
            (int x0, int y0) = entityPosition[ev.EntityID];
            entityMap[x0][y0] = (-1, false);
        }
        entityPosition[ev.EntityID] = (x, y);
        entityMap[x][y] = (ev.EntityID, true);

        if (ev.EntityID == mainCharacterId) {
            mainCharacterPosition = (x, y);
        }
    }

    private void HandleEntityCreated(EntityCreatedEvent ev) {
        IEntity entity = ev.Entity;
        int id = entity.ID;
        string appearance = entity.Appearance;
        Pawn pawn = pawnFactory.Get(appearance);
        pawn.EntityId = id;
        pawns.Add(pawn);
        pawn.gameObject.name = $"Pawn::{id} {appearance}";
        Debug.Log($"Pawn created {id}::{appearance}");
        pawns_by_id[id] = pawn;
        if(!entity.IsVisible && usingFOV) {
            pawn.gameObject.SetActive(false);
        }
    }

    private void HandleEntityKilled(EntityKilledEvent ev) {
        Debug.Log($"Entity {ev.EntityId} has been killed.");
        int id = ev.EntityId;
        if (id == mainCharacterId) {
            Debug.Log("Player died, reloading...");
            ClearGame();
            BeginNewGame();
        }

        Pawn pawn = pawns_by_id[id];
        // todo consider tracking save index
        int index = pawns.FindIndex(x => {return x.EntityId == id;});
        int lastIndex = pawns.Count - 1;
        if (index < lastIndex) {
            pawns[index] = pawns[lastIndex];
        }
        pawns.RemoveAt(lastIndex);
        pawns_by_id.Remove(id);
        pawn.Recycle(pawnFactory);

        (int x, int y) = entityPosition[id];
        entityMap[x][y] = (-1, false);
        entityPosition.Remove(id); 
    }

    private void HandleEntityVisibilityChanged(
        EntityVisibilityChangedEvent ev
    ) {
        int id = ev.EntityID;
        bool newVis = ev.NewVisibility;
        pawns_by_id[id].IsVisible = newVis;
        if (usingFOV || newVis) {
            pawns_by_id[id].gameObject.SetActive(newVis);
        }
    }
    
    private void HandleGameErrorEvent(GameErrorEvent ev) {
        string message = ev.Message;
        Debug.LogError(message);
    }

    private void HandleFieldOfViewUpdatedEvent(FieldOfViewUpdatedEvent ev) {
        Debug.Log("Field of view was updated.");
    }

    private void HandleMainCharacterChangedEvent(MainCharacterChangedEvent ev) {
        IEntity mainCharacter = ev.Entity;
        mainCharacterId = ev.Entity.ID;
        mainCharacterPosition = entityPosition[mainCharacterId];
        gui.healthBar.CurrentHealth =  mainCharacter.GetPart<FighterPart>().CurrentHealth;
        gui.healthBar.MaximumHealth = mainCharacter.GetPart<FighterPart>().MaxHealth;
        // todo should link updates to properties
        gui.healthBar.UpdateHealthBar();
    }

    private void HandleEntityAttacked(EntityAttackedEvent ev) {
        gui.messageBox.AddMessage($"{ev.Attacker} attacked {ev.Defender} for {ev.Damage} damage!");
        if (ev.Defender == mainCharacterId && ev.Success) {
            gui.healthBar.CurrentHealth -= ev.Damage;
            gui.healthBar.UpdateHealthBar();
        }
    }
}