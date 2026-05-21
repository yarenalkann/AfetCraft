using UnityEngine;

public class HouseInspector : MonoBehaviour, IInteractable
{
    public HouseData data; // Evin ScriptableObject verisi

    public void Interact()
    {
        Debug.Log("SİSTEM: HouseInspector'a ulaşıldı, rapor açılıyor...");

        if (data == null)
        {
            Debug.LogError("ARIZA: Bu evin ScriptableObject verisi (Data) boş! Müfettiş neyi okusun yavrum?");
        }

        if (ReportUIManager.Instance == null)
        {
            Debug.LogError("ARIZA: Sahnede ReportUIManager.Instance bulunamadı! UIManager objen sahneye eklenmemiş veya Singleton kurulamamış.");
        }

        if (data != null && ReportUIManager.Instance != null)
        {
            ReportUIManager.Instance.OpenReport(this);
        }
    }

    // Harita/Tablet sistemi için ContextMenu fonksiyonu (Duruyor)
    [ContextMenu("Konumu Veriye Kaydet")]
    public void SaveCurrentPositionToData()
    {
        if (data != null)
        {
            data.houseXYZLocation = transform.position;
            Debug.Log(gameObject.name + " konumu " + transform.position + " olarak kaydedildi!");
        }
    }

    public void ToggleHighlight(bool isOn)
    {
        if (GetComponentInChildren<Outline>() != null)
            GetComponentInChildren<Outline>().enabled = isOn;
    }
}