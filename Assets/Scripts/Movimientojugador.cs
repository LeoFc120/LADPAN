using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimientojugador : MonoBehaviour
{
    public Transform[] carriles; // Arrastra Carril_1, Carril_2, Carril_3
    private int carrilActual = 1; // Empieza en el carril central
    public float velocidadCambio = 8f;

    private Vector3 posicionObjetivo;

    void Start()
    {
        // Comenzamos en el carril del medio
        posicionObjetivo = carriles[carrilActual].position;
        transform.position = new Vector3(posicionObjetivo.x, transform.position.y, 0);
    }

    void Update()
    {
        // Movimiento táctil
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch toque = Input.GetTouch(0);

            if (toque.position.x < Screen.width / 2 && carrilActual > 0)
                carrilActual--; // Mueve a la izquierda
            else if (toque.position.x > Screen.width / 2 && carrilActual < carriles.Length - 1)
                carrilActual++; // Mueve a la derecha
        }

        // Movimiento con teclado (para pruebas en PC)
        if (Input.GetKeyDown(KeyCode.LeftArrow) && carrilActual > 0)
            carrilActual--;
        if (Input.GetKeyDown(KeyCode.RightArrow) && carrilActual < carriles.Length - 1)
            carrilActual++;

        // Mueve el auto suavemente al nuevo carril
        posicionObjetivo = carriles[carrilActual].position;
        transform.position = Vector3.Lerp(transform.position,
            new Vector3(posicionObjetivo.x, transform.position.y, 0),
            Time.deltaTime * velocidadCambio);
    }

}
