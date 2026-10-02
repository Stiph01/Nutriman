using UnityEngine;

public class HunterEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        spawnPoint = new Vector2(0f, 4f);
    }

    // Hereda persecución directa, evitación de paredes, respawn y buffs de Enemy.cs
}