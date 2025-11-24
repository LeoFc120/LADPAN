using UnityEngine;
using SQLite; // Esta librería ahora funciona gracias al archivo SQLite.cs
using System.IO;

// 1. Definimos la estructura de la tabla aquí mismo
public class PlayerStats
{
    [PrimaryKey] // Esto le dice a la base de datos que este es el ID único
    public int Id { get; set; }
    public int MaxLevel { get; set; }
}

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private SQLiteConnection connection;

    void Awake()
    {
        // Definimos la ruta. En Android esto va a la memoria interna del juego.
        dbPath = Path.Combine(Application.persistentDataPath, "GameData.db");

        // Creamos la conexión
        connection = new SQLiteConnection(dbPath);

        // Este comando mágico crea la tabla si no existe automáticamente
        connection.CreateTable<PlayerStats>();

        // Verificamos si la tabla está vacía para crear el primer registro
        if (connection.Table<PlayerStats>().Count() == 0)
        {
            var newStats = new PlayerStats
            {
                Id = 1,
                MaxLevel = 1
            };
            connection.Insert(newStats);
            Debug.Log("Base de datos creada por primera vez.");
        }
    }

    public void SaveProgress(int level)
    {
        // Buscamos al jugador con Id 1
        var stats = connection.Table<PlayerStats>().Where(x => x.Id == 1).FirstOrDefault();

        if (stats != null)
        {
            // Solo guardamos si superó su récord
            if (level > stats.MaxLevel)
            {
                stats.MaxLevel = level;
                connection.Update(stats); // Actualizamos el registro en la DB
                Debug.Log("¡Progreso Guardado! Nuevo Nivel Máximo: " + level);
            }
        }
    }

    public int LoadLevel()
    {
        // Buscamos al jugador con Id 1
        var stats = connection.Table<PlayerStats>().Where(x => x.Id == 1).FirstOrDefault();

        if (stats != null)
        {
            return stats.MaxLevel;
        }
        return 1; // Si falla algo, devolvemos nivel 1 por defecto
    }

    // Es buena práctica cerrar la conexión cuando el objeto se destruye
    void OnDestroy()
    {
        if (connection != null)
        {
            connection.Close();
        }
    }
}