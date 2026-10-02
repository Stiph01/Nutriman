using UnityEngine;

public class StrategistEnemy : Enemy
{
    [Header("Estrategia")]
    public int cellsAhead = 2;

    protected override void Start()
    {
        base.Start();
        spawnPoint = new Vector2(4f, 4f);
    }

    protected override void Move()
    {
        if (player == null || rb == null) return;

        Vector2 playerDirection = Vector2.zero;

        if (playerRb != null)
        {
            playerDirection = playerRb.linearVelocity.normalized;
        }

        Vector2 targetPosition = (Vector2)player.position + playerDirection * cellsAhead;
        Vector2 rawDirection = (targetPosition - rb.position).normalized;

        // Aplica el filtro de evitación de paredes
        Vector2 adjustedDirection = GetAvoidanceDirection(rawDirection);

        rb.MovePosition(rb.position + adjustedDirection * speed * Time.fixedDeltaTime);
    }
}