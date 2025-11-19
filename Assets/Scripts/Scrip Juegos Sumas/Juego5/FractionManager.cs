using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FractionManager : MonoBehaviour
{
    [Header("UI Textos")]
    public Text partsText;
    public Text linesText;
    public Text levelText;
    public Text winLoseText; // Texto de "¡Correcto!" o "¡Incorrecto!"
    public GameObject gameOverText; // Texto de Game Over global

    [Header("UI Plato")]
    public Image plateImage; // La imagen del plato
    public GameObject divideButton; // El botón de dividir

    [Header("Sprites de División")]
    // Aquí arrastrarás tus sprites de líneas si los tienes.
    // Ej: spriteLinea1 para 1/2, spriteLineaCruzada para 1/4
    public Sprite[] divisionSprites; // Guarda sprites para 1/2, 1/3, 1/4, etc.

    [Header("Base de Datos")]
    public LevelSaver databaseScript;

    // Variables de Juego
    private int currentLevel = 1;
    private int targetParts; // Ejemplo: 2 para 1/2, 3 para 1/3
    private int requiredLines; // Líneas necesarias para dividir (1 para 1/2, 2 para 1/3)
    private int totalPartsAvailable; // Las partes disponibles en el plato (p.ej. 8 si es un círculo)

    void Start()
    {
        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (gameOverText) gameOverText.SetActive(false);

        // Conectar el botón al script
        if (divideButton) divideButton.GetComponent<Button>().onClick.AddListener(CheckDivision);

        StartLevel();
    }

    void StartLevel()
    {
        // Reiniciar UI
        if (winLoseText) winLoseText.gameObject.SetActive(false);
        if (levelText) levelText.text = "Level: " + currentLevel;

        // DIFICULTAD
        // Nivel 1: dividir en 2 o 3 (1 ó 2 líneas)
        // Nivel 2: dividir en 2, 3 o 4 (1, 2 ó 3 líneas)
        // Nivel 3: dividir en 2, 3, 4 o 5
        int maxParts = 2 + currentLevel; // Empieza con 2+1=3, luego 2+2=4, etc.
        maxParts = Mathf.Min(maxParts, 8); // No más de 8 divisiones por ahora (si no tienes sprites)

        targetParts = Random.Range(2, maxParts + 1); // Queremos dividir en 2, 3, 4, etc.
        requiredLines = targetParts - 1; // Para dividir en N partes, necesitas N-1 líneas

        totalPartsAvailable = 8; // Podríamos asumir que el plato se puede dividir en 8 por ejemplo

        UpdateUI();
        // Mostrar el plato sin divisiones (o con la inicial si la tienes)
        if (plateImage && divisionSprites.Length > 0) plateImage.sprite = null; // O el sprite del plato entero
    }

    void UpdateUI()
    {
        if (partsText) partsText.text = "Parts: 1/" + targetParts;
        if (linesText) linesText.text = "Lines: " + requiredLines;
        if (levelText) levelText.text = "Level: " + currentLevel;

        // Mostrar la división correcta si tenemos el sprite
        if (plateImage && divisionSprites != null && divisionSprites.Length > 0)
        {
            // Opcional: Mostrar el sprite de la división correcta al iniciar,
            // o dejarlo vacío para que el jugador trace.
            // Por ahora, lo dejamos vacío para que el jugador "decida".
        }
    }

    void CheckDivision()
    {
        // En este juego, el jugador "traza" al hacer clic en el botón.
        // La dificultad está en saber CUÁNTAS líneas son necesarias.

        // Por ahora, simplemente si el jugador le da al botón, asume que "trazó"
        // y lo comparamos con las líneas requeridas.

        // Si el jugador tuviera que trazar manualmente, la lógica sería diferente.

        // Asumiendo que el botón "DIVIDIR" crea UNA línea a la vez:
        // Por simplicidad, el botón "DIVIDIR" simulará que el jugador ha hecho el número correcto de líneas.
        // Si el juego fuera interactivo, el jugador contaría cuántas líneas traza.

        // Por ahora, el jugador debe decidir si el número en 'LinesText' es el correcto.
        // Simplificado: si el jugador pulsa el botón, siempre será "correcto" si las líneas mostradas eran las requeridas.
        // Una mecánica más avanzada sería que el jugador TOCA N veces el plato para añadir N líneas.

        // Para esta primera versión funcional:
        // El jugador tiene que interpretar "Parts: 1/X" y saber que necesita "X-1" líneas.
        // Al pulsar el botón, comprobamos si la "línea" que simulamos poner es la correcta.

        // Vamos a hacer que el botón "DIVIDIR" sea el botón de "Comprobar".
        // El jugador ve "Parts: 1/2" y "Lines: 1". Si le da a dividir, es correcto.
        // Si ve "Parts: 1/4" y "Lines: 3" y le da a dividir, es correcto.

        Debug.Log("Jugador intentó dividir. Partes objetivo: " + targetParts + ", Líneas requeridas: " + requiredLines);

        // Para esta implementación:
        // Si la instrucción es "Parts: 1/X" y el botón es "DIVIDIR",
        // asumimos que el jugador está diciendo: "Divido en X partes".

        bool isCorrect = true; // Por ahora, si le da al botón, es correcto
                               // Luego podemos añadir más complejidad (ej. botones para 1, 2, 3 líneas)

        if (isCorrect)
        {
            Debug.Log("¡División Correcta!");
            StartCoroutine(ShowResultAndNextLevel(true));
        }
        else
        {
            Debug.Log("División Incorrecta - Game Over");
            StartCoroutine(ShowResultAndNextLevel(false));
        }
    }

    IEnumerator ShowResultAndNextLevel(bool success)
    {
        if (winLoseText)
        {
            winLoseText.gameObject.SetActive(true);
            winLoseText.text = success ? "¡CORRECTO!" : "¡INCORRECTO!";
            winLoseText.color = success ? Color.green : Color.red;
        }

        if (divideButton) divideButton.GetComponent<Button>().interactable = false; // Desactivar botón

        yield return new WaitForSeconds(1.5f); // Esperar para que el jugador vea el resultado

        if (divideButton) divideButton.GetComponent<Button>().interactable = true; // Reactivar botón

        if (success)
        {
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel); // Guardar progreso antes de avanzar
            currentLevel++;
            StartLevel();
        }
        else
        {
            if (databaseScript != null) databaseScript.SaveProgress(currentLevel); // Guardar progreso antes de reiniciar
            StartCoroutine(GameOverSequence());
        }
    }

    IEnumerator GameOverSequence()
    {
        if (winLoseText) winLoseText.gameObject.SetActive(false); // Esconder "Incorrecto"
        if (gameOverText)
        {
            gameOverText.GetComponent<Text>().text = "GAME OVER";
            gameOverText.SetActive(true);
        }
        yield return new WaitForSeconds(2f);
        if (gameOverText) gameOverText.SetActive(false);
        currentLevel = 1;
        StartLevel();
    }
}
