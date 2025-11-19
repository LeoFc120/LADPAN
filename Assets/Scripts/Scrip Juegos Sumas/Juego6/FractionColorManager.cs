using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FractionColorManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public Text fractionText; // Muestra la fracción
    public Text levelText;
    public Text winLoseText;
    public GameObject gameOverText;

    [Header("Juego")]
    public Transform circleContainer; // Objeto vacío donde nacen las rebanadas
    public GameObject slicePrefab;    // Tu prefab de la rebanada
    public Button checkButton;        // Botón comprobar

    [Header("Base de Datos")]
    public LevelSaver databaseScript;

    // Variables internas
    private int currentLevel = 1;
    private int numerator;   // Cuántos hay que pintar
    private int denominator; // Total de rebanadas
    private List<SliceController> currentSlices = new List<SliceController>();

    void Start()
    {
        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (gameOverText) gameOverText.SetActive(false);

        checkButton.onClick.AddListener(CheckAnswer);

        StartLevel();
    }

    void StartLevel()
    {
        // 1. Limpiar el círculo anterior
        foreach (Transform child in circleContainer) Destroy(child.gameObject);
        currentSlices.Clear();

        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (levelText) levelText.text = "Level: " + currentLevel;

        // 2. DIFICULTAD
        // Nivel 1: Partir en 2, 3 o 4
        // Nivel 2: Partir en 4, 5 o 6
        // Nivel 3: Partir en 6, 7 u 8
        int minParts = 2;
        int maxParts = 3 + currentLevel;
        if (maxParts > 8) maxParts = 8; // Máximo visual recomendado

        denominator = Random.Range(minParts, maxParts + 1); // Total de rebanadas
        numerator = Random.Range(1, denominator); // Cuántas pintar (siempre menos que el total)

        // 3. Mostrar Texto (Formato Vertical)
        fractionText.text = numerator + "\n—\n" + denominator;

        // 4. Crear el círculo
        GenerateCircle(denominator);
    }

    void GenerateCircle(int parts)
    {
        // Cálculo Matemático:
        // Si el círculo es 1.0, cada rebanada llena (1.0 / partes)
        float fillAmount = 1.0f / parts;

        // Cada rebanada ocupa (360 grados / partes)
        float degreesPerSlice = 360f / parts;

        for (int i = 0; i < parts; i++)
        {
            // Crear rebanada dentro del contenedor
            GameObject newSlice = Instantiate(slicePrefab, circleContainer);

            // Obtener el script
            SliceController controller = newSlice.GetComponent<SliceController>();

            // Calcular rotación (Unity gira en sentido horario con Z negativo)
            float rotationZ = -(degreesPerSlice * i);

            // Configurar la rebanada
            controller.Setup(fillAmount, rotationZ);

            // --- TRUCO PARA EL CLIC ---
            // Le ponemos un botón invisible a la rebanada en tiempo real
            Button btn = newSlice.AddComponent<Button>();
            btn.transition = Selectable.Transition.None; // Sin parpadeo
            btn.onClick.AddListener(controller.OnClick); // Conectar al script de la rebanada

            // Guardar en lista para revisarla luego
            currentSlices.Add(controller);
        }
    }

    void CheckAnswer()
    {
        int paintedCount = 0;

        // Contar cuántas están azules
        foreach (SliceController slice in currentSlices)
        {
            if (slice.isSelected) paintedCount++;
        }

        Debug.Log("Pintaste: " + paintedCount + ". Necesarias: " + numerator);

        if (paintedCount == numerator)
        {
            StartCoroutine(ResultSequence(true));
        }
        else
        {
            StartCoroutine(ResultSequence(false));
        }
    }

    IEnumerator ResultSequence(bool success)
    {
        if (winLoseText)
        {
            winLoseText.gameObject.SetActive(true);
            winLoseText.text = success ? "¡CORRECTO!" : "¡INCORRECTO!";
            winLoseText.color = success ? Color.green : Color.red;
        }

        checkButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        checkButton.interactable = true;

        if (success)
        {
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel);
            currentLevel++;
            StartLevel();
        }
        else
        {
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel);
            StartCoroutine(GameOverSequence());
        }
    }

    IEnumerator GameOverSequence()
    {
        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (gameOverText) gameOverText.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (gameOverText) gameOverText.SetActive(false);
        currentLevel = 1;
        StartLevel();
    }
}