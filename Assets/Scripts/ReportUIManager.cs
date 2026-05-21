using UnityEngine;
using TMPro;

public class ReportUIManager : MonoBehaviour
{
    public static ReportUIManager Instance;

    [Header("UI Panelleri")]
    public GameObject reportPanel; // Rapor kağıdı paneli

    [Header("Dinamik Metin Alanları")]
    public TextMeshProUGUI txtSutunDegerler;     // Sadece değerlerin alt alta yazacağı tek büyük kutu
    public TextMeshProUGUI txtOnIncelemeDetayi; // Raporun altındaki geniş detay açıklaması

    private HouseInspector currentHouse;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenReport(HouseInspector house)
    {
        // EĞER BU LOG KONSOLA DÜŞERSE PANEL KESİNLİKLE TETİKLENİYORDUR!
        Debug.Log("<color=cyan>SİSTEM: ReportUIManager -> OpenReport fonksiyonu BAŞARIYLA ÇALIŞTI!</color>");

        currentHouse = house;
    
        txtSutunDegerler.text = house.data.houseName + "\n" +
                                house.data.applicantName + "\n" +
                                house.data.date + "\n" +
                                house.data.address;
    
        if (txtOnIncelemeDetayi != null)
        {
            txtOnIncelemeDetayi.text = house.data.reportDetail;
        }

        reportPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseReport()
    {
        reportPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SubmitAssessment(int statusIndex)
        {
            if (currentHouse == null) return;

            // KONSOLDA SAYILARI GÖRMEK İÇİN BU İKİ SATIRI EKLE:
            Debug.Log("SİSTEM -> Oyuncunun bastığı butonun sayısı: " + statusIndex);
            Debug.Log("SİSTEM -> Evin ScriptableObject'indeki doğru sayı değeri: " + (int)currentHouse.data.correctStatus);

            HouseData.BuildingStatus playerChoice = (HouseData.BuildingStatus)statusIndex;

            if (playerChoice == currentHouse.data.correctStatus)
            {
                Debug.Log("<color=green>[AfetCraft] Rapor Doğru! Para kazanıldı: </color>" + currentHouse.data.rewardMoney);
            }
            else
            {
                Debug.Log("<color=red>[AfetCraft] Rapor Yanlış! Kasadan 200 TL düştü.</color>");
            }

            // ====================================================================
            // %100 KESİN KİLİTLEME: KATMAN DEĞİŞTİRME MÜHENDİSLİĞİ
            // ====================================================================
        
            // 1. Ana evin katmanını "Default" (0) yapıyoruz. Artık ışınlar onu algılamayacak!
            currentHouse.gameObject.layer = 0; 

            // 2. Alttaki 3D modelin (default objesinin) katmanını da "Default" yapıyoruz
            Transform altModel = currentHouse.transform.Find("default");
            if (altModel != null)
            {
            altModel.gameObject.layer = 0;
            }
        
            // 3. Scriptleri ve parlamayı da her ihtimale karşı yine kapatıyoruz
            currentHouse.enabled = false;
            InteractableObject altEtkilesimScripti = currentHouse.GetComponentInChildren<InteractableObject>();
            if (altEtkilesimScripti != null)
            {
                altEtkilesimScripti.ToggleHighlight(false);
                altEtkilesimScripti.enabled = false;
            }
            // ====================================================================

            CloseReport();
        }
}