using System;
public class HealthPart : EntityPart {
    int maxHealth = 0;
    int currentHealth = 0;

    public override EntityPartType PartType => EntityPartType.Health;
    public override void GameUpdate(IGameState gs) {
        if (currentHealth <= 0) {
            gs.Kill(Entity);
            // todo allow parts to send events somehow
        }
    }

    public override void Recycle() {
        EntityPartPool<HealthPart>.Reclaim(this);
    }

    public void DealDamage(int damage) {
        currentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));
    }

    public HealthPart SetMaxHealth(int maxHealth) {
        if (maxHealth > currentHealth) {
            this.currentHealth = maxHealth;
        }
        this.maxHealth = maxHealth;
        return this;
    }

    public HealthPart SetCurrentHealth(int currentHealth) {
        if (currentHealth > maxHealth) {
            this.maxHealth = currentHealth;
        }
        this.currentHealth = currentHealth;
        return this;
    }

    public override void Load(GameDataReader reader) {
        base.Load(reader);
        maxHealth = reader.ReadInt();
        currentHealth = reader.ReadInt();
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(maxHealth);
        writer.Write(currentHealth);
    }
}