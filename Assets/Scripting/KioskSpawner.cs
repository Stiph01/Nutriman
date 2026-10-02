using UnityEngine;

public class KioskSpawner : MonoBehaviour
{
    public GameObject junkFoodPrefab;
    public float spawnInterval = 3f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnJunkFood();
            timer = 0f;
        }
    }

    private void SpawnJunkFood()
    {
        if (junkFoodPrefab == null)
            return;

        GameObject junk = Instantiate(junkFoodPrefab, transform.position, Quaternion.identity);
        JunkFood junkScript = junk.GetComponent<JunkFood>();

        if (junkScript != null)
        {
            // Elegir una dirección aleatoria hacia abajo para variar el disparo
            int randomDir = Random.Range(0, 3);

            if (randomDir == 0)
                junkScript.direction = Vector2.down;
            else if (randomDir == 1)
                junkScript.direction = new Vector2(-0.5f, -1f).normalized;
            else
                junkScript.direction = new Vector2(0.5f, -1f).normalized;
        }
    }
}