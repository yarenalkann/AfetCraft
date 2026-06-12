using UnityEngine;
using UnityEngine.EventSystems;

public class HizliKullanimSagTikTetikleyici : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Bu slotun indeks numarası (0, 1 veya 2)")]
    public int slotIndex;

    // Unity UI üzerinde herhangi bir tıklama olduğunda burası otomatik tetiklenir
    public void OnPointerClick(PointerEventData eventData)
    {
        // Eğer tıklanan fare tuşu SAĞ TIK ise
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (InventoryUIManager.Instance != null)
            {
                // UI Manager'daki o yazdığımız yeni fonksiyonu çağırıyoruz!
                InventoryUIManager.Instance.HizliKullanimSagTiklandi(slotIndex);
            }
        }
    }
}