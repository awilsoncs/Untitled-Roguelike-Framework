using System.Collections.Generic;
using UnityEngine;
using URFCommon;


namespace URFFrontend {
    // todo refactor this to a client plugin with unit tests
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
            Pawn pawn = pawns_by_id[ev.Entity.ID];
            int x = ev.Position.X;
            int y = ev.Position.Y;
            pawn.transform.position = new Vector3(x*GRID_MULTIPLE, y*GRID_MULTIPLE, 0f);

            // Below code is for smart client actions (bump attack instead of attempt move)
            if (entityPosition.ContainsKey(ev.Entity.ID)) {
                (int x0, int y0) = entityPosition[ev.Entity.ID];
                entityMap[x0][y0] = (-1, false);
            }
            entityPosition[ev.Entity.ID] = (x, y);
            entityMap[x][y] = (ev.Entity.ID, true);

            if (ev.Entity.ID == mainCharacterId) {
                mainCharacterPosition = (x, y);
            }
        }

        private void HandleEntityCreated(EntityCreatedEvent ev) {
            IEntity entity = ev.Entity;
            int id = entity.ID;
            string appearance = entity.GetStringSlot("appearance");
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
            Debug.Log($"Entity {ev.Entity.GetStringSlot("name")} has been killed.");
            int id = ev.Entity.ID;
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
            int id = ev.Entity.ID;
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

        private void HandleMainCharacterChangedEvent(MainCharacterChangedEvent ev) {
            IEntity mainCharacter = ev.Entity;
            mainCharacterId = ev.Entity.ID;
            mainCharacterPosition = entityPosition[mainCharacterId];
            gui.healthBar.CurrentHealth =  ev.Entity.GetIntSlot("currentHealth");
            gui.healthBar.MaximumHealth = ev.Entity.GetIntSlot("maxHealth");
            // todo should link updates to properties
            gui.healthBar.UpdateHealthBar();
        }

        private void HandleEntityAttacked(EntityAttackedEvent ev) {
            var attackerName = ev.Attacker.GetStringSlot("name");
            var defenderName = ev.Defender.GetStringSlot("name");
            gui.messageBox.AddMessage(
                $"{attackerName} attacked {defenderName} for {ev.Damage} damage!");
            if (ev.Defender.ID == mainCharacterId && ev.Success) {
                gui.healthBar.CurrentHealth -= ev.Damage;
                gui.healthBar.UpdateHealthBar();
            }
        }
    }
}