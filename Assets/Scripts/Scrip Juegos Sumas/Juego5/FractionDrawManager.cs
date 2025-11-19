using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FractionDrawManager : MonoBehaviour
{
    [Header("UI Textos")]
    public Text instructionText; // "Corta en: 1/2"
    public Text levelText;
    public Text winLoseText;
    public GameObject gameOverText;

    [Header("Zona de Dibujo")]
    public RectTransform drawingArea; // Arrastra aquí la imagen del plato (debe tener RaycastTarget)
    public GameObject linePrefab;     // Tu prefab de la línea (Pivot X=0)
    public Transform linesContainer;  // Arrastra aquí el plato también

    [Header("Botones")]
    public Button checkButton; // Botón TRAZAR
    public Button cleanButton; // Botón BORRAR

    [Header("Base de Datos")]
    public LevelSaver databaseScript;

    // Variables de Juego
    private int currentLevel = 1;
    private int targetParts;       // En cuántas partes hay que dividir (2, 4, 6, 8)
    private List<GameObject> drawnLines = new List<GameObject>();

    // Variables para el Dibujo
    private GameObject currentLine;
    private bool isDrawing = false;
    private Vector2 startPoint;

    void Start()
    {
        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (gameOverText) gameOverText.SetActive(false);

        // Conectar botones
        if (checkButton) checkButton.onClick.AddListener(CheckAnswer);
        if (cleanButton) cleanButton.onClick.AddListener(ClearLines);

        StartLevel();
    }

    void Update()
    {
        // Detectar dibujo solo si el botón de comprobar está activo (no estamos en pausa/animación)
        if (checkButton != null && checkButton.interactable)
        {
            HandleDrawingInput();
        }
    }

    // --- LÓGICA DE DIBUJO ---
    void HandleDrawingInput()
    {
        // 1. Clic inicial
        if (Input.GetMouseButtonDown(0))
        {
            // Solo dibujar si el clic está dentro del plato
            if (RectTransformUtility.RectangleContainsScreenPoint(drawingArea, Input.mousePosition))
            {
                StartDrawing();
            }
        }

        // 2. Arrastrar
        if (Input.GetMouseButton(0) && isDrawing)
        {
            UpdateCurrentLine();
        }

        // 3. Soltar
        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            FinishDrawing();
        }
    }

    void StartDrawing()
    {
        isDrawing = true;
        startPoint = Input.mousePosition;

        // Crear línea
        currentLine = Instantiate(linePrefab, linesContainer);
        currentLine.transform.position = startPoint;

        // Inicializar tamaño en 0
        RectTransform rt = currentLine.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, rt.sizeDelta.y);

        // Desactivar raycast de la línea mientras dibujas para que no estorbe
        if (currentLine.GetComponent<Image>()) currentLine.GetComponent<Image>().raycastTarget = false;
    }

    void UpdateCurrentLine()
    {
        if (currentLine == null) return;

        Vector2 currentPos = Input.mousePosition;
        Vector2 direction = currentPos - startPoint;
        float distance = direction.magnitude;

        RectTransform rt = currentLine.GetComponent<RectTransform>();

        // Estirar
        rt.sizeDelta = new Vector2(distance, rt.sizeDelta.y);

        // Rotar hacia el mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FinishDrawing()
    {
        isDrawing = false;

        RectTransform rt = currentLine.GetComponent<RectTransform>();
        // Si la línea es muy corta (un error o un clic sin arrastrar), la borramos
        if (rt.sizeDelta.x < 10)
        {
            Destroy(currentLine);
        }
        else
        {
            // Reactivar raycast por si acaso (opcional)
            if (currentLine.GetComponent<Image>()) currentLine.GetComponent<Image>().raycastTarget = true;
            drawnLines.Add(currentLine);
        }
        currentLine = null;
    }

    // --- LÓGICA DEL JUEGO ---

    void StartLevel()
    {
        ClearLines();
        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (levelText) levelText.text = "Level: " + currentLevel;

        // LÓGICA DE PIZZA: Solo permitimos números pares fáciles de dibujar cruzando el centro
        int[] validFractions = { 2, 4, 6, 8 };

        // Elegimos uno al azar
        targetParts = validFractions[Random.Range(0, validFractions.Length)];

        if (instructionText) instructionText.text = "Corta en: 1/" + targetParts;
    }

    void ClearLines()
    {
        foreach (GameObject line in drawnLines)
        {
            Destroy(line);
        }
        if (currentLine != null) Destroy(currentLine);
        drawnLines.Clear();
    }

    void CheckAnswer()
    {
        int linesDrawn = drawnLines.Count;
        bool isCorrect = false;

        // --- VALIDACIÓN TIPO PIZZA ---
        // 1/2 = 1 línea
        // 1/4 = 2 líneas (cruz)
        // 1/6 = 3 líneas (asterisco simple)
        // 1/8 = 4 líneas (asterisco doble)

        if (targetParts == 2 && linesDrawn == 1) isCorrect = true;
        else if (targetParts == 4 && linesDrawn == 2) isCorrect = true;
        else if (targetParts == 6 && linesDrawn == 3) isCorrect = true;
        else if (targetParts == 8 && linesDrawn == 4) isCorrect = true;

        // Validación extra para 1/4 si alguien hace 3 líneas paralelas (raro pero posible)
        else if (targetParts == 4 && linesDrawn == 3) isCorrect = true;

        if (isCorrect)
        {
            Debug.Log("¡Correcto!");
            StartCoroutine(NextLevelSequence(true));
        }
        else
        {
            Debug.Log("Incorrecto. Partes objetivo: " + targetParts + " | Líneas hechas: " + linesDrawn);
            StartCoroutine(NextLevelSequence(false));
        }
    }

    IEnumerator NextLevelSequence(bool success)
    {
        if (winLoseText)
        {
            winLoseText.gameObject.SetActive(true);
            winLoseText.text = success ? "¡CORRECTO!" : "¡INCORRECTO!";
            winLoseText.color = success ? Color.green : Color.red;
        }

        if (checkButton) checkButton.interactable = false;
        if (cleanButton) cleanButton.interactable = false;

        yield return new WaitForSeconds(1.5f);

        if (checkButton) checkButton.interactable = true;
        if (cleanButton) cleanButton.interactable = true;

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
        if (gameOverText)
        {
            if (gameOverText.GetComponent<Text>()) gameOverText.GetComponent<Text>().text = "GAME OVER";
            gameOverText.SetActive(true);
        }
        yield return new WaitForSeconds(2f);
        if (gameOverText) gameOverText.SetActive(false);
        currentLevel = 1;
        StartLevel();
    }
}