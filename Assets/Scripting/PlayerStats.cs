using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int lives = 3;
    public int score = 0;

    public TMP_Text scoreText;
    public TMP_Text livesText;

    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
        UpdateUI();
    }

    public void AddPoints(int amount)
    {
        score += amount;

        if (score < 0)
        {
            score = 0;
        }

        UpdateUI();
    }
    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score;
        }
    }
    public void LoseLife()
    {
        if (isInvincible)
            return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            if (gameManager != null)
            {
                gameManager.OnGameOver();
            }
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private System.Collections.IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + score;

        if (livesText != null)
            livesText.text = "Vidas: " + lives;
    }
}