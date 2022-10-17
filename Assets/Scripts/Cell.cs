public class Cell {
    int x, y = 0;

    // this item takes up the majority of the space here.
    Entity contents;

    public Cell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetContents(Entity entity) {
        this.contents = entity;
    }

    public void ClearContents() {
        this.contents = null;
    }
}