using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    // Cambia este índice desde los botones de la interfaz
    public void SelectCharacter(int characterIndex)
    {
        // Guarda el índice del personaje seleccionado (0 al 6)
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.Save();

        Debug.Log("Personaje seleccionado: " + characterIndex);
    }

    // Método para cargar la escena de juego
    public void StartGame(string gameSceneName)
    {
        SceneManager.LoadScene(gameSceneName);
    }
}