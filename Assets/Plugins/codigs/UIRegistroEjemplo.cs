using UnityEngine;
using UnityEngine.UI;
using System;

public class UIRegistroEjemplo : MonoBehaviour {
    public InputField nombre;
    public InputField edad;
    public Text fechaActual;

    void Start(){
        fechaActual.text = DateTime.Now.ToString("yyyy-MM-dd");
    }

    public void Registrar(){
        UsuarioDB.Registrar(nombre.text,int.Parse(edad.text),fechaActual.text);
        Debug.Log("Usuario registrado");
    }
}
