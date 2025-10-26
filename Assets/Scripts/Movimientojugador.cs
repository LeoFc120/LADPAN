using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimientojugador : MonoBehaviour
{
    [Header("Carriles")]
    public Transform[] carriles; // Carril1, Carril2, Carril3

    [Header("Velocidades")]
    public float velocidadCambio = 8f;
    public float velocidadAvance = 10f;

    private int carrilActual = 0; // Inicio en carril 1 (índice 0)
    private Vector3 posicionObjetivo;

    void Start()
    {
        if (carriles.Length == 0)
        {
            Debug.LogError("No se asignaron carriles en el array 'carriles'.");
            enabled = false; // Desactiva el script si no hay carriles
            return;
        }

        // Posición inicial en el carril 1
        carrilActual = Mathf.Clamp(carrilActual, 0, carriles.Length - 1);
        posicionObjetivo = carriles[carrilActual].position;
        transform.position = new Vector3(posicionObjetivo.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (carriles.Length == 0) return;

        // Movimiento hacia adelante constante
        transform.Translate(Vector3.forward * velocidadAvance * Time.deltaTime);

        // Controles táctiles
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch toque = Input.GetTouch(0);
            if (toque.position.x < Screen.width / 2)
                MoverIzquierda();
            else
                MoverDerecha();
        }

        // Controles por teclado
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoverIzquierda();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoverDerecha();

        // Movimiento suave hacia el carril destino
        posicionObjetivo = carriles[Mathf.Clamp(carrilActual, 0, carriles.Length - 1)].position;
        Vector3 nuevaPos = new Vector3(posicionObjetivo.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, nuevaPos, Time.deltaTime * velocidadCambio);
    }

    void MoverIzquierda()
    {
        if (carrilActual > 0)
            carrilActual--;
    }

    void MoverDerecha()
    {
        if (carrilActual < carriles.Length - 1)
            carrilActual++;
    }

    // Getter para que los CPU puedan conocer el carril del jugador
    public int GetCarrilActual()
    {
        return carrilActual;
    }
}
