using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimientojugador : MonoBehaviour
{
    [Header("Carriles")]
    public Transform[] carriles;

    [Header("Velocidades")]
    public float velocidadCambio = 8f;
    public float velocidadAvance = 10f;

    private int carrilActual = 1;
    private Vector3 posicionObjetivo;

    [Header("Control")]
    public bool puedeMoverse = false;

    private int vidas = 3;

    void Start()
    {
        if (carriles == null || carriles.Length == 0)
        {
            Debug.LogError("ERROR: no hay carriles asignados al jugador.");
            enabled = false;
            return;
        }

        posicionObjetivo = carriles[carrilActual].position;
        transform.position = new Vector3(posicionObjetivo.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (!puedeMoverse) return;

        transform.Translate(Vector3.forward * velocidadAvance * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoverIzquierda();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoverDerecha();

        // Control táctil
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch t = Input.GetTouch(0);
            if (t.position.x < Screen.width / 2) MoverIzquierda();
            else MoverDerecha();
        }

        Vector3 destino = carriles[carrilActual].position;
        Vector3 nuevaPos = new Vector3(destino.x, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, nuevaPos, Time.deltaTime * velocidadCambio);
    }

    void MoverIzquierda()
    {
        if (carrilActual > 0) carrilActual--;
    }

    void MoverDerecha()
    {
        if (carrilActual < carriles.Length - 1) carrilActual++;
    }

    // TURBO
    public IEnumerator ActivateTurbo(float duration)
    {
        float original = velocidadAvance;
        velocidadAvance *= 2f;
        yield return new WaitForSeconds(duration);
        velocidadAvance = original;
    }

    // RALENTIZAR
    public void Ralentizar()
    {
        StartCoroutine(RalentizarRutina());
    }

    IEnumerator RalentizarRutina()
    {
        float original = velocidadAvance;
        velocidadAvance *= 0.5f;
        yield return new WaitForSeconds(1.5f);
        velocidadAvance = original;
    }

    // VIDAS
    public void QuitarVida()
    {
        vidas--;
        Debug.Log("❌ Vida perdida. Restantes: " + vidas);

        if (vidas <= 0)
        {
            puedeMoverse = false;
            Debug.Log("💀 GAME OVER");
        }
    }

    // Para CPU
    public int GetCarrilActual()
    {
        return carrilActual;
    }

    public int GetVidas()
    {
        return vidas;
    }
}
