using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Configuración de Comida")]
    public GameObject foodPrefab;
    public int totalFoodCount = 12;

    [Header("Límites de Dispersión")]
    public Vector2 minBounds = new Vector2(-15f, -8f);
    public Vector2 maxBounds = new Vector2(15f, 8f);

    [Header("Detección de Paredes")]
    public LayerMask obstacleMask; // Capa 'Walls' o 'Default'
    public float checkRadius = 0.4f;

    private void Start()
    {
        SpawnFoodInMap();
    }

    public void SpawnFoodInMap()
    {
        if (foodPrefab == null) return;

        int spawned = 0;
        int maxAttempts = totalFoodCount * 25; // Evita bucles infinitos
        int attempts = 0;

        while (spawned < totalFoodCount && attempts < maxAttempts)
        {
            attempts++;

            // Genera una coordenada aleatoria en el rango configurado
            float randomX = Random.Range(minBounds.x + 0.5f, maxBounds.x - 0.5f);
            float randomY = Random.Range(minBounds.y + 0.5f, maxBounds.y - 0.5f);
            Vector2 candidatePosition = new Vector2(randomX, randomY);

            // Valida que NO haya una pared en el punto elegido
            if (!Physics2D.OverlapCircle(candidatePosition, checkRadius, obstacleMask))
            {
                Instantiate(foodPrefab, candidatePosition, Quaternion.identity, transform);
                spawned++;
            }
        }
    }
}