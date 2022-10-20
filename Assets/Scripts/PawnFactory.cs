using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PawnFactory : ScriptableObject {
    [Serializable]
    private struct NamedSprite {
        public string name;
        public Sprite sprite;
    }

// todo implement pooling behavior
    [SerializeField] Pawn prefab;
    [SerializeField] private List<NamedSprite> sprites;
    private Dictionary<string, Sprite> spriteMap;

    private void OnEnable() {
        // rebuild the spriteMap;
        spriteMap = new Dictionary<string, Sprite>();
        foreach (var namedSprite in sprites) {
            spriteMap[namedSprite.name] = namedSprite.sprite;
        }
        if (spriteMap.Count != sprites.Count) {
            Debug.LogError("Name collision in sprite map. Check the PawnFactory.");
        }
    }

    public Pawn Get(string appearance) {
        Pawn pawn = Instantiate(prefab);
        if (!spriteMap.ContainsKey(appearance)) {
            Debug.LogError($"Sprite map does not contain '{appearance}'. Check the PawnFactory.");
            return pawn;
        }
        pawn.SetSprite(spriteMap[appearance]);
        return pawn;
    }

    public void Reclaim (Pawn pawn) {
        Destroy(pawn.gameObject);
    }
}