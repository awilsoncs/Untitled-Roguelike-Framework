using System;
public class FighterPart : EntityPart {
    int maxHealth;

    public int MaxHealth => maxHealth;
    int currentHealth;
    public int CurrentHealth => currentHealth;
    int damage;

    public override EntityPartType PartType => EntityPartType.Fighter;
    public override void GameUpdate() {
        if (currentHealth <= 0) {
            GameState.Kill(Entity);
        }
    }

    public override void Recycle() {
        EntityPartPool<FighterPart>.Reclaim(this);
    }

    public void Attack(FighterPart other) {
        GameState.PostEvent(new EntityAttackedEvent(Entity.ID, other.Entity.ID, true, damage));
        GameState.Log($"{Entity} will deal {damage} damage.");
        other.DealDamage(damage);
    }

    public void DealDamage(int damage) {
        GameState.Log($"{Entity} took {damage} damage.");
        currentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));
    }

    public FighterPart SetMaxHealth(int maxHealth) {
        if (maxHealth > currentHealth) {
            this.currentHealth = maxHealth;
        }
        this.maxHealth = maxHealth;
        return this;
    }

    public FighterPart SetCurrentHealth(int currentHealth) {
        if (currentHealth > maxHealth) {
            this.maxHealth = currentHealth;
        }
        this.currentHealth = currentHealth;
        return this;
    }

    public FighterPart SetDamage(int damage) {
        this.damage = Math.Max(damage, 0);
        return this;
    }

    public override void Load(GameDataReader reader) {
        base.Load(reader);
        maxHealth = reader.ReadInt();
        currentHealth = reader.ReadInt();
        damage = reader.ReadInt();
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(maxHealth);
        writer.Write(currentHealth);
        writer.Write(damage);
    }
}