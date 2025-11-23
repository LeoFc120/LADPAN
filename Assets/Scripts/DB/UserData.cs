using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string nombre;
    public int edad;
    public string fechaRegistro;

    public Dictionary<string, Puntaje> puntajes = new Dictionary<string, Puntaje>();
}
