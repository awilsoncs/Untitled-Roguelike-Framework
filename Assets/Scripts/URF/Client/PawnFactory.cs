using System;
using System.Collections.Generic;
using UnityEngine;

namespace URF.Client {
  /// <summary>
  /// Provide an interface to get pawns.
  /// </summary>
  [CreateAssetMenu]
  public class PawnFactory : ScriptableObject {

    // Hack a dictionary into the inspector. See OnEnable().
    [Serializable]
    private struct NamedSprite {

      [SerializeField] private string name;

      [SerializeField] private Sprite sprite;

      public readonly string Name => name;

      public readonly Sprite Sprite => sprite;

    }

    // todo implement pooling behavior
    [SerializeField] private List<NamedSprite> sprites;

    private readonly Dictionary<string, Sprite> _spriteMap = new();

    private void OnEnable() {
      _spriteMap.Clear();
      foreach(NamedSprite namedSprite in sprites) {
        _spriteMap[namedSprite.Name] = namedSprite.Sprite;
      }
      if(_spriteMap.Count != sprites.Count) {
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

      if(!_spriteMap.ContainsKey(appearance)) {
        Debug.LogError($"Sprite map does not contain '{appearance}'. Check the PawnFactory.");
        return pawn;
      }
      pawn.SetSprite(_spriteMap[appearance]);
      return pawn;
    }

    public static void Reclaim(Pawn pawn) {
      Destroy(pawn.gameObject);
    }

  }
}
