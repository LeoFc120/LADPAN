using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoCPU : MonoBehaviour
{
    [Header("Carriles")]
    public Transform[] carriles;

    private Movimientojugador jugador;

    private int carrilActual = 0;
    private Vector3 posicionObjetivo;

    [Header("Velocidades")]
    public float velocidadAvance = 8f;
    public float velocidadCambio = 5f;

    private int ultimoCarrilJugador;

    void Start()
    {
        jugador = FindObjectOfType<Movimientojugador>();

        if (jugador == null)
            Debug.LogWarning("⚠ No se encontró Movimientojugador en la escena.");

        if (carriles == null || carriles.Length == 0)
        {
            Debug.LogError("❌ MovimientoCPU: No hay carriles asignados.");
            enabled = false;
            return;
        }

        carrilActual = ObtenerCarrilInicial();
        posicionObjetivo = carriles[carrilActual].position;

        ultimoCarrilJugador = jugador != null ? jugador.GetCarrilActual() : carrilActual;
    }

    void Update()
    {
        // Mantener ritmo del jugador
        if (jugador != null)
            velocidadAvance = jugador.velocidadAvance;

        // Avance
        transform.Translate(Vector3.forward * velocidadAvance * Time.deltaTime);

        // Movimiento lateral suave
        posicionObjetivo = carriles[carrilActual].position;
        Vector3 nuevaPos = new Vector3(posicionObjetivo.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, nuevaPos, Time.deltaTime * velocidadCambio);

        if (jugador == null) return;

        int carrilJugadorActual = jugador.GetCarrilActual();

        // Cambio de carril solo si el jugador cambia
        if (carrilJugadorActual != ultimoCarrilJugador)
        {
            CambiarCarrilSegunJugador(carrilJugadorActual);
            ultimoCarrilJugador = carrilJugadorActual;
        }
    }

    void CambiarCarrilSegunJugador(int carrilJugador)
    {
        List<int> opciones = new List<int>();

        for (int i = 0; i < carriles.Length; i++)
        {
            if (i != carrilJugador && !CarrilOcupado(i))
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
        MovimientoCPU[] CPUs = FindObjectsOfType<MovimientoCPU>();

        foreach (var cpu in CPUs)
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
            tiempo += Time.deltaTime * velocidadCambio;
            yield return null;
        }

        transform.position = destino;
    }

    int ObtenerCarrilInicial()
    {
        int mejor = 0;
        float distMin = Mathf.Infinity;

        for (int i = 0; i < carriles.Length; i++)
        {
            float distancia = Mathf.Abs(transform.position.x - carriles[i].position.x);
            if (distancia < distMin)
            {
                distMin = distancia;
                mejor = i;
            }
        }
        return mejor;
    }
}
