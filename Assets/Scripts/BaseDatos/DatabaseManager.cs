using UnityEngine;
using SQLite;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;
    private SQLiteConnection _connection;
    private string _dbPath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            IniciarBaseDeDatos();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void IniciarBaseDeDatos()
    {
        string fileName = "EscuelaGames.db";
        _dbPath = Path.Combine(Application.persistentDataPath, fileName);

        _connection = new SQLiteConnection(_dbPath);

        // Crear tablas si no existen
        _connection.CreateTable<Usuario>();
        _connection.CreateTable<ProgresoJuego>();

        Debug.Log("Base de datos iniciada en: " + _dbPath);
        CrearProfesorDefault();
    }

    // --- GESTIÓN DE USUARIOS ---

    private void CrearProfesorDefault()
    {
        // Verificar si ya existe el profe
        var profe = _connection.Table<Usuario>().FirstOrDefault(u => u.EsProfesor);
        if (profe == null)
        {
            var nuevoProfe = new Usuario
            {
                Nombre = "Profesor",
                EsProfesor = true,
                Password = "admin", // Contraseña simple
                GlobalId = System.Guid.NewGuid().ToString(),
                FechaRegistro = System.DateTime.Now
            };
            _connection.Insert(nuevoProfe);
            Debug.Log("Profesor creado por defecto (Pass: admin)");
        }
    }

    public bool RegistrarAlumno(string nombre, out Usuario usuarioCreado)
    {
        usuarioCreado = null;

        // Validar duplicados (Case Insensitive)
        var existente = _connection.Table<Usuario>()
                            .FirstOrDefault(u => u.Nombre.ToLower() == nombre.ToLower() && !u.EsProfesor);

        if (existente != null) return false; // Ya existe

        usuarioCreado = new Usuario
        {
            Nombre = nombre,
            EsProfesor = false,
            GlobalId = System.Guid.NewGuid().ToString(), // ID único para el futuro online
            FechaRegistro = System.DateTime.Now
        };

        _connection.Insert(usuarioCreado);
        return true;
    }

    public Usuario Login(string nombre, string password = "")
    {
        if (nombre.ToLower() == "profesor")
        {
            return _connection.Table<Usuario>().FirstOrDefault(u => u.EsProfesor && u.Password == password);
        }
        else
        {
            return _connection.Table<Usuario>().FirstOrDefault(u => u.Nombre.ToLower() == nombre.ToLower() && !u.EsProfesor);
        }
    }

    // --- GESTIÓN DE JUEGO (Lo que usa tu GameManager) ---

    // Guardar o Actualizar Progreso
    public void GuardarProgreso(int usuarioId, string gameID, int puntajeSumar, int nivelAlcanzado)
    {
        // Buscamos si ya jugó este juego antes
        var progreso = _connection.Table<ProgresoJuego>()
                            .FirstOrDefault(p => p.UsuarioId == usuarioId && p.GameID == gameID);

        if (progreso == null)
        {
            // Primer registro
            progreso = new ProgresoJuego
            {
                UsuarioId = usuarioId,
                GameID = gameID,
                NivelMaximo = nivelAlcanzado,
                PuntajeAcumulado = puntajeSumar,
                Sincronizado = false,
                UltimaJugada = System.DateTime.Now
            };
            _connection.Insert(progreso);
        }
        else
        {
            // Actualizar existente
            progreso.PuntajeAcumulado += puntajeSumar; // Acumulamos puntos
            if (nivelAlcanzado > progreso.NivelMaximo)
            {
                progreso.NivelMaximo = nivelAlcanzado;
            }
            progreso.Sincronizado = false; // Marcamos para subir a la nube luego
            progreso.UltimaJugada = System.DateTime.Now;

            _connection.Update(progreso);
        }
    }

    // Cargar nivel para configurar dificultad
    public int LoadLevel(int usuarioId, string gameID)
    {
        var progreso = _connection.Table<ProgresoJuego>()
                            .FirstOrDefault(p => p.UsuarioId == usuarioId && p.GameID == gameID);

        return progreso != null ? progreso.NivelMaximo : 1; // Si no existe, devuelve nivel 1
    }

    // --- PARA EL PROFESOR ---
    // Obtiene lista de todos los alumnos
    public List<Usuario> ObtenerAlumnos()
    {
        return _connection.Table<Usuario>().Where(u => !u.EsProfesor).ToList();
    }

    // Obtiene estadísticas de un alumno específico
    public List<ProgresoJuego> ObtenerStatsAlumno(int alumnoId)
    {
        return _connection.Table<ProgresoJuego>().Where(p => p.UsuarioId == alumnoId).ToList();
    }
}