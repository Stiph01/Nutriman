using UnityEngine;

public class RandomEnemy : Enemy
{
    [Header("Movimiento Aleatorio")]
    public float changeDirectionInterval = 1.5f;

    private static readonly Vector2[] Directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private Vector2 currentDirection;
    private float timer;

    protected override void Start()
    {
        base.Start();
        spawnPoint = new Vector2(-4f, 4f);
        ChooseRandomDirection();
    }

    protected override void Move()
    {
        if (rb == null) return;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            ChooseRandomDirection();
        }

        rb.MovePosition(rb.position + currentDirection * speed * Time.fixedDeltaTime);
    }

    private void ChooseRandomDirection()
    {
        currentDirection = Directions[Random.Range(0, Directions.Length)];
        timer = changeDirectionInterval;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // Si choca contra una pared u objeto sólido, cambia de dirección inmediatamente
        if (!collision.gameObject.CompareTag("Player"))
        {
            ChooseRandomDirection();
        }
    }
}