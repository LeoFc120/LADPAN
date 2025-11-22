using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FractionDrawManager : MonoBehaviour
{
    [Header("UI Textos")]
    public Text instructionText; // "Corta en: 1/2"
    public Text hintText;        // NUEVO: "Usa X líneas"
    public Text levelText;
    public Text winLoseText;
    public GameObject gameOverText;

    [Header("Zona de Dibujo")]
    public RectTransform drawingArea; // La imagen del plato
    public GameObject linePrefab;     // Prefab línea (Pivot X=0, Y=0.5)
    public Transform linesContainer;  // Contenedor (el plato)

    [Header("Botones")]
    public Button checkButton; // TRAZAR
    public Button cleanButton; // BORRAR

    [Header("Base de Datos")]
    public LevelSaver databaseScript;

    // Variables de Juego
    private int currentLevel = 1;
    private int targetParts;
    private int linesNeeded; // NUEVO: Para guardar cuántas líneas se esperan
    private List<GameObject> drawnLines = new List<GameObject>();

    // Variables para el Dibujo
    private GameObject currentLine;
    private bool isDrawing = false;
    private Vector2 startPoint;

    void Start()
    {
        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (gameOverText) gameOverText.SetActive(false);

        if (checkButton) checkButton.onClick.AddListener(CheckAnswer);
        if (cleanButton) cleanButton.onClick.AddListener(ClearLines);

        StartLevel();
    }

    void Update()
    {
        if (checkButton != null && checkButton.interactable)
        {
            HandleDrawingInput();
        }
    }

    void HandleDrawingInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(drawingArea, Input.mousePosition))
            {
                StartDrawing();
            }
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            UpdateCurrentLine();
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            FinishDrawing();
        }
    }

    void StartDrawing()
    {
        isDrawing = true;
        startPoint = Input.mousePosition;
        currentLine = Instantiate(linePrefab, linesContainer);
        currentLine.transform.position = startPoint;

        RectTransform rt = currentLine.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, rt.sizeDelta.y);

        if (currentLine.GetComponent<Image>()) currentLine.GetComponent<Image>().raycastTarget = false;
    }

    void UpdateCurrentLine()
    {
        if (currentLine == null) return;
        Vector2 currentPos = Input.mousePosition;
        Vector2 direction = currentPos - startPoint;
        float distance = direction.magnitude;

        RectTransform rt = currentLine.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(distance, rt.sizeDelta.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FinishDrawing()
    {
        isDrawing = false;
        RectTransform rt = currentLine.GetComponent<RectTransform>();

        if (rt.sizeDelta.x < 10) Destroy(currentLine);
        else
        {
            if (currentLine.GetComponent<Image>()) currentLine.GetComponent<Image>().raycastTarget = true;
            drawnLines.Add(currentLine);
        }
        currentLine = null;
    }

    void StartLevel()
    {
        ClearLines();
        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (levelText) levelText.text = "Level: " + currentLevel;

        int[] validFractions = { 2, 4, 6, 8 };
        targetParts = validFractions[Random.Range(0, validFractions.Length)];

        // CALCULAR PISTA DE LÍNEAS
        if (targetParts == 2) linesNeeded = 1;
        else if (targetParts == 4) linesNeeded = 2;
        else if (targetParts == 6) linesNeeded = 3;
        else if (targetParts == 8) linesNeeded = 4;

        // Actualizar Textos
        if (instructionText) instructionText.text = "Corta en: 1/" + targetParts;

        // AQUÍ MOSTRAMOS LA AYUDA
        if (hintText) hintText.text = "(Usa " + linesNeeded + " líneas)";
    }

    void ClearLines()
    {
        foreach (GameObject line in drawnLines) Destroy(line);
        if (currentLine != null) Destroy(currentLine);
        drawnLines.Clear();
    }

    void CheckAnswer()
    {
        int linesDrawn = drawnLines.Count;
        bool isCorrect = false;

        // Validación estricta usando la variable linesNeeded
        if (linesDrawn == linesNeeded) isCorrect = true;

        // Excepción: 1/4 también se puede hacer con 3 líneas paralelas (aunque es raro)
        if (targetParts == 4 && linesDrawn == 3) isCorrect = true;

        if (isCorrect)
        {
            Debug.Log("¡Correcto!");
            StartCoroutine(NextLevelSequence(true));
        }
        else
        {
            Debug.Log("Incorrecto.");
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