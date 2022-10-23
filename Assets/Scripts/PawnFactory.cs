using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provide an interface to get pawns.
/// </summary>
[CreateAssetMenu]
public class PawnFactory : ScriptableObject {
    // Hack a dictionary into the inspector. See OnEnable().
    [Serializable]
    private struct NamedSprite {
        public string name;
        public Sprite sprite;
    }

    // todo implement pooling behavior
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

    /// <summary>
    /// Get a pawn with the specified appearance, if possible.
    /// </summary>
    /// <param name="appearance">A descriptor of the sprite to find.</param>
    /// <returns>A Pawn with the specified appearance.</returns>
    public Pawn Get(string appearance) {
        GameObject pawnObject = new GameObject();
        pawnObject.AddComponent<SpriteRenderer>();
        Pawn pawn = pawnObject.AddComponent<Pawn>();
        pawn.IsVisible = true;

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