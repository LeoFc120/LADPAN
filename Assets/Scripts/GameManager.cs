using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control general del juego y punto de conexión con SQLite.
/// Mantiene los puntos, nivel actual y referencias generales.
/// </summary>
public class GameManager : MonoBehaviour
{
    // 🔹 Patrón Singleton para acceder desde cualquier script (ejemplo: GameManager.Instancia)
    public static GameManager Instancia;

    [Header("Datos del jugador")]
    public int puntosActuales = 0;
    public int nivelActual = 1;
    public string nombreJugador = "Jugador";

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject); // persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔹 Método que tus minijuegos llamarán
    public void SumarPuntos(int cantidad)
    {
        puntosActuales += cantidad;
        Debug.Log($"Puntos actuales: {puntosActuales}");

        // 👉 Aquí tu compañero conectará SQLite:
        // Ejemplo:
        // SQLiteManager.GuardarPuntos(nombreJugador, puntosActuales);
    }

    // 🔹 Método para restar puntos (o penalizaciones)
    public void RestarPuntos(int cantidad)
    {
        puntosActuales -= cantidad;
        if (puntosActuales < 0) puntosActuales = 0;
        Debug.Log($"Puntos actuales: {puntosActuales}");

        // 👉 Aquí también puede actualizarse SQLite
    }

    // 🔹 Llamado al finalizar un minijuego
    public void GuardarProgreso()
    {
        Debug.Log($"Progreso guardado: Nivel {nivelActual}, Puntos {puntosActuales}");

        // 👉 Aquí tu compañero agregará el método SQLite para registrar nivel y puntos
        // SQLiteManager.GuardarProgreso(nombreJugador, nivelActual, puntosActuales);
    }

    // 🔹 Método para reiniciar puntos (si reinician minijuego)
    public void ReiniciarPuntos()
    {
        puntosActuales = 0;
        Debug.Log("Puntos reiniciados.");

        // 👉 Aquí también puede limpiar los datos en SQLite
    }
}

