using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FondoInfinito : MonoBehaviour
{
    public float velocidad = 5f;          // velocidad de avance del fondo
    public float alturaSprite = 20f;      // altura del sprite
    public Transform[] segmentos;         // los dos segmentos del fondo

    void Update()
    {
        foreach (Transform seg in segmentos)
        {
            // Mueve los fondos hacia abajo
            seg.Translate(Vector3.down * velocidad * Time.deltaTime);

            // Si un segmento se sale de la pantalla, lo recoloca arriba
            if (seg.position.y < -alturaSprite)
            {
                Transform top = ObtenerSegmentoSuperior();
                seg.position = new Vector3(seg.position.x, top.position.y + alturaSprite, seg.position.z);
            }
        }
    }

    Transform ObtenerSegmentoSuperior()
    {
        return (segmentos[0].position.y > segmentos[1].position.y) ? segmentos[0] : segmentos[1];
    }

    // 🔹 Permite cambiar la velocidad del fondo (para turbo, freno, etc.)
    public void CambiarVelocidad(float nuevaVelocidad)
    {
        velocidad = nuevaVelocidad;
    }
}
