using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI centerText; // El resultado objetivo
    public TextMeshProUGUI levelText;
    public Button confirmButton;
    public List<PetalController> petals; // Arrastra todos los pétalos aquí en el inspector

    [Header("Game State")]
    public int currentLevel = 1;
    private int targetResult;

    // Referencia al DatabaseManager
    private DatabaseManager dbManager;

    void Start()
    {
        // CORRECCIÓN 1: Usamos FindObjectOfType por si pusiste el script en otro objeto por error.
        // Es más seguro que GetComponent.
        dbManager = FindObjectOfType<DatabaseManager>();

        // CORRECCIÓN 2: Protección contra errores (Null Check)
        if (dbManager != null)
        {
            currentLevel = dbManager.LoadLevel();
        }
        else
        {
            Debug.LogWarning("No se encontró DatabaseManager. Iniciando en Nivel 1 por defecto.");
            currentLevel = 1;
        }

        levelText.text = "Nivel: " + currentLevel;

        // Limpiamos los listeners anteriores por seguridad y agregamos el nuevo
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

        // Llenar el resto con números random
        for (int i = 2; i < petals.Count; i++)
        {
            int randomVal = Random.Range(2, maxRange + 5);
            // Pequeña mejora: Evitar que el distractor sea igual al resultado (opcional pero recomendado)
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
                // Esto llama al Setup del pétalo, que reinicia su color y selección automáticamente
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

        // CORRECCIÓN 3: Aseguramos que solo validamos si hay al menos 2 pétalos seleccionados
        // (porque una multiplicación necesita al menos dos factores)
        if (petalsSelectedCount >= 2 && currentProduct == targetResult)
        {
            Debug.Log("¡Correcto!");
            LevelUp();
        }
        else
        {
            Debug.Log("Incorrecto. Resultado actual: " + currentProduct + " / Objetivo: " + targetResult);
            // Opcional: Aquí podrías añadir un efecto visual de error
        }
    }

    void LevelUp()
    {
        currentLevel++;
        levelText.text = "Nivel: " + currentLevel;

        // Guardar progreso solo si existe la base de datos
        if (dbManager != null)
        {
            dbManager.SaveProgress(currentLevel);
        }

        GenerateLevel();
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
