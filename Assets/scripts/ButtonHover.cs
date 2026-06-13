using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.08f, 1.08f, 1.08f); // %8 büyütür
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one; // Normal boyuta döndürür
    }
}