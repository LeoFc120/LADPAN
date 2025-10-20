using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FondoInfinito : MonoBehaviour
{
    public float velocidad = 5f;          // velocidad de avance
    public float alturaSprite = 20f;      // altura en unidades del sprite (ajústalo)
    public Transform[] segmentos;         // referencias a los dos segmentos

    void Update()
    {
        foreach (Transform seg in segmentos)
        {
            seg.Translate(Vector3.down * velocidad * Time.deltaTime);

            if (seg.position.y < -alturaSprite)
            {
                Transform top = ObtenerSegmentoSuperior();
                seg.position = new Vector3(seg.position.x, top.position.y + alturaSprite, seg.position.z);
            }
        }
    }

    Transform ObtenerSegmentoSuperior()
    {
        if (segmentos[0].position.y > segmentos[1].position.y)
            return segmentos[0];
        else
            return segmentos[1];
    }

    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
