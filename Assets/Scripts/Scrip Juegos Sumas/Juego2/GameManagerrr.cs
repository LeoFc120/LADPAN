using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManagerrr : MonoBehaviour
{
    [Header("Referencias UI Ecuaciones")]
    // texts[0] será el primer número, texts[1] será el segundo.
    public Text[] row1Texts;
    public DropSlot row1Slot;

    [Header("UI Puntuación")]
    public Text scoreText;

    public Text[] row2Texts;
    public DropSlot row2Slot;

    public Text[] row3Texts;
    public DropSlot row3Slot;

    [Header("Referencias UI Respuestas")]
    public DraggableItem[] answerOptions;
    public Text[] answerTexts;
    public Transform answersParent;

    [Header("UI Juego")]
    public GameObject gameOverText;

    // Variables de estado
    private int currentLevel = 1;
    private int correctCount = 0;

    // Variables de Puntuación
    private int currentScore = 0; // Puntos de la sesión actual
    private int pointsPerLevel = 30; // Puntos que gana al pasar nivel

    // ID ÚNICO: Este ID debe coincidir en la base de datos para este juego específico
    private string gameID = "JuegoRestas";

    void Start()
    {
        if (gameOverText) gameOverText.SetActive(false);

        // --- NUEVO: Cargar Nivel desde la Base de Datos ---
        if (DatabaseManager.Instance != null && GameSession.CurrentUser != null)
        {
            // Pedimos a la DB el nivel donde se quedó este alumno en "JuegoRestas"
            currentLevel = DatabaseManager.Instance.LoadLevel(GameSession.CurrentUser.Id, gameID);

            // Si devuelve 0 o menor, forzamos nivel 1
            if (currentLevel < 1) currentLevel = 1;
        }
        else
        {
            currentLevel = 1; // Modo prueba sin login
        }

        UpdateScoreUI();
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        correctCount = 0;

        // Limpiar los slots visualmente
        ResetSlot(row1Slot);
        ResetSlot(row2Slot);
        ResetSlot(row3Slot);

        List<int> correctAnswers = new List<int>();

        // Generar ecuaciones (Restas)
        SetupEquation(row1Texts, row1Slot, correctAnswers);
        SetupEquation(row2Texts, row2Slot, correctAnswers);
        SetupEquation(row3Texts, row3Slot, correctAnswers);

        // Rellenar respuestas (Correctas + Distractores)
        List<int> finalOptions = new List<int>(correctAnswers);

        while (finalOptions.Count < answerOptions.Length)
        {
            int fakeMax = 15 + (currentLevel * 5);
            int fakeNumber = Random.Range(1, fakeMax);
            if (!finalOptions.Contains(fakeNumber)) finalOptions.Add(fakeNumber);
        }

        Shuffle(finalOptions);

        // Asignar valores a las fichas arrastrables
        for (int i = 0; i < answerOptions.Length; i++)
        {
            answerOptions[i].numberValue = finalOptions[i];
            answerTexts[i].text = finalOptions[i].ToString();

            // Reactivar fichas
            answerOptions[i].gameObject.SetActive(true);
            answerOptions[i].GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (answersParent != null) answerOptions[i].transform.SetParent(answersParent);
            answerOptions[i].transform.localScale = Vector3.one;
        }
    }

    // Función auxiliar para limpiar slots
    void ResetSlot(DropSlot slot)
    {
        slot.isFilled = false;
        var textComp = slot.GetComponentInChildren<Text>();
        if (textComp) textComp.text = "";
    }

    void SetupEquation(Text[] texts, DropSlot slot, List<int> answers)
    {
        // LOGICA DE RESTAS (Aumenta dificultad según currentLevel)
        int minNum = 5 + (currentLevel * 2);
        int maxNum = 10 + (currentLevel * 5);

        int numA = Random.Range(minNum, maxNum);
        int numB = Random.Range(1, numA); // B menor que A para evitar negativos
        int result = numA - numB;

        texts[0].text = numA.ToString();
        texts[1].text = numB.ToString();

        slot.expectedResult = result;
        answers.Add(result);
    }

    public void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            correctCount++;

            // Si completa las 3 ecuaciones de la pizarra
            if (correctCount >= 3)
            {
                Debug.Log("¡Nivel Completado!");

                // 1. Sumar puntos locales (para mostrar en pantalla)
                currentScore += pointsPerLevel;
                UpdateScoreUI();

                // 2. Subir nivel
                currentLevel++;

                // 3. --- NUEVO: Guardar Progreso en DB ---
                SaveProgress(pointsPerLevel);
                // Nota: Pasamos 'pointsPerLevel' para que la DB los sume al total histórico

                Invoke("GenerateLevel", 1f);
            }
        }
        else
        {
            // AL PERDER
            // No guardamos puntos extra porque falló, pero el nivel maximo ya está guardado
            StartCoroutine(GameOverSequence());
        }
    }

    // Guardar en la DB
    void SaveProgress(int puntosGanados)
    {
        if (DatabaseManager.Instance != null && GameSession.CurrentUser != null)
        {
            int myUserId = GameSession.CurrentUser.Id;

            // Guardamos: ID Alumno, ID Juego, Puntos a sumar, Nivel alcanzado
            DatabaseManager.Instance.GuardarProgreso(myUserId, gameID, puntosGanados, currentLevel);

            Debug.Log($"Progreso Restas guardado: Nivel {currentLevel}");
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore.ToString();
        }
    }

    IEnumerator GameOverSequence()
    {
        if (gameOverText) gameOverText.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (gameOverText) gameOverText.SetActive(false);

        // Reiniciar puntaje de sesión (opcional)
        currentScore = 0;
        UpdateScoreUI();

        // Puedes decidir si reinicias el nivel a 1 o lo dejas donde estaba
        // currentLevel = 1; // Descomenta si quieres castigo de volver al inicio

        GenerateLevel();
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