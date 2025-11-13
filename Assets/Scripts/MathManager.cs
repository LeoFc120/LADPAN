using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathManager : MonoBehaviour
{
    public TMP_Text operationText;
    public GameObject[] answerBoxes; // las 3 cajas de respuestas
    private int correctAnswer;

    void Start()
    {
        GenerarOperacion();
    }

    public void GenerarOperacion()
    {
        // Operaciones simples tipo primaria
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        correctAnswer = a + b;

        operationText.text = a + " + " + b;

        // Elegir una caja donde irá la respuesta correcta
        int correctIndex = Random.Range(0, answerBoxes.Length);

        for (int i = 0; i < answerBoxes.Length; i++)
        {
            TMP_Text txt = answerBoxes[i].GetComponentInChildren<TMP_Text>();
            if (i == correctIndex)
                txt.text = correctAnswer.ToString();
            else
                txt.text = Random.Range(1, 20).ToString();
        }
    }

    public bool ComprobarRespuesta(int respuesta)
    {
        return respuesta == correctAnswer;
    }
}
