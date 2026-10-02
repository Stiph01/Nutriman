using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2.5f;
    public Vector2 spawnPoint = Vector2.zero;

    [Header("Detección de Obstáculos / Paredes")]
    public float obstacleCheckDistance = 0.6f;
    public LayerMask obstacleMask; // Configurar capa 'Walls' o 'Default' en el Inspector

    [Header("Efectos / JunkFood Buff")]
    public float scaleMultiplier = 1.3f;

    protected Transform player;
    protected Rigidbody2D playerRb;
    protected Rigidbody2D rb;

    private float originalSpeed;
    private Vector3 originalScale;
    private Coroutine buffCoroutine;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalSpeed = speed;
        originalScale = transform.localScale;

        // Búsqueda del jugador y almacenamiento en caché
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerRb = playerObj.GetComponent<Rigidbody2D>();
        }
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    // Algoritmo de movimiento base con evitación de paredes
    protected virtual void Move()
    {
        if (player == null || rb == null) return;

        Vector2 rawDirection = ((Vector2)player.position - rb.position).normalized;
        Vector2 adjustedDirection = GetAvoidanceDirection(rawDirection);

        rb.MovePosition(rb.position + adjustedDirection * speed * Time.fixedDeltaTime);
    }

    // Calcula una dirección alternativa si hay una pared al frente para evitar quedarse varado
    protected Vector2 GetAvoidanceDirection(Vector2 desiredDirection)
    {
        if (desiredDirection == Vector2.zero) return Vector2.zero;

        // Chequeo frontal
        RaycastHit2D hit = Physics2D.Raycast(rb.position, desiredDirection, obstacleCheckDistance, obstacleMask);

        if (hit.collider != null && !hit.collider.CompareTag("Player"))
        {
            // Intenta deslizarse en direcciones perpendiculares
            Vector2 dirA = new Vector2(-desiredDirection.y, desiredDirection.x);
            Vector2 dirB = new Vector2(desiredDirection.y, -desiredDirection.x);

            bool blockedA = Physics2D.Raycast(rb.position, dirA, obstacleCheckDistance, obstacleMask);
            bool blockedB = Physics2D.Raycast(rb.position, dirB, obstacleCheckDistance, obstacleMask);

            if (!blockedA) return dirA;
            if (!blockedB) return dirB;
        }

        return desiredDirection;
    }

    // Potenciador unificado (Soporta llamadas de JunkFood por velocidad y escala)
    public void ApplyJunkFoodBuff(float speedMult, float scaleMult, float duration)
    {
        if (buffCoroutine != null)
        {
            StopCoroutine(buffCoroutine);
        }
        buffCoroutine = StartCoroutine(JunkFoodBuffRoutine(speedMult, scaleMult, duration));
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        ApplyJunkFoodBuff(multiplier, scaleMultiplier, duration);
    }

    private IEnumerator JunkFoodBuffRoutine(float speedMult, float scaleMult, float duration)
    {
        speed = originalSpeed * speedMult;
        transform.localScale = originalScale * scaleMult;

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
        transform.localScale = originalScale;
        buffCoroutine = null;
    }

    // Colisión centralizada contra el jugador
    [System.Obsolete]
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats stats = collision.gameObject.GetComponentInParent<PlayerStats>();
            if (stats == null) stats = collision.gameObject.GetComponent<PlayerStats>();

            if (stats != null)
            {
                stats.LoseLife();
                
                // Reproduce el sonido de pérdida de vida
                if(AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.loseLifeSound);
                }
            }

            // Regresa a su punto de reaparición asignado
            rb.position = spawnPoint;
            rb.velocity = Vector2.zero;
        }
    }
}