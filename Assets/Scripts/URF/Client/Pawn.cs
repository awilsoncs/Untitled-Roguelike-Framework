using UnityEngine;

// Viewable gameObject of the entity
namespace URF.Client {
  public class Pawn : MonoBehaviour {

    private SpriteRenderer _spriteRenderer;

    public int EntityId { get; set; }

    public bool IsVisible { get; set; }

    private void OnEnable() {
      _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Recycle(PawnFactory pawnFactory) {
      PawnFactory.Reclaim(this);
    }

    public void SetSprite(Sprite sprite) {
      _spriteRenderer.sprite = sprite;
    }

  }
}
