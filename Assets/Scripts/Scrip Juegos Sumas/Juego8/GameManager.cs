using System.Collections;
using System.Collections.Generic;
using TMPro; // Usamos TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Referencias")]
    public TextMeshProUGUI centerText; // El resultado objetivo
    public TextMeshProUGUI levelText;
    public Button confirmButton;
    public List<PetalController> petals; // Arrastra todos los pétalos aquí

    [Header("UI Puntuación")]
    public TextMeshProUGUI scoreText; // Arrastra aquí tu texto de "Puntos: 0"

    [Header("UI Juego")]
    public GameObject gameOverText; // Texto de "GAME OVER"

    [Header("Estado del Juego")]
    public int currentLevel = 1;
    private int targetResult;

    // Variables de Puntuación
    private int currentScore = 0;
    private int pointsPerLevel = 15;

    // ID ÚNICO PARA ESTE JUEGO
    private string gameID = "JuegoFlores";

    void Start()
    {
        // Ocultar Game Over al inicio
        if (gameOverText) gameOverText.SetActive(false);

        // Iniciar UI de puntos
        UpdateScoreUI();

        // --- CARGA DE NIVEL (Opcional) ---
        // Si quieres que el juego recuerde en qué nivel se quedó el usuario:
        if (DatabaseManager.Instance != null && GameSession.Current != null && GameSession.Current.CurrentUser != null)
        {
            int myUserId = GameSession.Current.CurrentUser.Id;
            // Cargamos el nivel guardado
            currentLevel = DatabaseManager.Instance.LoadLevel(myUserId);
        }
        else
        {
            Debug.LogWarning("Modo prueba: Iniciando en Nivel 1.");
            currentLevel = 1;
        }

        levelText.text = "Nivel: " + currentLevel;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(CheckAnswer);

        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Lógica matemática (Multiplicación)
        int minRange = 2 + (currentLevel / 5);
        int maxRange = 10 + (currentLevel / 2);

        int factorA = Random.Range(minRange, maxRange);
        int factorB = Random.Range(minRange, maxRange);

        targetResult = factorA * factorB;
        centerText.text = targetResult.ToString();

        // Preparar lista de valores
        List<int> values = new List<int>();
        values.Add(factorA);
        values.Add(factorB);

        // Llenar el resto con números random (distractores)
        for (int i = 2; i < petals.Count; i++)
        {
            int randomVal = Random.Range(2, maxRange + 5);

            // Evitar que el distractor sea igual al resultado
            while (randomVal == targetResult)
            {
                randomVal = Random.Range(2, maxRange + 5);
            }
            values.Add(randomVal);
        }

        // Barajar
        Shuffle(values);

        // Asignar a los pétalos
        for (int i = 0; i < petals.Count; i++)
        {
            if (i < values.Count)
            {
                // Asegúrate que tu script PetalController tenga el método Setup
                if (petals[i] != null) petals[i].Setup(values[i]);
            }
        }
    }

    void CheckAnswer()
    {
        int currentProduct = 1;
        int petalsSelectedCount = 0;

        foreach (var petal in petals)
        {
            if (petal.isSelected)
            {
                currentProduct *= petal.value;
                petalsSelectedCount++;
            }
        }

        // VALIDACIÓN: Necesitamos al menos 2 pétalos y que el producto sea exacto
        if (petalsSelectedCount >= 2 && currentProduct == targetResult)
        {
            Debug.Log("¡Correcto!");

            // Sumar puntos
            currentScore += pointsPerLevel;
            UpdateScoreUI();

            LevelUp();
        }
        else
        {
            Debug.Log("Incorrecto. Game Over.");

            // --- AL PERDER: Guardar PUNTAJE final ---
            SaveMyScore();

            StartCoroutine(GameOverSequence());
        }
    }

    void LevelUp()
    {
        currentLevel++;
        levelText.text = "Nivel: " + currentLevel;

        // --- GUARDADO DE PROGRESO (NIVEL) ---
        if (DatabaseManager.Instance != null && GameSession.Current != null && GameSession.Current.CurrentUser != null)
        {
            int myUserId = GameSession.Current.CurrentUser.Id;
            DatabaseManager.Instance.SaveProgress(myUserId, currentLevel);
        }

        GenerateLevel();
    }

    // --- NUEVO: Guardar PUNTAJE en la DB ---
    void SaveMyScore()
    {
        if (DatabaseManager.Instance != null && GameSession.Current != null && GameSession.Current.CurrentUser != null)
        {
            int myUserId = GameSession.Current.CurrentUser.Id;
            // Guardamos con el ID "JuegoFlores"
            DatabaseManager.Instance.SaveScore(myUserId, gameID, currentScore);
            Debug.Log($"Puntaje de Flores guardado: {currentScore}");
        }
    }

    // Secuencia de Game Over
    IEnumerator GameOverSequence()
    {
        if (gameOverText) gameOverText.SetActive(true);

        // Bloquear botón
        confirmButton.interactable = false;

        yield return new WaitForSeconds(2f);

        if (gameOverText) gameOverText.SetActive(false);
        confirmButton.interactable = true;

        // Reiniciar puntos y nivel
        currentScore = 0;
        UpdateScoreUI();

        // Reinicia a nivel 1
        currentLevel = 1;
        levelText.text = "Nivel: " + currentLevel;

        GenerateLevel();
    }

    // Actualizar texto de puntos
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore.ToString();
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}