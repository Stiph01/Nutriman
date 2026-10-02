using UnityEngine;
using UnityEngine.Rendering;

public class Aguaymanto : MonoBehaviour
{
    public float speed = 2f;
    public float changeDirectionInterval = 2f;
    public float dropInterval = 15f;
    public GameObject quinoaSeedPrefab;
    public LayerMask wallLayer;

    private Vector2 currentDirection;
    private float directionTimer;
    private float dropTimer;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChooseRandomDirection();
    }

    private void Update()
    {
        // Detectar si hay una pared cerca en la dirección actual
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, 0.6f, wallLayer);

        if (hit.collider != null)
        {
            ChooseRandomDirection();
        }

        // Cambiar de dirección por tiempo
        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionInterval)
        {
            ChooseRandomDirection();
            directionTimer = 0f;
        }

        // Dejar caer la Semilla de Quinua
        dropTimer += Time.deltaTime;
        if (dropTimer >= dropInterval)
        {
            DropSeed();
            dropTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + currentDirection * speed * Time.fixedDeltaTime);
        }
    }

    private void ChooseRandomDirection()
    {
        int dir = Random.Range(0, 4);

        switch (dir)
        {
            case 0: currentDirection = Vector2.up; break;
            case 1: currentDirection = Vector2.down; break;
            case 2: currentDirection = Vector2.left; break;
            case 3: currentDirection = Vector2.right; break;
        }
    }

    private void DropSeed()
    {
        if (quinoaSeedPrefab != null)
        {
            Instantiate(quinoaSeedPrefab, transform.position, Quaternion.identity);
        }
    }
}