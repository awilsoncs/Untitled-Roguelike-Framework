namespace URF.Client {
  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using URF.Client.GUI;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.RulesSystems;

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

    [SerializeField] private KeyCode mapKey = KeyCode.M;

    private const float GridMultiple = 0.5f;

    private readonly List<Pawn> pawns = new();

    private readonly Dictionary<int, Pawn> pawnsByID = new();

    private IEntity mainCharacter;

    private (int, int) mainCharacterPosition;

    private readonly Queue<IGameEvent> gameEvents = new();

    // track attackable enemies so that we can attack instead of attempt to move
    private readonly Dictionary<IEntity, (int, int)> entityPosition = new();

    // todo add a convenience type here to simplify initialization
    private List<IEntity>[][] entitiesByPosition;

    private bool usingFOV = true;

    // Unity message events appear unused to simple IDEs.
    private void Start() {
      this.gameEventChannel.Connect(this);
      this.BeginNewGame();
    }

    private void Update() {
      if (this.gameEvents.Count > 0) {
        Debug.Log("Beginning client update...");
      }
      while (this.gameEvents.Count > 0) {
        IGameEvent ev = this.gameEvents.Dequeue();
        base.HandleEvent(this, ev);
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
      this.entityPosition.Clear();
      this.entitiesByPosition = null;
    }

    private void ConfigureClientMap(Position mapSize) {
      this.entitiesByPosition = new List<IEntity>[mapSize.X][];
      for (int x = 0; x < mapSize.X; x++) {
        this.entitiesByPosition[x] = new List<IEntity>[mapSize.Y];
        for (int y = 0; y < mapSize.Y; y++) {
          this.entitiesByPosition[x][y] = new List<IEntity>();
        }
      }
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
      if (this.entityPosition.ContainsKey(entity)) {
        (int x0, int y0) = this.entityPosition[entity];
        _ = this.entitiesByPosition[x0][y0].Remove(entity);
      }
      this.entityPosition[entity] = (x, y);
      this.entitiesByPosition[x][y].Add(entity);

      if (entity == this.mainCharacter) {
        this.mainCharacterPosition = (x, y);
      }
    }

    private void CreatePawn(IEntity entity, Position position) {
      int id = entity.ID;
      EntityInfo info = entity.GetComponent<EntityInfo>();
      string appearance = info.Appearance;
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

    public override void HandleEntityDeleted(EntityDeleted ev) {
      if (ev.Entity == this.mainCharacter) {
        Debug.Log("Player died, reloading...");
        this.ClearGame();
        this.BeginNewGame();
      }

      if (this.pawnsByID.ContainsKey(ev.Entity.ID)) {
        // If the server forgot to send a EntityLocationChanged first.
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

      (int x, int y) = this.entityPosition[entity];
      _ = this.entityPosition.Remove(entity);
      _ = this.entitiesByPosition[x][y].Remove(entity);
    }

    public override void HandleEntityVisibilityChanged(EntityVisibilityChanged ev) {
      int id = ev.Entity.ID;
      bool newVis = ev.NewVisibility;
      Debug.Log($"Testing: {ev}");
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
      this.mainCharacterPosition = this.entityPosition[this.mainCharacter];
      CombatComponent stats = this.mainCharacter.GetComponent<CombatComponent>();
      this.gui.HealthBar.CurrentHealth = stats.CurrentHealth;
      this.gui.HealthBar.MaximumHealth = stats.MaxHealth;
      // todo should link updates to properties
      this.gui.HealthBar.UpdateHealthBar();
    }

    public override void HandleEntityAttacked(EntityAttacked ev) {
      EntityInfo attackerInfo = ev.Attacker.GetComponent<EntityInfo>();
      EntityInfo defenderInfo = ev.Defender.GetComponent<EntityInfo>();

      string attackerName = attackerInfo.Name;
      string defenderName = defenderInfo.Name;

      this.gui.MessageBox.AddMessage($"{attackerName} attacked {defenderName} for {ev.Damage} damage!");
      if (ev.Defender != this.mainCharacter || !ev.Success) {
        return;
      }
      this.gui.HealthBar.CurrentHealth -= ev.Damage;
      this.gui.HealthBar.UpdateHealthBar();
    }

    public override void HandleGameConfigured(GameConfigured ev) {
      this.ConfigureClientMap(ev.MapSize);
      this.mainCamera.transform.position = new Vector3(
        ev.MapSize.X / (2 / GridMultiple),
        ev.MapSize.Y / (2 / GridMultiple),
        -10
      );
    }

    public override void HandleInventoryChanged(InventoryChanged inventoryChanged) {
      switch (inventoryChanged.Action) {
        case InventoryChanged.InventoryAction.PickedUp:
          string agent = "";
          if (inventoryChanged.Entity == this.mainCharacter) {
            agent = "You";
          } else {
            agent = inventoryChanged.Entity.GetComponent<EntityInfo>().Name;
          }
          string targetName = inventoryChanged.Item.GetComponent<EntityInfo>().Name;
          this.gui.MessageBox.AddMessage($"{agent} got a {targetName}.");
          break;
      }
    }

    private void HandleUserInput() {
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
        this.SpawnCrab();
      } else if (Input.GetKeyDown(this.mapKey)) {
        this.ToggleFieldOfView();
      } else if (Input.GetKeyDown(this.getKey)) {
        this.GetItem();
      } else if (Input.GetKeyDown(this.saveKey)) {
        this.OnGameEvent(new SaveAction());
      } else if (Input.GetKeyDown(this.loadKey)) {
        this.ResetEverything();
        this.OnGameEvent(new LoadAction());
      } else if (Input.GetKeyDown(this.newGameKey)) {
        this.BeginNewGame();
      } else {
        // The player didn't send any known input.
      }
    }

    private void GetItem() {
      int x = this.mainCharacterPosition.Item1;
      int y = this.mainCharacterPosition.Item2;
      List<IEntity> entities = this.entitiesByPosition[x][y];
      IEntity itemToGet = entities.FirstOrDefault(x => x != this.mainCharacter);
      if (itemToGet != null) {
        this.OnGameEvent(new GetAction(this.mainCharacter, itemToGet));
      } else {
        this.gui.MessageBox.AddMessage("There's nothing here.");
      }
    }

    private void MouseClicked(Vector3 clickPos) {
      Vector3 worldPos = this.mainCamera.ScreenToWorldPoint(clickPos);
      Position pos = new(
        (int)((worldPos.x / GridMultiple) + 0.5f),
        (int)((worldPos.y / GridMultiple) + 0.5f)
      );
      if (this.entitiesByPosition[pos.X][pos.Y].Count > 0) {
        List<IEntity> entities = this.entitiesByPosition[pos.X][pos.Y];
        foreach (IEntity entity in entities) {
          EntityInfo entityInfo = entity.GetComponent<EntityInfo>();
          string description = entityInfo.Description;
          this.gui.MessageBox.AddMessage(description);
        }
      } else {
        this.gui.MessageBox.AddMessage("There's nothing there.");
      }
    }

    private void Move(int mx, int my) {
      int x = this.mainCharacterPosition.Item1 + mx;
      int y = this.mainCharacterPosition.Item2 + my;
      List<IEntity> fighters = new();
      List<IEntity> blockers = new();
      List<IEntity> entities = this.entitiesByPosition[x][y];
      foreach (IEntity entity in entities) {
        if (entity.GetComponent<CombatComponent>().CanFight) {
          fighters.Add(entity);
        } else if (entity.GetComponent<Movement>().BlocksMove) {
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

    private void SpawnCrab() {
      this.OnGameEvent(DebugAction.SpawnCrab());
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
