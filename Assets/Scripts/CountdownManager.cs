using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownManager : MonoBehaviour
{
    public TMP_Text countdownText;
    public GameObject player;
    public GameObject[] cpuCars;

    void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1);
        countdownText.text = "";

        // después de activar los autos
        FindObjectOfType<MathManager>().GenerarOperacion();

        // Activar movimiento de autos
        player.GetComponent<Movimientojugador>().enabled = true;
        foreach (GameObject cpu in cpuCars)
        {
            cpu.GetComponent<MovimientoCPU>().enabled = true;   
        }
    }
}
