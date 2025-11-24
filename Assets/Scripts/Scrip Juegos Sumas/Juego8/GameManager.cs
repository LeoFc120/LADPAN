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

    [Header("UI Puntuación")] // NUEVO: Sección para los puntos
    public TextMeshProUGUI scoreText; // Arrastra aquí tu texto de "Puntos: 0"

    [Header("UI Juego")]
    public GameObject gameOverText; // Arrastra aquí un texto de "GAME OVER" (opcional)

    [Header("Estado del Juego")]
    public int currentLevel = 1;
    private int targetResult;

    // Variables de Puntuación
    private int currentScore = 0;       // NUEVO
    private int pointsPerLevel = 15;    // NUEVO: Puntos por nivel completado

    // Referencia al DatabaseManager
    private DatabaseManager dbManager;

    void Start()
    {
        // Ocultar Game Over al inicio
        if (gameOverText) gameOverText.SetActive(false);

        // NUEVO: Iniciar UI de puntos
        UpdateScoreUI();

        // Conexión segura con la base de datos
        dbManager = FindObjectOfType<DatabaseManager>();

        if (dbManager != null)
        {
            currentLevel = dbManager.LoadLevel();
        }
        else
        {
            Debug.LogWarning("No se encontró DatabaseManager. Iniciando en Nivel 1.");
            currentLevel = 1;
        }

        levelText.text = "Nivel: " + currentLevel;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(CheckAnswer);

        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Lógica matemática
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
                petals[i].Setup(values[i]);
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

            // NUEVO: Sumar puntos
            currentScore += pointsPerLevel;
            UpdateScoreUI();

            LevelUp();
        }
        else
        {
            Debug.Log("Incorrecto. Game Over.");
            // Si te equivocas en la multiplicación, pierdes el avance
            StartCoroutine(GameOverSequence());
        }
    }

    void LevelUp()
    {
        currentLevel++;
        levelText.text = "Nivel: " + currentLevel;

        // Guardar progreso
        if (dbManager != null)
        {
            dbManager.SaveProgress(currentLevel);
        }

        GenerateLevel();
    }

    // NUEVO: Secuencia de Game Over
    IEnumerator GameOverSequence()
    {
        if (gameOverText) gameOverText.SetActive(true);

        // Bloquear botón para que no sigan pulsando
        confirmButton.interactable = false;

        yield return new WaitForSeconds(2f);

        if (gameOverText) gameOverText.SetActive(false);
        confirmButton.interactable = true;

        // Reiniciar puntos y nivel
        currentScore = 0;
        UpdateScoreUI();

        // Opcional: ¿Quieres que el nivel vuelva a 1 o que solo se reinicie el actual?
        // Aquí lo dejo para que vuelva a 1 (mayor dificultad).
        currentLevel = 1;
        levelText.text = "Nivel: " + currentLevel;

        GenerateLevel();
    }

    // NUEVO: Actualizar texto de puntos
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore.ToString();
        }
    }

    // NUEVO: Para leer los puntos desde fuera
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