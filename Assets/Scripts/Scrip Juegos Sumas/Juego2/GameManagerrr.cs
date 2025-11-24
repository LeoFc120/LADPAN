using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManagerrr : MonoBehaviour
{
    [Header("Referencias UI Ecuaciones")]
    public Text[] row1Texts;
    public DropSlot row1Slot;

    [Header("UI Puntuación")] // NUEVO: Sección de puntos
    public Text scoreText;    // Arrastra aquí tu texto de "Puntos: 0"

    public Text[] row2Texts;
    public DropSlot row2Slot;

    public Text[] row3Texts;
    public DropSlot row3Slot;

    [Header("Referencias UI Respuestas")]
    public DraggableItem[] answerOptions;
    public Text[] answerTexts;
    public Transform answersParent;

    [Header("UI Juego")]
    public GameObject gameOverText; // Arrastra aquí tu texto de Fin del Juego

    [Header("Base de Datos")] // NUEVO
    public LevelSaver databaseScript;

    // Variables de estado
    private int currentLevel = 1;
    private int correctCount = 0; // Cuántas lleva bien en este nivel

    // Variables de Puntuación
    private int currentScore = 0;      // NUEVO
    private int pointsPerLevel = 30;  // NUEVO: 100 puntos por cada ecuación, se dan juntos al final

    void Start()
    {
        gameOverText.SetActive(false); // Asegurar que esté apagado

        // NUEVO: Iniciar UI de puntos
        UpdateScoreUI();

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        correctCount = 0; // Reiniciar contador

        // Limpiar los slots (marcarlos como vacíos)
        row1Slot.isFilled = false;
        row2Slot.isFilled = false;
        row3Slot.isFilled = false;

        // Limpiar textos viejos de los slots
        row1Slot.GetComponentInChildren<Text>().text = "";
        row2Slot.GetComponentInChildren<Text>().text = "";
        row3Slot.GetComponentInChildren<Text>().text = "";

        List<int> correctAnswers = new List<int>();

        // Generar ecuaciones con dificultad basada en el nivel
        SetupEquation(row1Texts, row1Slot, correctAnswers);
        SetupEquation(row2Texts, row2Slot, correctAnswers);
        SetupEquation(row3Texts, row3Slot, correctAnswers);

        // Rellenar respuestas (Correctas + Falsas)
        List<int> finalOptions = new List<int>(correctAnswers);

        while (finalOptions.Count < answerOptions.Length)
        {
            // Los números falsos también crecen con el nivel para despistar
            int fakeMax = 15 + (currentLevel * 5);
            int fakeNumber = Random.Range(1, fakeMax);
            if (!finalOptions.Contains(fakeNumber)) finalOptions.Add(fakeNumber);
        }

        Shuffle(finalOptions);

        // Mostrar fichas
        for (int i = 0; i < answerOptions.Length; i++)
        {
            answerOptions[i].numberValue = finalOptions[i];
            answerTexts[i].text = finalOptions[i].ToString();
            answerOptions[i].gameObject.SetActive(true);
            answerOptions[i].GetComponent<CanvasGroup>().blocksRaycasts = true;

            // Mover al padre correcto
            answerOptions[i].transform.SetParent(answersParent);

            // Esto asegura que la escala sea 1 (a veces se hace pequeña al cambiar de padre)
            answerOptions[i].transform.localScale = Vector3.one;
        }
    }

    void SetupEquation(Text[] texts, DropSlot slot, List<int> answers)
    {
        // FORMULA DE DIFICULTAD:
        // Nivel 1: Números entre 5 y 12
        int minNum = 5 + (currentLevel * 2);
        int maxNum = 10 + (currentLevel * 5);

        int numA = Random.Range(minNum, maxNum);
        int numB = Random.Range(1, numA); // B siempre menor para evitar negativos
        int result = numA - numB;

        texts[0].text = numA.ToString();
        texts[1].text = numB.ToString();

        slot.expectedResult = result;
        answers.Add(result);
    }

    // Esta función la llama el DropSlot
    public void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            correctCount++;
            // Si completó las 3 ecuaciones...
            if (correctCount >= 3)
            {
                Debug.Log("¡Nivel Completado! Subiendo dificultad.");

                // NUEVO: Sumar puntos solo al completar las 3
                currentScore += pointsPerLevel;
                UpdateScoreUI();

                // Guardar en Base de Datos
                if (databaseScript != null) databaseScript.SaveProgress(currentLevel);

                currentLevel++; // SUBE NIVEL
                Invoke("GenerateLevel", 1f); // Espera 1 segundo y genera el siguiente
            }
        }
        else
        {
            // PERDIO
            // Si falla UNA sola ecuación, pierde.
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel);
            StartCoroutine(GameOverSequence());
        }
    }

    // NUEVO: Función para actualizar texto
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore.ToString();
        }
    }

    // NUEVO: Getter público
    public int GetCurrentScore()
    {
        return currentScore;
    }

    IEnumerator GameOverSequence()
    {
        gameOverText.SetActive(true); // Mostrar texto

        // Opcional: Asegurar que el texto diga algo en español si tiene un componente Text
        if (gameOverText.GetComponent<Text>()) gameOverText.GetComponent<Text>().text = "¡HAS FALLADO!";

        yield return new WaitForSeconds(2f); // Esperar 2 segundos

        gameOverText.SetActive(false);

        // NUEVO: Reiniciar puntos al perder
        currentScore = 0;
        UpdateScoreUI();

        currentLevel = 1; // REINICIAR NIVEL A 1
        GenerateLevel(); // Empezar de nuevo
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}