using UnityEngine;

// Hereda toda la funcionalidad optimizada de la clase base Enemy
public class EnemyAI : Enemy
{
    protected override void Start()
    {
        base.Start();
        if (spawnPoint == Vector2.zero)
        {
            spawnPoint = new Vector2(0f, 3f);
        }
    }
}