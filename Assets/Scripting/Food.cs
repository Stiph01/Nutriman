using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Food : MonoBehaviour
{
    [Header("Puntuación")]
    public int points = 10;

    [Header("Movimiento en Laberinto")]
    public float speed = 2.0f;
    public float changeDirectionInterval = 3f;
    public float raycastDistance = 0.6f;
    public LayerMask obstacleMask;

    private Rigidbody2D rb;
    private Vector2 currentDirection;
    private float timer;

    private static readonly Vector2[] Directions = {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChooseValidDirection();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        timer -= Time.fixedDeltaTime;

        // Si se acaba el tiempo o detecta pared al frente, busca un nuevo pasillo libre
        if (timer <= 0f || IsWallAhead())
        {
            ChooseValidDirection();
        }

        rb.linearVelocity = currentDirection * speed;
    }

    // Detecta si la dirección actual está bloqueada por un muro
    private bool IsWallAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, currentDirection, raycastDistance, obstacleMask);
        return hit.collider != null && !hit.collider.CompareTag("Player");
    }

    // Filtra las direcciones y elige una que esté libre de paredes
    private void ChooseValidDirection()
    {
        timer = changeDirectionInterval;
        List<Vector2> validDirections = new List<Vector2>();

        foreach (Vector2 dir in Directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, dir, raycastDistance, obstacleMask);
            if (hit.collider == null || hit.collider.CompareTag("Player"))
            {
                validDirections.Add(dir);
            }
        }

        if (validDirections.Count > 0)
        {
            currentDirection = validDirections[Random.Range(0, validDirections.Count)];
        }
        else
        {
            // Si está atrapado, da la vuelta
            currentDirection = -currentDirection;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObject = collision.gameObject;

        if (hitObject.CompareTag("Player") || hitObject.GetComponentInParent<PlayerStats>() != null)
        {
            ProcessImpact(hitObject);
            return;
        }

        // Si choca contra una pared, busca un camino libre inmediatamente
        ChooseValidDirection();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcessImpact(other.gameObject);
    }

    private void ProcessImpact(GameObject target)
    {
        PlayerStats stats = target.GetComponentInParent<PlayerStats>();
        if (stats == null) stats = target.GetComponent<PlayerStats>();

        if (target.CompareTag("Player") || stats != null)
        {
            if (stats != null)
            {
                stats.AddScore(points);
            }

            if (AudioManager.Instance != null && AudioManager.Instance.healthyFoodSound != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.healthyFoodSound);
            }

            Destroy(gameObject);
        }
    }
}