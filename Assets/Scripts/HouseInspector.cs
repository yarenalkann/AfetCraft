using UnityEngine;

public class HouseInspector : MonoBehaviour, IInteractable
{
    public HouseData data; // Mühendislik verisini buraya sürükleyeceğiz

    public void Interact()
    {
        // UI Manager'a "Ben tıklandım, benim verilerimi göster" diyoruz
        if (ReportUIManager.Instance != null)
        {
            ReportUIManager.Instance.OpenReport(this);
        }
    }

    public void ToggleHighlight(bool isOn)
    {
        // Daha önce yaptığımız Outline (parlama) efekti buraya gelecek
        if (GetComponentInChildren<Outline>() != null)
            GetComponentInChildren<Outline>().enabled = isOn;
    }
}