using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManagueJ9 : MonoBehaviour
{
    [Header("UI Referencias")]
    public TextMeshProUGUI textoPregunta;
    public TextMeshProUGUI textoRespuestaUsuario;
    public TextMeshProUGUI textoPuntaje;
    public Image barraTiempo;
    public GameObject[] iconosVidas; // Arrastra aquí las 3 imagenes de vacas
    public GameObject panelGameOver; // Un panel que diga "Perdiste" (opcional)

    [Header("Configuración Juego")]
    public float tiempoMaximo = 10f;

    private int puntajeActual = 0;
    private int vidas = 3;
    private float tiempoRestante;
    private int resultadoCorrecto;
    private string respuestaActual = "";
    private bool juegoActivo = true;

    void Start()
    {
        StartCoroutine(LogicaTiempo());
        GenerarNuevaOperacion();
        ActualizarUI();
    }

    void Update()
    {
        // Actualizar visualmente la barra de tiempo
        if (juegoActivo)
        {
            barraTiempo.fillAmount = tiempoRestante / tiempoMaximo;
        }
    }

    // --- Lógica del Juego ---

    void GenerarNuevaOperacion()
    {
        // Genera números aleatorios (tablas del 1 al 10)
        int a = Random.Range(1, 11);
        int b = Random.Range(1, 11);

        resultadoCorrecto = a * b;
        textoPregunta.text = a.ToString() + " X " + b.ToString();

        // Resetear tiempo y respuesta
        tiempoRestante = tiempoMaximo;
        respuestaActual = "";
        textoRespuestaUsuario.text = "";
    }

    IEnumerator LogicaTiempo()
    {
        while (juegoActivo)
        {
            yield return new WaitForSeconds(0.1f);
            tiempoRestante -= 0.1f;

            if (tiempoRestante <= 0)
            {
                PerderVida();
                GenerarNuevaOperacion(); // Pasamos a la siguiente aunque no responda
            }
        }
    }

    public void PerderVida()
    {
        vidas--;

        // Ocultar una vaca
        if (vidas >= 0 && vidas < iconosVidas.Length)
        {
            iconosVidas[vidas].SetActive(false);
        }

        if (vidas <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        juegoActivo = false;
        Debug.Log("Juego Terminado");

        // Guardar Puntaje
        if (DataManager.Instance != null)
        {
            DataManager.Instance.GuardarMaxPuntuacion(puntajeActual);
        }

        if (panelGameOver != null) panelGameOver.SetActive(true);
    }

    // --- Métodos para los Botones (UI) ---

    // Asigna este método a los botones 0-9. En el inspector, escribe el número en el parámetro.
    public void PresionarNumero(string numero)
    {
        if (!juegoActivo) return;
        if (respuestaActual.Length < 4) // Límite de caracteres
        {
            respuestaActual += numero;
            textoRespuestaUsuario.text = respuestaActual;
        }
    }

    // Asigna al botón "Borrar"
    public void Borrar()
    {
        if (!juegoActivo) return;
        respuestaActual = "";
        textoRespuestaUsuario.text = respuestaActual;
    }

    // Asigna al botón "Confirmar"
    public void Confirmar()
    {
        if (!juegoActivo || string.IsNullOrEmpty(respuestaActual)) return;

        int respuestaInt = int.Parse(respuestaActual);

        if (respuestaInt == resultadoCorrecto)
        {
            // Respuesta Correcta
            puntajeActual += 10; // O lo que quieras sumar
            GenerarNuevaOperacion();
        }
        else
        {
            // Respuesta Incorrecta
            PerderVida();
            respuestaActual = "";
            textoRespuestaUsuario.text = "";
            // Opcional: Generar nueva operación o dejar que intente de nuevo
            GenerarNuevaOperacion();
        }
        ActualizarUI();
    }

    void ActualizarUI()
    {
        textoPuntaje.text = "Puntaje: " + puntajeActual.ToString();
    }
}
