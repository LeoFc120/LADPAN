using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public Slot[] slots;               // 9 slots
    public RectTransform piezasContainer;
    public GameObject piezaPrefab;
    public int[] expectedValues = new int[9];

    void Start()
    {
        // Asignar valores esperados al tablero
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].expectedValue = expectedValues[i];
        }

        // Generar piezas
        foreach (int val in expectedValues)
        {
            GameObject p = Instantiate(piezaPrefab, piezasContainer);
            p.GetComponent<DragItem>().SetValue(val);
        }
    }
}