using System;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public static class SQLiteHelper {
    public static string DBPath {
        get {
            string path = Path.Combine(Application.persistentDataPath, "MiJuego.db");
            return path;
        }
    }

    public static void CreateDBIfNotExists() {
        if (!File.Exists(DBPath)) {
            SqliteConnection.CreateFile(DBPath);
            using (var conn = new SqliteConnection("URI=file:" + DBPath)) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Usuario (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nombre TEXT, Edad INTEGER, Fecha TEXT);";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
