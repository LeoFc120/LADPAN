using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurboManager : MonoBehaviour
{
    public TMP_Text turboText;          // Texto que muestra cantidad de turbos (opcional)
    public Movimientojugador player;    // Referencia al script del jugador

    private int turbos = 0;
    private bool turboActivo = false;

    public float duracionTurbo = 2f;
    public float multiplicadorVelocidad = 2f;

    void Start()
    {
        ActualizarTexto();
    }

    // Llamado cuando la respuesta es correcta
    public void ActivarTurbo()
    {
        turbos++;
        ActualizarTexto();
        StartCoroutine(TurboRoutine());
    }

    private IEnumerator TurboRoutine()
    {
        if (turboActivo) yield break;

        turboActivo = true;

        float velocidadOriginal = player.velocidadAvance;
        player.velocidadAvance *= multiplicadorVelocidad;

        Debug.Log("🚀 Turbo ACTIVADO!");

        yield return new WaitForSeconds(duracionTurbo);

        player.velocidadAvance = velocidadOriginal;
        turboActivo = false;

        Debug.Log("⛽ Turbo FINALIZADO");
    }

    private void ActualizarTexto()
    {
        if (turboText != null)
            turboText.text = "Turbos: " + turbos;
    }
}
