using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CajaRespuesta : MonoBehaviour, IPointerClickHandler
{
    public int carrilAsignado;   // 0 izquierda, 1 centro, 2 derecha
    public int valorRespuesta;   // número que muestra esta caja

    private Movimientojugador jugador;
    private MathManager math;

    void Start()
    {
        jugador = FindObjectOfType<Movimientojugador>();
        math = FindObjectOfType<MathManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Caja clickeada: " + valorRespuesta);

        if (jugador == null || math == null) return;

        // 1?? Verificar si el jugador está en el carril correcto
        if (jugador.GetCarrilActual() != carrilAsignado)
        {
            jugador.Ralentizar();
            jugador.QuitarVida();
            Debug.Log("? NO ESTAS EN EL MISMO CARRIL");
            return;
        }

        // 2?? Verificar si la respuesta es correcta
        if (math.ComprobarRespuesta(valorRespuesta))
        {
            Debug.Log("?? RESPUESTA CORRECTA ? TURBO");
            jugador.StartCoroutine(jugador.ActivateTurbo(2f));
        }
        else
        {
            Debug.Log("? RESPUESTA INCORRECTA");
            jugador.Ralentizar();
            jugador.QuitarVida();
        }

        // 3?? Nueva operación
        math.GenerarOperacion();
    }
}