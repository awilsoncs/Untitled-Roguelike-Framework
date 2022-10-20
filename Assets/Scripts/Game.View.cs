using System.Collections.Generic;
using UnityEngine;

public partial class Game : PersistableObject, IGameClient {
    private List<Pawn> pawns;
    private Dictionary<int, Pawn> pawns_by_id;
    [SerializeField] public PawnFactory pawnFactory;
    const float GRID_MULTIPLE = 0.5f;
    private void UpdateView() {}

    public void EntityMoved(int entityId, int x, int y) {
        Debug.Log("You moved!");
    }
    public void EntityCreated(int id, string appearance, int x, int y) {
        Pawn pawn = pawnFactory.Get(appearance);
        pawn.EntityId = id;
        pawn.transform.position = new Vector3(x*GRID_MULTIPLE, y*GRID_MULTIPLE, 0f);
        pawns.Add(pawn);
        pawn.gameObject.name = $"Pawn::{id} {appearance}";
        pawns_by_id[id] = pawn;
    }
    public void EntityKilled(int id) {
        // Need to remove the pawn...
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
    }
    public void LogMessage(System.String message) {
        Debug.Log(message);
    }

    public void ClearView() {
        foreach (var pawn in pawns) {
            pawn.Recycle(pawnFactory);
        }
        pawns.Clear();
        pawns_by_id.Clear();
    }
}