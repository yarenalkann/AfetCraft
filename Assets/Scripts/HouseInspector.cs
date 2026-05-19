using UnityEngine;

public class HouseInspector : MonoBehaviour, IInteractable
{
    public HouseData data; 

    public void Interact()
    {
        if (data != null && ReportUIManager.Instance != null)
        {
            ReportUIManager.Instance.OpenReport(this);
        }
    }

    // Mühendislik Tüyosu: Unity Editor'de bu scriptin sağ üstündeki üç noktaya 
    // basınca bu fonksiyon çıkar ve evin o anki X,Y,Z konumunu otomatik dosyaya yazar!
    [ContextMenu("Konumu Veriye Kaydet")]
    public void SaveCurrentPositionToData()
    {
        if (data != null)
        {
            data.houseXYZLocation = transform.position;
            Debug.Log(gameObject.name + " konumu " + transform.position + " olarak kaydedildi!");
        }
        else
        {
            Debug.LogError("Önce 'Data' kutucuğuna bir HouseData sürüklemelisin!");
        }
    }

    public void ToggleHighlight(bool isOn)
    {
        if (GetComponentInChildren<Outline>() != null)
            GetComponentInChildren<Outline>().enabled = isOn;
    }
}