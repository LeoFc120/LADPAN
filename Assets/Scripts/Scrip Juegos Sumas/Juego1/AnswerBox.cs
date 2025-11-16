using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnswerBox : MonoBehaviour
{
    public Movimientojugador jugador;
    public bool correcta = false;
    public int carrilAsignado;   // 0,1,2 según carril
    public float velocidadSubida = 12f;

    void Update()
    {
        // Subir hacia el jugador
        transform.Translate(Vector3.back * velocidadSubida * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        int carrilJugador = jugador.GetCarrilActual();

        if (carrilJugador == carrilAsignado)
        {
            // Está en el carril correcto → evaluar respuesta
            if (correcta)
            {
                Debug.Log("✔ Turbo Activado");
                StartCoroutine(jugador.ActivateTurbo(2f));
            }
            else
            {
                Debug.Log("❌ Incorrecta → Ralentizar");
                jugador.Ralentizar();
                jugador.QuitarVida();
            }
        }
        else
        {
            Debug.Log("Jugador no estaba en este carril → ignorado");
        }

        Destroy(gameObject);
    }
}
