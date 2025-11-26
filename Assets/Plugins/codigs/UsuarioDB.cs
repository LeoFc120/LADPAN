using System;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public class UsuarioDB {
    public static void Registrar(string nombre,int edad,string fecha){
        SQLiteHelper.CreateDBIfNotExists();
        using(var conn=new SqliteConnection("URI=file:"+SQLiteHelper.DBPath)){
            conn.Open();
            using(var cmd=conn.CreateCommand()){
                cmd.CommandText="INSERT INTO Usuarios(Nombre,Edad,Fecha) VALUES (@n,@e,@f);";
                cmd.Parameters.AddWithValue("@n",nombre);
                cmd.Parameters.AddWithValue("@e",edad);
                cmd.Parameters.AddWithValue("@f",fecha);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
