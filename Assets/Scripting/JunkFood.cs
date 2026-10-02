using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class JunkFood : MonoBehaviour
{
    [Header("Efectos")]
    public int penaltyPoints = 15;
    public float speedMultiplier = 1.5f;
    public float boostDuration = 4f;

    [Header("Movimiento Físico")]
    public float speed = 3f;
    public Vector2 direction = new Vector2(1f, -1f);

    [Header("Tiempo de Vida")]
    public float lifeTime = 20f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessImpact(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcessImpact(other.gameObject);
    }

    private void ProcessImpact(GameObject target)
    {
        // 1. Impacto con el Jugador (Nutriman)
        PlayerStats stats = target.GetComponentInParent<PlayerStats>();
        if (stats == null) stats = target.GetComponent<PlayerStats>();

        if (target.CompareTag("Player") || stats != null)
        {
            if (stats != null)
            {
                // Resta los puntos en la interfaz
                stats.AddScore(-penaltyPoints);
            }

            // Reproduce el sonido a través del AudioManager
            if (AudioManager.Instance != null && AudioManager.Instance.junkFoodSound != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.junkFoodSound);
            }

            Destroy(gameObject);
            return;
        }

        // 2. Impacto con los Monstruos (Compatible con cualquier subclase de Enemy)
        Enemy enemy = target.GetComponentInParent<Enemy>();
        if (enemy == null) enemy = target.GetComponent<Enemy>();

        if (target.CompareTag("Enemy") || enemy != null)
        {
            if (enemy != null)
            {
                enemy.ApplyJunkFoodBuff(speedMultiplier, 1.3f, boostDuration);
            }

            Destroy(gameObject);
        }
    }
}