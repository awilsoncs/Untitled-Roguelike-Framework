using UnityEngine;

// Viewable gameObject of the entity
public class Pawn : MonoBehaviour {
    public int EntityId { get; set; }
    private SpriteRenderer spriteRenderer;

    private void OnEnable() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Recycle(PawnFactory pawnFactory) {
        pawnFactory.Reclaim(this);
    }

    public void SetSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }
}