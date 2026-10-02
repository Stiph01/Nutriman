using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    [Header("Componentes de Vista Previa")]
    [Tooltip("Asigna el Image del Canvas si la vista previa está en la UI")]
    public Image previewUIHolder;

    [Tooltip("Asigna el SpriteRenderer si el personaje de muestra está directo en el escenario 2D")]
    public SpriteRenderer previewWorldHolder;

    [Tooltip("Texto para mostrar el nombre del personaje")]
    public TMP_Text characterNameText;

    [Header("Datos de los Personajes (7 Personajes)")]
    public Sprite[] characterSprites; // Arrastra los 7 sprites de vista previa aquí
    public string[] characterNames = new string[]
    {
        "Nutriman Clásico",
        "Nutriman Cobalto",
        "Nutriman Escarlata",
        "Nutriman Verde",
        "Nutriman Dorado",
        "Nutriman Violeta",
        "Nutriman Neón"
    };

    private int currentIndex = 0;

    private void Start()
    {
        // Carga la última selección guardada
        currentIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        UpdateCharacterDisplay();
    }

    // Botón Siguiente (Flecha Derecha)
    public void NextCharacter()
    {
        if (characterSprites == null || characterSprites.Length == 0) return;

        currentIndex = (currentIndex + 1) % characterSprites.Length;
        UpdateCharacterDisplay();
    }

    public void NextOutfit() => NextCharacter();

    // Botón Anterior (Flecha Izquierda)
    public void PreviousCharacter()
    {
        if (characterSprites == null || characterSprites.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = characterSprites.Length - 1;
        }
        UpdateCharacterDisplay();
    }

    public void PreviousOutfit() => PreviousCharacter();

    // Actualiza la visualización y guarda la elección
    private void UpdateCharacterDisplay()
    {
        if (characterSprites == null || characterSprites.Length == 0) return;

        currentIndex = Mathf.Clamp(currentIndex, 0, characterSprites.Length - 1);

        // 1. Asigna el sprite
        if (previewUIHolder != null)
        {
            previewUIHolder.sprite = characterSprites[currentIndex];
        }

        if (previewWorldHolder != null)
        {
            previewWorldHolder.sprite = characterSprites[currentIndex];
        }

        // 2. Actualiza el nombre
        if (characterNameText != null && characterNames.Length > currentIndex)
        {
            characterNameText.text = characterNames[currentIndex];
        }

        // 3. Guarda en PlayerPrefs y muestra log de depuración
        PlayerPrefs.SetInt("SelectedCharacter", currentIndex);
        PlayerPrefs.Save();

        Debug.Log($"[SELECCIÓN] Guardado Índice: {currentIndex} | Personaje: {characterNames[currentIndex]} (Elemento {currentIndex})");
    }

    public void StartGame(string gameSceneName)
    {
        // Fuerza guardado del indice actual justo antes de cambiar de escena
        PlayerPrefs.SetInt("SelectedCharacter", currentIndex);
        PlayerPrefs.Save();

        // Cargar escena
        SceneManager.LoadScene(gameSceneName);
    }
}