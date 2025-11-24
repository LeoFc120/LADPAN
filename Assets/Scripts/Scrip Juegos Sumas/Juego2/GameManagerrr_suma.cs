using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManagerrr_suma : MonoBehaviour
{
    [Header("Referencias UI Ecuaciones")]
    public Text[] row1Texts;
    public DropSlot_SUMA row1Slot; // Referencia corregida

    [Header("UI Puntuación")]
    public Text scoreText;

    public Text[] row2Texts;
    public DropSlot_SUMA row2Slot; // Referencia corregida

    public Text[] row3Texts;
    public DropSlot_SUMA row3Slot; // Referencia corregida

    [Header("Referencias UI Respuestas")]
    public DraggableItem_SUMA[] answerOptions; // Referencia corregida
    public Text[] answerTexts;
    public Transform answersParent;

    [Header("UI Juego")]
    public GameObject gameOverText;

    [Header("Base de Datos")]
    public LevelSaver databaseScript;

    // Variables internas
    private int currentLevel = 1;
    private int correctCount = 0;
    private int currentScore = 0;
    private int pointsPerLevel = 30;

    void Start()
    {
        if (gameOverText) gameOverText.SetActive(false);
        UpdateScoreUI();
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        correctCount = 0;

        // Validación para evitar errores si olvidas arrastrar algo
        if (row1Slot == null || row2Slot == null || row3Slot == null)
        {
            Debug.LogError("¡ALERTA! Faltan asignar los Slots en el Inspector del Manager.");
            return;
        }

        // Reiniciar slots
        row1Slot.isFilled = false;
        row2Slot.isFilled = false;
        row3Slot.isFilled = false;

        // Limpiar textos viejos
        if (row1Slot.GetComponentInChildren<Text>()) row1Slot.GetComponentInChildren<Text>().text = "";
        if (row2Slot.GetComponentInChildren<Text>()) row2Slot.GetComponentInChildren<Text>().text = "";
        if (row3Slot.GetComponentInChildren<Text>()) row3Slot.GetComponentInChildren<Text>().text = "";

        List<int> correctAnswers = new List<int>();

        // Crear Ecuaciones
        SetupEquation(row1Texts, row1Slot, correctAnswers);
        SetupEquation(row2Texts, row2Slot, correctAnswers);
        SetupEquation(row3Texts, row3Slot, correctAnswers);

        // Crear Respuestas (3 correctas + 2 falsas)
        List<int> finalOptions = new List<int>(correctAnswers);

        while (finalOptions.Count < answerOptions.Length)
        {
            int fakeMax = 20 + (currentLevel * 10);
            int fakeNumber = Random.Range(1, fakeMax);
            if (!finalOptions.Contains(fakeNumber)) finalOptions.Add(fakeNumber);
        }

        Shuffle(finalOptions);

        // Asignar valores a las fichas
        for (int i = 0; i < answerOptions.Length; i++)
        {
            if (answerOptions[i] != null)
            {
                answerOptions[i].numberValue = finalOptions[i];
                if (answerTexts[i] != null) answerTexts[i].text = finalOptions[i].ToString();

                answerOptions[i].gameObject.SetActive(true);
                // Asegurar que se puedan tocar
                if (answerOptions[i].GetComponent<CanvasGroup>())
                    answerOptions[i].GetComponent<CanvasGroup>().blocksRaycasts = true;

                if (answersParent) answerOptions[i].transform.SetParent(answersParent);
                answerOptions[i].transform.localScale = Vector3.one;
            }
        }
    }

    void SetupEquation(Text[] texts, DropSlot_SUMA slot, List<int> answers)
    {
        int rangeMax = 5 + (currentLevel * 3);
        int numA = Random.Range(1, rangeMax);
        int numB = Random.Range(1, rangeMax);

        int result = numA + numB; // SUMA

        if (texts.Length > 1)
        {
            texts[0].text = numA.ToString();
            texts[1].text = numB.ToString();
        }

        slot.expectedResult = result;
        answers.Add(result);
    }

    public void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            correctCount++;
            // Ganas si aciertas las 3
            if (correctCount >= 3)
            {
                Debug.Log("Nivel completado");
                currentScore += pointsPerLevel;
                UpdateScoreUI();

                if (databaseScript != null) databaseScript.SaveProgress(currentLevel);

                currentLevel++;
                Invoke("GenerateLevel", 1f);
            }
        }
        else
        {
            // Pierdes si fallas una
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel);
            StartCoroutine(GameOverSequence());
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Puntos: " + currentScore.ToString();
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    IEnumerator GameOverSequence()
    {
        if (gameOverText)
        {
            gameOverText.SetActive(true);
            if (gameOverText.GetComponent<Text>()) gameOverText.GetComponent<Text>().text = "¡HAS FALLADO!";
        }

        yield return new WaitForSeconds(2f);

        if (gameOverText) gameOverText.SetActive(false);

        // Reiniciar puntos
        currentScore = 0;
        UpdateScoreUI();
        currentLevel = 1;
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