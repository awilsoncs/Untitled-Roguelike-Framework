namespace URF.Client {
  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using URF.Client.GUI;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Common.GameState;

  /// <summary>
  /// GameClient client view
  /// </summary>
  [DisallowMultipleComponent]
  public class GameClient : MonobehaviourBaseGameEventChannel {
    [SerializeField] private GuiComponents gui;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private MonobehaviourBaseGameEventChannel gameEventChannel;

    [SerializeField] private PawnFactory pawnFactory;

    [SerializeField] private KeyCode newGameKey = KeyCode.N;

    [SerializeField] private KeyCode saveKey = KeyCode.S;

    [SerializeField] private KeyCode loadKey = KeyCode.L;

    [SerializeField] private KeyCode upKey = KeyCode.UpArrow;

    [SerializeField] private KeyCode downKey = KeyCode.DownArrow;

    [SerializeField] private KeyCode leftKey = KeyCode.LeftArrow;

    [SerializeField] private KeyCode rightKey = KeyCode.RightArrow;

    [SerializeField] private KeyCode spawnKey = KeyCode.C;

    [SerializeField] private KeyCode getKey = KeyCode.G;

    [SerializeField] private KeyCode dropKey = KeyCode.D;

    [SerializeField] private KeyCode healKey = KeyCode.H;

    [SerializeField] private KeyCode mapKey = KeyCode.M;

    [SerializeField] private KeyCode cancelKey = KeyCode.Escape;

    private const float GridMultiple = 0.5f;

    private readonly List<Pawn> pawns = new();

    private readonly Dictionary<int, Pawn> pawnsByID = new();

    private IEntity mainCharacter;

    private IReadOnlyGameState<IReadOnlyCell> gameState;

    private readonly Queue<IGameEvent> gameEvents = new();

    private bool usingFOV = true;

    private TargetEvent targetRequest;

    // Unity message events appear unused to simple IDEs.
    private void Start() {
      this.gameEventChannel.Connect(this);
      this.BeginNewGame();
    }

    private void Update() {
      if (this.targetRequest == null) {
        if (this.gameEvents.Count > 0) {
          Debug.Log("Beginning client update...");
        }
        while (this.gameEvents.Count > 0) {
          IGameEvent ev = this.gameEvents.Dequeue();
          base.HandleEvent(this, ev);
        }
      }
      this.HandleUserInput();
    }

    public override void HandleEvent(object _, IGameEvent ev) {
      this.gameEvents.Enqueue(ev);
    }

    private void BeginNewGame() {
      this.ResetEverything();
      this.OnGameEvent(new ConfigureAction());
      if (this.gameEvents.Count <= 0) {
        Debug.LogError("Client received 0 events from server start.");
      }
    }

    private void ResetEverything() {
      foreach (Pawn pawn in this.pawns) {
        pawn.Recycle(this.pawnFactory);
      }
      this.pawns.Clear();
      this.pawnsByID.Clear();
      this.gameEvents.Clear();
    }

    private void ClearGame() {
      foreach (Pawn pawn in this.pawns) {
        pawn.Recycle(this.pawnFactory);
      }
      this.pawns.Clear();
      this.pawnsByID.Clear();
    }

    public override void HandleEntityLocationChanged(EntityLocationChanged ev) {
      if (ev.SubType is EntityLocationChanged.EventSubType.Moved) {
        int x = ev.NewPosition.X;
        int y = ev.NewPosition.Y;
        this.DoMove(ev.Entity, x, y);
      } else if (ev.SubType is EntityLocationChanged.EventSubType.Removed) {
        this.DeletePawn(ev.Entity);
      } else if (ev.SubType is EntityLocationChanged.EventSubType.Placed) {
        this.CreatePawn(ev.Entity, ev.NewPosition);
      }
    }

    private void DoMove(IEntity entity, int x, int y) {
      Pawn pawn = this.pawnsByID[entity.ID];
      pawn.transform.position = new Vector3(x * GridMultiple, y * GridMultiple, 0f);
    }

    private void CreatePawn(IEntity entity, Position position) {
      int id = entity.ID;
      string appearance = entity.Appearance;
      Pawn pawn = this.pawnFactory.Get(appearance);
      pawn.EntityId = id;
      this.pawns.Add(pawn);
      pawn.gameObject.name = $"Pawn::{id} {appearance}";
      Debug.Log($"Pawn created {id}::{appearance}");
      this.pawnsByID[id] = pawn;
      if (!entity.IsVisible && this.usingFOV) {
        pawn.gameObject.SetActive(false);
      }

      this.DoMove(entity, position.X, position.Y);
    }

    public override void HandleEntityEvent(EntityEvent ev) {
      if (ev.Method == EntityEvent.EntityMethod.Updated) {
        this.HandleEntityEventUpdated(ev);
      } else if (ev.Method == EntityEvent.EntityMethod.Deleted) {
        this.HandleEntityEventDeleted(ev);
      }
    }

    private void HandleEntityEventUpdated(EntityEvent ev) {
      if (ev.Entity == this.mainCharacter) {
        this.gui.HealthBar.UpdateHealthBar(this.mainCharacter.CurrentHealth);
      }
    }


    private void HandleEntityEventDeleted(EntityEvent ev) {
      if (ev.Entity == this.mainCharacter) {
        Debug.Log("Player died, reloading...");
        this.ClearGame();
        this.BeginNewGame();
      }

      if (this.pawnsByID.ContainsKey(ev.Entity.ID)) {
        this.DeletePawn(ev.Entity);
      }
    }

    public void DeletePawn(IEntity entity) {
      int id = entity.ID;

      Pawn pawn = this.pawnsByID[id];
      // todo consider tracking save index
      int index = this.pawns.FindIndex(t => t.EntityId == id);
      int lastIndex = this.pawns.Count - 1;
      if (index < lastIndex) {
        this.pawns[index] = this.pawns[lastIndex];
      }
      this.pawns.RemoveAt(lastIndex);
      _ = this.pawnsByID.Remove(id);
      pawn.Recycle(this.pawnFactory);
    }

    public override void HandleEntityVisibilityChanged(EntityVisibilityChanged ev) {
      int id = ev.Entity.ID;
      bool newVis = ev.NewVisibility;
      this.pawnsByID[id].IsVisible = newVis;
      if (this.usingFOV || newVis) {
        this.pawnsByID[id].gameObject.SetActive(newVis);
      }
    }

    public override void HandleGameErrored(GameErrored ev) {
      string message = ev.Message;
      Debug.LogError(message);
    }

    public override void HandleMainCharacterChanged(MainCharacterChanged ev) {
      this.mainCharacter = ev.Entity;
      this.gui.HealthBar.MaximumHealth = this.mainCharacter.MaxHealth;
      this.gui.HealthBar.UpdateHealthBar(this.mainCharacter.CurrentHealth);
    }

    public override void HandleEntityAttacked(EntityAttacked ev) {

      string attackerName = ev.Attacker.Name;
      string defenderName = ev.Defender.Name;

      this.gui.MessageBox.AddMessage(
        $"{attackerName} attacked {defenderName} for {ev.Damage} damage!");
      if (ev.Defender != this.mainCharacter || !ev.Success) {
        return;
      }
    }

    public override void HandleGameConfigured(GameConfigured ev) {
      if (ev == null) {
        Debug.LogError("Null GameConfigured event");
      }
      this.gameState = ev.GameState;
      this.mainCamera.transform.position = new Vector3(
        ev.GameState.MapSize.X / 2 * GridMultiple,
        ev.GameState.MapSize.Y / 2 * GridMultiple,
        -10
      );
    }

    public override void HandleInventoryEvent(InventoryEvent inventoryEvent) {
      string agent;
      if (inventoryEvent.Entity == this.mainCharacter) {
        agent = "You";
      } else {
        agent = inventoryEvent.Entity.Name;
      }
      string targetName = inventoryEvent.Item.Name;
      if (inventoryEvent.Action == InventoryEvent.InventoryAction.PickedUp) {
        this.gui.MessageBox.AddMessage($"{agent} got a {targetName}.");
      } else if (inventoryEvent.Action == InventoryEvent.InventoryAction.Used) {
        this.gui.MessageBox.AddMessage($"{agent} used a {targetName}.");
      }
    }

    public override void HandleTargetEvent(TargetEvent targetEvent) {
      if (targetEvent.Method == TargetEvent.TargetEventMethod.Request) {
        Debug.Log("Targeting request received! Entering target control mode.");
        this.gui.MessageBox.AddMessage("Select a target.");
        this.targetRequest = targetEvent;
      }
    }

    private void HandleUserInput() {
      if (this.targetRequest == null) {
        this.HandleNormalFlow();
      } else {
        this.HandleTargetSelection();
      }
    }

    private void HandleTargetSelection() {
      if (Input.GetMouseButtonDown(0)) {
        this.MouseClicked(Input.mousePosition);
      } else if (Input.GetKeyDown(this.cancelKey)) {
        this.OnGameEvent(new TargetEvent(TargetEvent.TargetEventMethod.Cancelled));
        this.gui.MessageBox.AddMessage("Action canceled.");
        this.targetRequest = null;
      }
    }

    private void HandleNormalFlow() {
      if (Input.GetMouseButtonDown(0)) {
        this.MouseClicked(Input.mousePosition);
      } else if (Input.GetKeyDown(this.leftKey)) {
        this.Move(-1, 0);
      } else if (Input.GetKeyDown(this.rightKey)) {
        this.Move(1, 0);
      } else if (Input.GetKeyDown(this.upKey)) {
        this.Move(0, 1);
      } else if (Input.GetKeyDown(this.downKey)) {
        this.Move(0, -1);
      } else if (Input.GetKeyDown(this.spawnKey)) {
        this.DebugKey();
      } else if (Input.GetKeyDown(this.mapKey)) {
        this.ToggleFieldOfView();
      } else if (Input.GetKeyDown(this.getKey)) {
        this.GetItem();
      } else if (Input.GetKeyDown(this.dropKey)) {
        this.DropItem();
      } else if (Input.GetKeyDown(this.healKey)) {
        this.Heal();
      } else if (Input.GetKeyDown(this.saveKey)) {
        this.OnGameEvent(PersistenceEvent.SaveRequested());
      } else if (Input.GetKeyDown(this.loadKey)) {
        this.ResetEverything();
        this.OnGameEvent(PersistenceEvent.LoadRequested());
      } else if (Input.GetKeyDown(this.newGameKey)) {
        this.BeginNewGame();
      } else {
        // The player didn't send any known input.
      }
    }

    private void GetItem() {
      Position position = this.gameState.LocateEntityOnMap(this.mainCharacter);
      IEntity itemToGet = this.gameState
        .GetCell(position).Contents.FirstOrDefault(x => x != this.mainCharacter);
      if (itemToGet != null) {
        this.OnGameEvent(this.mainCharacter.WantsToGet(itemToGet));
      } else {
        this.gui.MessageBox.AddMessage("There's nothing here.");
      }
    }

    private void DropItem() {
      // figure out which item to drop
      if (this.mainCharacter.Inventory.Count == 0) {
        this.gui.MessageBox.AddMessage("You're not holding anything.");
        return;
      }

      int topItemId = this.mainCharacter.Inventory[0];
      IEntity itemToDrop = this.gameState.GetEntityById(topItemId);
      this.OnGameEvent(this.mainCharacter.WantsToDrop(itemToDrop));
    }

    private void Heal() {
      if (this.mainCharacter.Inventory.Count == 0) {
        this.gui.MessageBox.AddMessage("You don't have any potions.");
        return;
      }

      if (this.mainCharacter.CurrentHealth == this.mainCharacter.MaxHealth) {
        this.gui.MessageBox.AddMessage("You're already at maximum health.");
        return;
      }


      int topItemId = this.mainCharacter.Inventory[0];
      IEntity itemToUse = this.gameState.GetEntityById(topItemId);
      this.OnGameEvent(this.mainCharacter.WantsToUse(itemToUse));
    }

    private void MouseClicked(Vector3 clickPos) {
      Vector3 worldPos = this.mainCamera.ScreenToWorldPoint(clickPos);
      Position pos = new(
        (int)((worldPos.x / GridMultiple) + 0.5f),
        (int)((worldPos.y / GridMultiple) + 0.5f)
      );

      IReadOnlyCollection<IEntity> entities = this.gameState.GetCell(pos).Contents;
      if (entities.Count == 0) {
        this.gui.MessageBox.AddMessage("There's nothing there.");
        return;
      }

      if (this.targetRequest != null) {
        IEnumerable<IEntity> targets = this.targetRequest.Targets.Intersect(entities);
        if (!targets.Any()) {
          this.gui.MessageBox.AddMessage("There are no legal targets there.");
          return;
        }

        this.OnGameEvent(
          new TargetEvent(
            TargetEvent.TargetEventMethod.Response,
            new List<IEntity>() { targets.First() }
          )
        );
        this.targetRequest = null;
      } else {
        foreach (IEntity entity in entities) {
          string description = entity.Description;
          this.gui.MessageBox.AddMessage(description);
        }
      }
    }

    private void Move(int mx, int my) {
      Position targetPosition = this.gameState.LocateEntityOnMap(this.mainCharacter) + (mx, my);
      List<IEntity> fighters = new();
      List<IEntity> blockers = new();
      IReadOnlyCollection<IEntity> entities = this.gameState.GetCell(targetPosition).Contents;
      foreach (IEntity entity in entities) {
        if (entity.CanFight) {
          fighters.Add(entity);
        } else if (entity.BlocksMove) {
          blockers.Add(entity);
        } else {
          // this entity can be stepped on or into
        }
      }

      if (fighters.Any()) {
        this.OnGameEvent(new AttackAction(this.mainCharacter, fighters.First()));
      } else if (blockers.Any()) {
        Debug.Log("Bonk!");
      } else {
        this.OnGameEvent(new MoveAction(this.mainCharacter, (mx, my)));
      }
    }

    private void DebugKey() {
      this.OnGameEvent(new DebugAction(DebugAction.DebugMethod.Damage));
    }

    private void ToggleFieldOfView() {
      this.usingFOV = !this.usingFOV;
      Debug.Log(this.usingFOV ? "Debug: FOV on" : "Debug: FOV off");
      foreach (Pawn t in this.pawns) {
        t.gameObject.SetActive(t.IsVisible || !this.usingFOV);
      }
    }
  }
}
