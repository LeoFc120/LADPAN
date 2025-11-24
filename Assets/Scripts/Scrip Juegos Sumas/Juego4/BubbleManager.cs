using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleManager : MonoBehaviour
{
    [Header("UI References")]
    public Text targetText;
    public Transform bubbleArea; // El panel de la izquierda
    public GameObject bubblePrefab; // El prefab que creamos

    [Header("Game UI")]
    public GameObject gameOverText; // Texto de perder/ganar

    [Header("Base de Datos")]
    public LevelSaver databaseScript; // Para guardar nivel

    // Variables de juego
    private int currentTarget;
    private int currentLevel = 1;

    void Start()
    {
        if (gameOverText) gameOverText.SetActive(false);
        StartLevel();
    }

    void StartLevel()
    {
        // Limpiar burbujas viejas si quedaron
        foreach (Transform child in bubbleArea)
        {
            Destroy(child.gameObject);
        }

        // Dificultad: El número objetivo crece con el nivel
        // Nivel 1: Objetivo 10-20. Nivel 2: 20-30, etc.
        int minTarget = 10 + (currentLevel * 5);
        int maxTarget = 20 + (currentLevel * 10);
        currentTarget = Random.Range(minTarget, maxTarget);

        UpdateUI();
        SpawnBubbles();
    }

    void SpawnBubbles()
    {
        // Generamos 3 burbujas iniciales
        for (int i = 0; i < 3; i++)
        {
            CreateOneBubble();
        }
    }

    public void CreateOneBubble()
    {
        // Crear la burbuja visualmente dentro del BubbleArea
        GameObject newBubble = Instantiate(bubblePrefab, bubbleArea);

        // Posición Aleatoria dentro del área
        RectTransform rect = bubbleArea.GetComponent<RectTransform>();
        float x = Random.Range(-rect.rect.width / 2 + 50, rect.rect.width / 2 - 50);
        float y = Random.Range(-rect.rect.height / 2 + 50, rect.rect.height / 2 - 50);
        newBubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

        // Calcular valor: 
        // Debe ser un número aleatorio, pero NO mayor que el objetivo actual 
        // (para que no pierdas al primer clic)
        int maxVal = currentTarget > 1 ? currentTarget : 1;
        int val = Random.Range(1, Mathf.Min(10, maxVal + 1)); // Máximo valor de burbuja 10 para que no sea tan fácil

        // Configurar el script de la burbuja
        newBubble.GetComponent<BubbleController>().Setup(val);
    }

    // Esta función la llama la burbuja al explotar
    public void ProcessBubble(int valueSubtracted)
    {
        currentTarget -= valueSubtracted; // RESTAR
        UpdateUI();

        if (currentTarget == 0)
        {
            // GANASTE NIVEL
            Debug.Log("¡Nivel Completado!");
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel);
            currentLevel++;
            Invoke("StartLevel", 1.5f); // Siguiente nivel en 1.5 seg
        }
        else if (currentTarget < 0)
        {
            // PERDISTE (Te pasaste de la resta)
            Debug.Log("Game Over - Te pasaste");
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel);
            StartCoroutine(GameOverSequence());
        }
        else
        {
            // EL JUEGO SIGUE
            // Al explotar una, generamos otra para que siempre haya opciones
            CreateOneBubble();
        }
    }

    void UpdateUI()
    {
        targetText.text = currentTarget.ToString();
    }

    IEnumerator GameOverSequence()
    {
        if (gameOverText)
        {
            gameOverText.GetComponent<Text>().text = "¡TE PASASTE!";
            gameOverText.SetActive(true);
        }
        yield return new WaitForSeconds(2f);
        if (gameOverText) gameOverText.SetActive(false);

        currentLevel = 1; // Reiniciar
        StartLevel();
    }
}
