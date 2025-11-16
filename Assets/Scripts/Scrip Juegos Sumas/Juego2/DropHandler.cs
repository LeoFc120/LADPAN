using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public Slot slot;

    public void OnDrop(PointerEventData eventData)
    {
        if (slot == null) return;

        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();
        if (item == null) return;

        bool colocado = slot.TryPlacePiece(item.gameObject);

        if (!colocado)
        {
            item.transform.position = item.transform.GetComponent<RectTransform>().anchoredPosition;
        }
    }
}