using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum QuestionDifficulty { Easy, Medium, Hard }

[System.Serializable]
public class QuestionData
{
    public string questionText;
    public string[] options = new string[3];
    public int correctIndex;
    public string difficulty; // "Easy", "Medium", "Hard"
    public string category;   // "Hierro", "Nutrición", "Anemia", "Alimentacion", etc.
}

[System.Serializable]
public class QuestionContainer
{
    public List<QuestionData> questions;
}

public class QuestionManager : MonoBehaviour
{
    [Header("Interfaz de UI")]
    public GameObject triviaPanel;
    public TMP_Text questionText;
    public TMP_Text[] buttonTexts;

    [Header("Carga de Datos externa")]
    [Tooltip("Nombre del archivo JSON dentro de Assets/Resources (sin la extensión .json)")]
    public string jsonFileName = "questions";

    [Header("Referencias del Juego")]
    public GameManager gameManager;
    public PlayerStats playerStats;

    private List<QuestionData> allQuestions = new List<QuestionData>();
    private QuestionData currentQuestion;

    private void Awake()
    {
        LoadQuestionsFromJSON();
    }

    private void Start()
    {
        if (triviaPanel != null)
            triviaPanel.SetActive(false);
    }

    // Carga y convierte el archivo JSON en objetos C#
    public void LoadQuestionsFromJSON()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFileName);

        if (jsonTextAsset != null)
        {
            QuestionContainer container = JsonUtility.FromJson<QuestionContainer>(jsonTextAsset.text);
            allQuestions = container.questions;
            Debug.Log($"[QuestionManager] Cargadas {allQuestions.Count} preguntas correctamente desde JSON.");
        }
        else
        {
            Debug.LogError($"[QuestionManager] No se encontró el archivo '{jsonFileName}.json' en la carpeta Assets/Resources.");
        }
    }

    // A mayor puntaje acumulado, MÁS FÁCIL será la pregunta (Regla invertida de Nutriman)
    public void OpenTrivia(int score)
    {
        QuestionDifficulty targetDifficulty = QuestionDifficulty.Hard;

        if (score >= 200)
        {
            targetDifficulty = QuestionDifficulty.Easy;
        }
        else if (score >= 100)
        {
            targetDifficulty = QuestionDifficulty.Medium;
        }

        OpenTriviaFiltered(targetDifficulty, null); // null = cualquier categoría por ahora
    }

    // Permite filtrar preguntas por Dificultad y opcionalmente por Categoría/Área
    public void OpenTriviaFiltered(QuestionDifficulty difficulty, string category = null)
    {
        Time.timeScale = 0f; // Pausa el juego durante la trivia

        if (triviaPanel != null)
            triviaPanel.SetActive(true);

        string difficultyString = difficulty.ToString();

        // Filtrar del banco total según criterios
        List<QuestionData> matchingQuestions = allQuestions.FindAll(q =>
            q.difficulty.Equals(difficultyString, System.StringComparison.OrdinalIgnoreCase) &&
            (string.IsNullOrEmpty(category) || q.category.Equals(category, System.StringComparison.OrdinalIgnoreCase))
        );

        // Fallback: si no hay preguntas filtradas, busca en todo el banco
        if (matchingQuestions.Count == 0)
        {
            matchingQuestions = allQuestions;
        }

        if (matchingQuestions.Count > 0)
        {
            int randomIndex = Random.Range(0, matchingQuestions.Count);
            currentQuestion = matchingQuestions[randomIndex];
        }

        DisplayQuestion();
    }

    private void DisplayQuestion()
    {
        if (currentQuestion == null) return;

        if (questionText != null)
            questionText.text = currentQuestion.questionText;

        for (int i = 0; i < buttonTexts.Length && i < currentQuestion.options.Length; i++)
        {
            if (buttonTexts[i] != null)
                buttonTexts[i].text = currentQuestion.options[i];
        }
    }

    public void SelectOption(int optionIndex)
    {
        if (triviaPanel != null)
            triviaPanel.SetActive(false);

        if (currentQuestion != null && optionIndex == currentQuestion.correctIndex)
        {
            if (gameManager != null)
                gameManager.OnWinGame();
        }
        else
        {
            if (playerStats != null)
                playerStats.LoseLife();

            if (gameManager != null)
                gameManager.OnWrongAnswerExtraTime();
            else
                Time.timeScale = 1f;
        }
    }
}