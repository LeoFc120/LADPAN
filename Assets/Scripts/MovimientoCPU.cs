using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoCPU : MonoBehaviour
{
    public Transform[] carriles; // Los mismos que usa el jugador
    private Movimientojugador jugador;
    private int carrilActual;
    private Vector3 posicionObjetivo;

    public float velocidadAvance = 8f;
    public float velocidadCambio = 5f;

    private int ultimoCarrilJugador;

    void Start()
    {
        // Buscar automáticamente al jugador
        jugador = FindObjectOfType<Movimientojugador>();
        if (jugador == null)
            Debug.LogWarning("No se encontró ningún jugador en la escena.");

        carrilActual = ObtenerCarrilInicial();
        posicionObjetivo = carriles[carrilActual].position;

        if (jugador != null)
            ultimoCarrilJugador = jugador.GetCarrilActual();
    }

    void Update()
    {
        // Ajustar velocidad al jugador
        if (jugador != null)
            velocidadAvance = jugador.velocidadAvance;

        // Movimiento hacia adelante
        transform.Translate(Vector3.forward * velocidadAvance * Time.deltaTime);

        // Movimiento suave entre carriles
        posicionObjetivo = carriles[carrilActual].position;
        Vector3 nuevaPos = new Vector3(posicionObjetivo.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, nuevaPos, Time.deltaTime * velocidadCambio);

        // Revisar si el jugador cambió de carril
        if (jugador != null)
        {
            int carrilJugadorActual = jugador.GetCarrilActual();
            if (carrilJugadorActual != ultimoCarrilJugador)
            {
                CambiarCarrilSegunJugador();
                ultimoCarrilJugador = carrilJugadorActual;
            }
        }
    }

    void CambiarCarrilSegunJugador()
    {
        // Obtener carriles vacíos
        List<int> opciones = new List<int>();
        for (int i = 0; i < carriles.Length; i++)
        {
            if (i != jugador.GetCarrilActual() && i != carrilActual && !CarrilOcupado(i))
                opciones.Add(i);
        }

        if (opciones.Count > 0)
        {
            int nuevoCarril = opciones[Random.Range(0, opciones.Count)];
            StartCoroutine(AnimacionRebase(carrilActual, nuevoCarril));
            carrilActual = nuevoCarril;
        }
    }

    bool CarrilOcupado(int indice)
    {
        // Evita que otro CPU esté en ese carril
        MovimientoCPU[] todosCPU = FindObjectsOfType<MovimientoCPU>();
        foreach (var cpu in todosCPU)
        {
            if (cpu != this && cpu.carrilActual == indice)
                return true;
        }
        return false;
    }

    IEnumerator AnimacionRebase(int desde, int hacia)
    {
        float tiempo = 0f;
        Vector3 inicio = transform.position;
        Vector3 destino = new Vector3(carriles[hacia].position.x, transform.position.y, transform.position.z);

        while (tiempo < 1f)
        {
            transform.position = Vector3.Lerp(inicio, destino, tiempo);
            tiempo += Time.deltaTime * velocidadCambio * 0.5f;
            yield return null;
        }
        transform.position = destino;
    }

    int ObtenerCarrilInicial()
    {
        int indiceMasCercano = 0;
        float distanciaMin = Mathf.Infinity;

        for (int i = 0; i < carriles.Length; i++)
        {
            float dist = Mathf.Abs(transform.position.x - carriles[i].position.x);
            if (dist < distanciaMin)
            {
                distanciaMin = dist;
                indiceMasCercano = i;
            }
        }
        return indiceMasCercano;
    }
}
