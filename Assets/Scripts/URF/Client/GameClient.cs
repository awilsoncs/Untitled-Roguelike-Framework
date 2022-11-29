namespace URF.Client {
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

    [SerializeField] private KeyCode mapKey = KeyCode.M;

    private const float GridMultiple = 0.5f;

    private readonly List<Pawn> pawns = new();

    private readonly Dictionary<int, Pawn> pawnsByID = new();

    private int mainCharacterId;

    private (int, int) mainCharacterPosition;

    private readonly Queue<IGameEvent> gameEvents = new();

    // track attackable enemies so that we can attack instead of attempt to move
    private readonly Dictionary<int, (int, int)> entityPosition = new();

    // todo add a convenience type here to simplify initialization
    private List<IEntity>[][] entitiesByPosition;

    private bool usingFOV = true;

    // Unity message events appear unused to simple IDEs.
    private void Start() {
      this.gameEventChannel.Connect(this);
      this.BeginNewGame();
    }

    private void Update() {
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

    public override void HandleEntityMoved(EntityMoved ev) {
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

    public override void HandleEntityCreated(EntityCreated ev) {
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

    public override void HandleEntityKilled(EntityKilled ev) {
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
      IEntity mainCharacter = ev.Entity;
      this.mainCharacterId = ev.Entity.ID;
      this.mainCharacterPosition = this.entityPosition[this.mainCharacterId];
      CombatComponent stats = mainCharacter.GetComponent<CombatComponent>();
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
      if (ev.Defender.ID != this.mainCharacterId || !ev.Success) {
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
        this.OnGameEvent(new AttackAction(this.mainCharacterId, fighters.First().ID));
      } else if (blockers.Any()) {
        Debug.Log("Bonk!");
      } else {
        this.OnGameEvent(new MoveAction(this.mainCharacterId, (mx, my)));
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
