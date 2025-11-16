using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int valor = 0;
    public TMP_Text numeroText;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 startPos;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetValue(valor);
    }

    public void SetValue(int v)
    {
        valor = v;
        if (numeroText != null)
            numeroText.text = v.ToString();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        originalParent = transform.parent;
        startPos = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e)
    {
        transform.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == originalParent)
            transform.position = startPos; // no se colocó en slot
    }
}
