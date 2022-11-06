using UnityEngine;
using URFCommon;

namespace URFFrontend {
    public partial class Game : IGameClient {
        [SerializeField] KeyCode newGameKey = KeyCode.N;
        [SerializeField] KeyCode saveKey = KeyCode.S;
        [SerializeField] KeyCode loadKey = KeyCode.L;
        [SerializeField] KeyCode upKey = KeyCode.UpArrow;
        [SerializeField] KeyCode downKey = KeyCode.DownArrow;
        [SerializeField] KeyCode leftKey = KeyCode.LeftArrow;
        [SerializeField] KeyCode rightKey = KeyCode.RightArrow;
        [SerializeField] KeyCode spawnKey = KeyCode.C;
        [SerializeField] KeyCode mapKey = KeyCode.M;
        private bool usingFOV = true;
        private void HandleUserInput() {
            if (Input.GetKeyDown(leftKey)) Move(-1, 0);
            else if (Input.GetKeyDown(rightKey)) Move(1, 0);
            else if (Input.GetKeyDown(upKey)) Move(0, 1);
            else if (Input.GetKeyDown(downKey)) Move(0, -1);
            else if (Input.GetKeyDown(spawnKey)) SpawnCrab();
            else if (Input.GetKeyDown(mapKey)) ToggleFieldOfView();
            else if (Input.GetKeyDown(saveKey)) {
                Debug.Log("Player asked for save...");
                storage.Save(this, saveVersion);
            }
            else if (Input.GetKeyDown(loadKey)) {
                Debug.Log("Player asked for load...");
                storage.Load(this);
            }
            else if (Input.GetKeyDown(newGameKey)) {
                Debug.Log("Player asked for a reload...");
                ClearGame();
                BeginNewGame();
            }
        }

        private void Move(int mx, int my) {
            int x = mainCharacterPosition.Item1 + mx;
            int y = mainCharacterPosition.Item2 + my;
            if (entityMap[x][y].Item1 >= 0 && entityMap[x][y].Item2) {
                // this is an enemy, bump attack instead
                gameState.PostEvent(
                    new AttackCommand(mainCharacterId, entityMap[x][y].Item1)
                );
            } else if (entityMap[x][y].Item1 >= 0) {
                // there's something here...
                Debug.Log("Bonk!");
            } else {
                gameState.PostEvent(new MoveCommand(mainCharacterId, (mx, my)));
            }
        }

        private void SpawnCrab() {
            gameState.PostEvent(DebugCommand.SpawnCrab());
        }

        private void ToggleFieldOfView() {
            usingFOV = !usingFOV;
            if (usingFOV) {
                Debug.Log("Debug: FOV on");
            } else {
                Debug.Log("Debug: FOV off");
            }
            for (int i = 0; i < pawns.Count; i++) {
                pawns[i].gameObject.SetActive(pawns[i].IsVisible || !usingFOV);
            }
        }
    }
}