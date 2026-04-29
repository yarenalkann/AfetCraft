using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    private Outline outline; // QuickOutline paketindeki component

    void Start()
    {
        // Önce objenin üzerinde bir Outline bileşeni var mı bakalım
        outline = GetComponent<Outline>();

        if (outline == null)
        {
            // Eğer yoksa, çalışma anında otomatik ekleyelim (Mühendislik budur!)
            outline = gameObject.AddComponent<Outline>();
        }

        // Başlangıçta parlamasın
        outline.enabled = false;

        // İsteğe bağlı görsel ayarlar
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 5f; // Kenar kalınlığı
        outline.OutlineColor = Color.yellow; // Parlama rengi
    }

    public void Interact()
    {
        Debug.Log(gameObject.name + " tıklandı!");
    }

    public void ToggleHighlight(bool isOn)
    {
        if (outline != null)
        {
            // Sadece kenar çizgisini açıyoruz, objenin rengine dokunmuyoruz
            outline.enabled = isOn;
        }
    }
}