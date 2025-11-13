using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurboManager : MonoBehaviour
{
    public TMP_Text turboText;          // Texto en pantalla que muestra la cantidad de turbos
    public Movimientojugador player;    // Referencia al script del jugador

    private int turbos = 0;             // Contador de turbos
    private bool turboActivo = false;   // Controla si el turbo está activo
    public float duracionTurbo = 2f;    // Duración del turbo
    public float multiplicadorVelocidad = 2f; // Cuánto aumenta la velocidad

    void Start()
    {
        ActualizarTexto();
    }

    // Este método es el que llama AnswerBox
    public void AgregarTurbo()
    {
        turbos++;
        ActualizarTexto();
        StartCoroutine(UsarTurbo());
    }

    private IEnumerator UsarTurbo()
    {
        if (turboActivo) yield break;

        turboActivo = true;
        float velocidadOriginal = player.velocidadAvance;

        // Aumentar velocidad temporalmente
        player.velocidadAvance *= multiplicadorVelocidad;
        Debug.Log("🚀 Turbo activado!");

        yield return new WaitForSeconds(duracionTurbo);

        // Volver a la velocidad original
        player.velocidadAvance = velocidadOriginal;
        turboActivo = false;
        Debug.Log("⛽ Turbo terminado!");
    }

    private void ActualizarTexto()
    {
        if (turboText != null)
            turboText.text = "Turbos: " + turbos;
    }
}