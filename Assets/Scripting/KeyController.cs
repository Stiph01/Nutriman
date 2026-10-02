using UnityEngine;

public class KeyController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            QuestionManager questionManager = Object.FindFirstObjectByType<QuestionManager>();

            if (questionManager != null && stats != null)
            {
                questionManager.OpenTrivia(stats.score);
            }

            Destroy(gameObject);
        }
    }
}