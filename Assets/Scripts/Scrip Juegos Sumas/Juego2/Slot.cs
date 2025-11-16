using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int expectedValue = 0;       // valor esperado
    public TMP_Text numeroText;         // referencia al TMP hijo

    [HideInInspector] public bool ocupado = false;
    [HideInInspector] public GameObject currentPiece = null;

    public void SetNumber(int n)
    {
        if (numeroText != null)
            numeroText.text = n.ToString();
    }

    public void Clear()
    {
        ocupado = false;
        currentPiece = null;
        SetNumber(0);
    }

    public bool TryPlacePiece(GameObject piece)
    {
        if (ocupado) return false;

        piece.transform.SetParent(this.transform, false);
        piece.transform.localPosition = Vector3.zero;

        currentPiece = piece;
        ocupado = true;

        DragItem d = piece.GetComponent<DragItem>();
        if (d != null)
            SetNumber(d.valor);

        return true;
    }
}
