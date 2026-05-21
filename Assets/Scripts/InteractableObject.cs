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
        outline.OutlineColor = Color.white; // Parlama rengi
    }

    public void Interact() // Senin koddaki fonksiyon adı neyse artık
    {
        // 1. Önce bu objenin bağlı olduğu üst ana objede (Parent) HouseInspector var mı ona bakıyoruz
        HouseInspector anaEvScripti = GetComponentInParent<HouseInspector>();

        // 2. Eğer yukarıda o kodu bulduysak, onun içindeki Interact'ı tetikliyoruz!
        if (anaEvScripti != null)
        {
            anaEvScripti.Interact();
        }
        else
        {
            Debug.LogError("default objesi yukarıdaki ana objede HouseInspector scriptini bulamadı yavrum!");
        }
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