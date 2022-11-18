namespace URF.Client {
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using URF.Client.GUI;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server;
  using URF.Server.RulesSystems;

  /// <summary>
  /// GameClient client view
  /// </summary>
  [DisallowMultipleComponent]
  public class GameClient : MonoBehaviour, IPlayerActionChannel {

    public event EventHandler<IActionEventArgs> PlayerAction;

    [SerializeField] private GuiComponents gui;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private BaseGameEventChannel gameEventChannel;

    [SerializeField] private PawnFactory pawnFactory;

    [SerializeField] private KeyCode newGameKey = KeyCode.N;

    [SerializeField] private KeyCode saveKey = KeyCode.S;

    [SerializeField] private KeyCode loadKey = KeyCode.L;

    [SerializeField] private KeyCode upKey = KeyCode.UpArrow;

    [SerializeField] private KeyCode downKey = KeyCode.DownArrow;

    [SerializeField] private KeyCode leftKey = KeyCode.LeftArrow;

    [SerializeField] private KeyCode rightKey = KeyCode.RightArrow;

    [SerializeField] private KeyCode spawnKey = KeyCode.C;

    [SerializeField] private KeyCode mapKey = KeyCode.M;

    private const float GridMultiple = 0.5f;

    private readonly List<Pawn> pawns = new();

    private readonly Dictionary<int, Pawn> pawnsByID = new();

    private int mainCharacterId;

    private (int, int) mainCharacterPosition;

    private readonly Queue<IGameEventArgs> gameEvents = new();

    // track attackable enemies so that we can attack instead of attempt to move
    private readonly Dictionary<int, (int, int)> entityPosition = new();

    // todo add a convenience type here to simplify initialization
    private List<IEntity>[][] entitiesByPosition;

    private bool usingFOV = true;

    // Unity message events appear unused to simple IDEs.
    private void Start() {
      this.gameEventChannel.GameEvent += this.EnqueueGameEvent;
      this.gameEventChannel.Connect(this);
      this.BeginNewGame();
    }

    private void Update() {
      while (this.gameEvents.Count > 0) {
        this.HandleGameEvent(this.gameEvents.Dequeue());
      }
      this.HandleUserInput();
    }

    private void EnqueueGameEvent(object sender, IGameEventArgs e) => this.gameEvents.Enqueue(e);

    private void BeginNewGame() {
      this.ResetEverything();
      this.OnPlayerAction(new ConfigureActionArgs());
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


    private void HandleGameEvent(IGameEventArgs ev) {
      switch (ev.EventType) {
        case GameEventType.EntityMoved:
          this.HandleEntityMoved((EntityMovedEventArgs)ev);
          return;
        case GameEventType.EntityCreated:
          this.HandleEntityCreated((EntityCreatedEventArgs)ev);
          return;
        case GameEventType.EntityAttacked:
          this.HandleEntityAttacked((EntityAttackedEventArgs)ev);
          return;
        case GameEventType.EntityKilled:
          this.HandleEntityKilled((EntityKilledEventArgs)ev);
          return;
        case GameEventType.EntityVisibilityChanged:
          this.HandleEntityVisibilityChanged((EntityVisibilityChangedEventArgs)ev);
          return;
        case GameEventType.GameError:
          this.HandleGameErrorEvent((GameErroredEventArgs)ev);
          return;
        case GameEventType.MainCharacterChanged:
          this.HandleMainCharacterChangedEvent((MainCharacterChangedEventArgs)ev);
          return;
        case GameEventType.Configure:
          this.HandleGameConfiguredEvent((GameConfiguredEventArgs)ev);
          return;
        case GameEventType.AttackCommand:
          break;
        case GameEventType.MoveCommand:
          break;
        case GameEventType.DebugCommand:
          break;
        case GameEventType.Save:
          break;
        case GameEventType.Load:
          break;
        case GameEventType.SpentTurn:
          break;
        case GameEventType.Start:
          break;
        default:
          Debug.Log($"Unhandled GameEventType {ev.EventType}");
          return;
      }
    }

    private void HandleEntityMoved(EntityMovedEventArgs ev) {
      Pawn pawn = this.pawnsByID[ev.Entity.ID];
      int x = ev.Position.X;
      int y = ev.Position.Y;
      pawn.transform.position = new Vector3(x * GridMultiple, y * GridMultiple, 0f);
      if (this.entityPosition.ContainsKey(ev.Entity.ID)) {
        (int x0, int y0) = this.entityPosition[ev.Entity.ID];
        _ = this.entitiesByPosition[x0][y0].Remove(ev.Entity);
      }
      this.entityPosition[ev.Entity.ID] = (x, y);
      this.entitiesByPosition[x][y].Add(ev.Entity);

      if (ev.Entity.ID == this.mainCharacterId) {
        this.mainCharacterPosition = (x, y);
      }
    }

    private void HandleEntityCreated(EntityCreatedEventArgs ev) {
      IEntity entity = ev.Entity;
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
    }

    private void HandleEntityKilled(EntityKilledEventArgs ev) {
      EntityInfo info = ev.Entity.GetComponent<EntityInfo>();
      Debug.Log($"Entity {info.Name} has been killed.");
      int id = ev.Entity.ID;
      if (id == this.mainCharacterId) {
        Debug.Log("Player died, reloading...");
        this.ClearGame();
        this.BeginNewGame();
      }

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

      (int x, int y) = this.entityPosition[id];
      _ = this.entityPosition.Remove(id);
      _ = this.entitiesByPosition[x][y].Remove(ev.Entity);
    }

    private void HandleEntityVisibilityChanged(EntityVisibilityChangedEventArgs ev) {
      int id = ev.Entity.ID;
      bool newVis = ev.NewVisibility;
      this.pawnsByID[id].IsVisible = newVis;
      if (this.usingFOV || newVis) {
        this.pawnsByID[id].gameObject.SetActive(newVis);
      }
    }

    private void HandleGameErrorEvent(GameErroredEventArgs ev) {
      string message = ev.Message;
      Debug.LogError(message);
    }

    private void HandleMainCharacterChangedEvent(MainCharacterChangedEventArgs ev) {
      IEntity mainCharacter = ev.Entity;
      this.mainCharacterId = ev.Entity.ID;
      this.mainCharacterPosition = this.entityPosition[this.mainCharacterId];
      CombatComponent stats = mainCharacter.GetComponent<CombatComponent>();
      this.gui.HealthBar.CurrentHealth = stats.CurrentHealth;
      this.gui.HealthBar.MaximumHealth = stats.MaxHealth;
      // todo should link updates to properties
      this.gui.HealthBar.UpdateHealthBar();
    }

    private void HandleEntityAttacked(EntityAttackedEventArgs ev) {
      EntityInfo attackerInfo = ev.Attacker.GetComponent<EntityInfo>();
      EntityInfo defenderInfo = ev.Defender.GetComponent<EntityInfo>();

      string attackerName = attackerInfo.Name;
      string defenderName = defenderInfo.Name;

      this.gui.MessageBox.AddMessage($"{attackerName} attacked {defenderName} for {ev.Damage} damage!");
      if (ev.Defender.ID != this.mainCharacterId || !ev.Success) {
        return;
      }
      this.gui.HealthBar.CurrentHealth -= ev.Damage;
      this.gui.HealthBar.UpdateHealthBar();
    }

    private void HandleGameConfiguredEvent(GameConfiguredEventArgs ev) {
      this.ConfigureClientMap(ev.MapSize);
      this.mainCamera.transform.position = new Vector3(
        ev.MapSize.X / (2 / GridMultiple),
        ev.MapSize.Y / (2 / GridMultiple),
        -10
      );
    }

    protected virtual void OnPlayerAction(IActionEventArgs ev) => PlayerAction?.Invoke(this, ev);

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
      } else if (Input.GetKeyDown(this.saveKey)) {
        this.OnPlayerAction(new SaveActionEventArgs());
      } else if (Input.GetKeyDown(this.loadKey)) {
        this.ResetEverything();
        this.OnPlayerAction(new LoadActionEventArgs());
      } else if (Input.GetKeyDown(this.newGameKey)) {
        this.BeginNewGame();
      } else {
        // The player didn't send any known input.
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
        this.OnPlayerAction(new AttackActionEventArgs(this.mainCharacterId, fighters.First().ID));
      } else if (blockers.Any()) {
        Debug.Log("Bonk!");
      } else {
        this.OnPlayerAction(new MoveActionEventArgs(this.mainCharacterId, (mx, my)));
      }
    }

    private void SpawnCrab() => this.OnPlayerAction(DebugActionEventArgs.SpawnCrab());

    private void ToggleFieldOfView() {
      this.usingFOV = !this.usingFOV;
      Debug.Log(this.usingFOV ? "Debug: FOV on" : "Debug: FOV off");
      foreach (Pawn t in this.pawns) {
        t.gameObject.SetActive(t.IsVisible || !this.usingFOV);
      }
    }

  }
}
