using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManagerrrSuma : MonoBehaviour
{
    [Header("Referencias UI Ecuaciones")]
    public Text[] row1Texts;
    public DropSlot row1Slot;

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

    [Header("Base de Datos")]
    public LevelSaver databaseScript; // <--- AGREGADO PARA GUARDAR PROGRESO

    // Variables de estado
    private int currentLevel = 1;
    private int correctCount = 0;

    void Start()
    {
        if (gameOverText) gameOverText.SetActive(false);
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        correctCount = 0;

        // Limpiar los slots
        row1Slot.isFilled = false;
        row2Slot.isFilled = false;
        row3Slot.isFilled = false;

        // Limpiar textos
        row1Slot.GetComponentInChildren<Text>().text = "";
        row2Slot.GetComponentInChildren<Text>().text = "";
        row3Slot.GetComponentInChildren<Text>().text = "";

        List<int> correctAnswers = new List<int>();

        // Generar ecuaciones
        SetupEquation(row1Texts, row1Slot, correctAnswers);
        SetupEquation(row2Texts, row2Slot, correctAnswers);
        SetupEquation(row3Texts, row3Slot, correctAnswers);

        // Rellenar respuestas
        List<int> finalOptions = new List<int>(correctAnswers);

        while (finalOptions.Count < answerOptions.Length)
        {
            // En sumas, los resultados son más grandes, así que la trampa debe ser mayor
            int fakeMax = 20 + (currentLevel * 5);
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

            if (answersParent) answerOptions[i].transform.SetParent(answersParent);

            answerOptions[i].transform.localScale = Vector3.one;
        }
    }

    void SetupEquation(Text[] texts, DropSlot slot, List<int> answers)
    {
        // FORMULA DE DIFICULTAD PARA SUMAS:
        // Nivel 1: Sumas sencillas (ej. 5 + 3)
        // Nivel 2: Un poco más altas
        int maxNum = 8 + (currentLevel * 2);

        int numA = Random.Range(1, maxNum);
        int numB = Random.Range(1, maxNum); // Aquí B puede ser mayor que A sin problemas

        // --- CAMBIO PRINCIPAL: SUMA ---
        int result = numA + numB;
        // ------------------------------

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
            if (correctCount >= 3)
            {
                Debug.Log("¡Nivel Completado!");
                // Guardar progreso al ganar
                if (databaseScript != null) databaseScript.SaveProgress(currentLevel);

                currentLevel++;
                Invoke("GenerateLevel", 1f);
            }
        }
        else
        {
            // Guardar progreso al perder (el nivel al que llegó)
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel);

            StartCoroutine(GameOverSequence());
        }
    }

    IEnumerator GameOverSequence()
    {
        if (gameOverText) gameOverText.SetActive(true);
        yield return new WaitForSeconds(2f);

        if (gameOverText) gameOverText.SetActive(false);
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