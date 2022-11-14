using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Server.RulesSystems;

namespace URF.Client {
  public partial class GameClient {

    public event EventHandler<IActionEventArgs> PlayerAction;

    protected virtual void OnPlayerAction(IActionEventArgs e) {
      PlayerAction?.Invoke(this, e);
    }

    [SerializeField] private KeyCode newGameKey = KeyCode.N;

    [SerializeField] private KeyCode saveKey = KeyCode.S;

    [SerializeField] private KeyCode loadKey = KeyCode.L;

    [SerializeField] private KeyCode upKey = KeyCode.UpArrow;

    [SerializeField] private KeyCode downKey = KeyCode.DownArrow;

    [SerializeField] private KeyCode leftKey = KeyCode.LeftArrow;

    [SerializeField] private KeyCode rightKey = KeyCode.RightArrow;

    [SerializeField] private KeyCode spawnKey = KeyCode.C;

    [SerializeField] private KeyCode mapKey = KeyCode.M;

    private bool _usingFOV = true;

    private void HandleUserInput() {
      if(Input.GetMouseButtonDown(0)) { MouseClicked(Input.mousePosition); }
      else if(Input.GetKeyDown(leftKey)) { Move(-1, 0); }
      else if(Input.GetKeyDown(rightKey)) { Move(1, 0); }
      else if(Input.GetKeyDown(upKey)) { Move(0, 1); }
      else if(Input.GetKeyDown(downKey)) { Move(0, -1); }
      else if(Input.GetKeyDown(spawnKey)) { SpawnCrab(); }
      else if(Input.GetKeyDown(mapKey)) { ToggleFieldOfView(); }
      else if(Input.GetKeyDown(saveKey)) { OnPlayerAction(new SaveActionEventArgs()); }
      else if(Input.GetKeyDown(loadKey)) { OnPlayerAction(new LoadActionEventArgs()); }
      else if(Input.GetKeyDown(newGameKey)) { BeginNewGame();}
    }

    private void MouseClicked(Vector3 clickPos) {
      Vector3 worldPos = mainCamera.ScreenToWorldPoint(clickPos);
      Position pos = new((int)(worldPos.x / gridMultiple + 0.5f),
        (int)(worldPos.y / gridMultiple + 0.5f));
      if(_entitiesByPosition[pos.X][pos.Y].Count > 0) {
        List<IEntity> entities = _entitiesByPosition[pos.X][pos.Y];
        foreach(IEntity entity in entities) {
          EntityInfo entityInfo = entity.GetComponent<EntityInfo>();
          string description = entityInfo.Description;
          gui.MessageBox.AddMessage(description);
        }
      }
      else { gui.MessageBox.AddMessage("There's nothing there."); }
    }

    private void Move(int mx, int my) {
      int x = _mainCharacterPosition.Item1 + mx;
      int y = _mainCharacterPosition.Item2 + my;
      List<IEntity> fighters = new();
      List<IEntity> blockers = new();
      List<IEntity> entities = _entitiesByPosition[x][y];
      foreach(IEntity entity in entities) {
        if(entity.GetComponent<CombatComponent>().CanFight) { fighters.Add(entity); }
        else if(entity.GetComponent<Movement>().BlocksMove) { blockers.Add(entity); }
      }

      if(fighters.Any()) {
        OnPlayerAction(new AttackActionEventArgs(_mainCharacterId, fighters.First().ID));
      }
      else if(blockers.Any()) { Debug.Log("Bonk!"); }
      else { OnPlayerAction(new MoveActionEventArgs(_mainCharacterId, (mx, my))); }
    }

    private void SpawnCrab() {
      OnPlayerAction(DebugActionEventArgs.SpawnCrab());
    }

    private void ToggleFieldOfView() {
      _usingFOV = !_usingFOV;
      Debug.Log(_usingFOV ? "Debug: FOV on" : "Debug: FOV off");
      foreach(Pawn t in _pawns) { t.gameObject.SetActive(t.IsVisible || !_usingFOV); }
    }

  }
}
