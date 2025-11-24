using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public string nombre;
    public int edad;
    public string fechaRegistro;

    public Dictionary<string, Puntaje> puntajes = new Dictionary<string, Puntaje>();

    public UserData(string nombre, int edad, string fechaRegistro)
    {
        this.nombre = nombre;
        this.edad = edad;
        this.fechaRegistro = fechaRegistro;
    }
}
