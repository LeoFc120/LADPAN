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

    private int carrilActual = 0;
    private Vector3 posicionObjetivo;

    [Header("Control del movimiento")]
    public bool puedeMoverse = false; // ← Controla si puede moverse (inicia en false)

    void Start()
    {
        if (carriles.Length == 0)
        {
            Debug.LogError("No se asignaron carriles en el array 'carriles'.");
            enabled = false;
            return;
        }

        carrilActual = Mathf.Clamp(carrilActual, 0, carriles.Length - 1);
        posicionObjetivo = carriles[carrilActual].position;
        transform.position = new Vector3(posicionObjetivo.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (!puedeMoverse || carriles.Length == 0) return;

        // Movimiento hacia adelante
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

        // Movimiento suave entre carriles
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

    public int GetCarrilActual()
    {
        return carrilActual;
    }

    // 🔥 TURBO: duplicar velocidad por unos segundos
    public IEnumerator ActivateTurbo(float duration)
    {
        float originalSpeed = velocidadAvance;
        velocidadAvance *= 2f; // duplica velocidad
        yield return new WaitForSeconds(duration);
        velocidadAvance = originalSpeed;
    }
}
