using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Configuración de Personajes")]
    public PlayerController player;
    public RuntimeAnimatorController[] characterAnimators; // Array para los 7 Animator Override Controllers

    [Header("Parámetros del Juego")]
    public float timeRemaining = 90f;
    public int requiredScore = 50;

    [Header("Referencias de UI y Juego")]
    public TMP_Text timerText;
    public PlayerStats playerStats;
    public GameObject goldenKeyPrefab;
    public GameObject winPanel;
    public GameObject gameOverPanel;
    public GameObject mainMenuPanel;
    public GameObject pausePanel;

    public Vector3 keySpawnPosition = new Vector3(0, -4.5f, 0);

    private bool keySpawned = false;
    private bool gameStarted = false;
    private bool isPaused = false;

    private static bool shouldAutoStart = false;

    private void Start()
    {
        // 1. Asigna el personaje seleccionado al jugador en cuanto arranca la escena
        SetupPlayerCharacter();

        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        UpdateTimerUI(); // Muestra el tiempo inicial (01:30) en cuanto arranca la escena

        if (shouldAutoStart)
        {
            shouldAutoStart = false;
            StartGame();
        }
        else
        {
            Time.timeScale = 0f;
            gameStarted = false;
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }
    }

    // Carga el personaje desde PlayerPrefs y aplica su Animator Override Controller
    private void SetupPlayerCharacter()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        if (characterAnimators != null && characterAnimators.Length > 0)
        {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, characterAnimators.Length - 1);

            if (player != null && characterAnimators[selectedIndex] != null)
            {
                player.SetCharacterSkin(characterAnimators[selectedIndex]);
                Debug.Log($"[GameManager] Personaje asignado con éxito: Índice {selectedIndex}");
            }
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        isPaused = false;
        Time.timeScale = 1f; // Reanuda la velocidad normal del motor

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        SetupPlayerCharacter();

        UpdateTimerUI();
    }

    private void Update()
    {
        // Controles de pausa por teclado
        if (gameStarted && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)))
        {
            TogglePause();
        }

        // Si el juego no ha comenzado o está pausado, no resta tiempo
        if (!gameStarted || isPaused) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            UpdateTimerUI();
            CheckKeySpawn();
        }
    }

    public void TogglePause()
    {
        if (!gameStarted) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }

    public void PauseGame()
    {
        if (!gameStarted) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("Tiempo: {0:00}:{1:00}", minutes, seconds);
        }
    }

    private void CheckKeySpawn()
    {
        if (keySpawned) return;

        if (playerStats != null && playerStats.score >= requiredScore)
        {
            SpawnKey();
        }
    }

    private void SpawnKey()
    {
        keySpawned = true;
        if (goldenKeyPrefab != null)
        {
            Instantiate(goldenKeyPrefab, keySpawnPosition, Quaternion.identity);
            Debug.Log("¡La Llave Dorada ha aparecido!");
        }
    }

    public void OnWinGame()
    {
        Time.timeScale = 0f;
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void OnGameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void OnWrongAnswerExtraTime()
    {
        keySpawned = false;
        timeRemaining = 30f;
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        shouldAutoStart = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevel()
    {
        shouldAutoStart = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}