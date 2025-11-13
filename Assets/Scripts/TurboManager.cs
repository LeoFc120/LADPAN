using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class turbomanager : MonoBehaviour
{
    public int turboCount = 3;
    public float turboDuration = 2f;
    public TMP_Text turboText;
    public Movimientojugador playerCar; // referencia al script del auto

    void Start()
    {
        UpdateTurboUI();
    }

    public void AddTurbo()
    {
        if (turboCount > 0)
        {
            turboCount--;
            UpdateTurboUI();
            StartCoroutine(playerCar.ActivateTurbo(turboDuration));
        }
    }

    void UpdateTurboUI()
    {
        if (turboText != null)
            turboText.text = "Turbos: " + turboCount;
    }
}

