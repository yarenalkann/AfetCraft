using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class ReportUIManager : MonoBehaviour
{
    public static ReportUIManager Instance; // Singleton: Diğer kodlar buna kolayca ulaşsın

    [Header("UI Elemanları")]
    public GameObject reportPanel;       // Ana Panel
    public TextMeshProUGUI titleText;    // Ev ID'si yazısı
    public TextMeshProUGUI descriptionText; // Açıklama yazısı
    public TextMeshProUGUI moneyText;    // Mevcut parayı göstermek için (opsiyonel)

    private HouseInspector currentHouse; // O an hangi eve bakıyoruz?

    void Awake()
    {
        Instance = this;
        reportPanel.SetActive(false); // Başlangıçta kapalı olsun
    }

    // Ev tıklandığında bu fonksiyon çağrılır
    public void OpenReport(HouseInspector house)
    {
        currentHouse = house;
    
        // Başlıkta sadece Ev Numarası ve Ev Adı görünecek
        titleText.text = house.data.houseID + " - " + house.data.houseName;
    
        // Koordinat (xyzMetni) kısmını tamamen uçurduk, temiz bir rapor oldu
        descriptionText.text = "<b>Başvuran:</b> " + house.data.applicantName + "\n" +
                                "<b>Tarih:</b> " + house.data.date + "\n" +
                                "<b>Adres:</b> " + house.data.address + "\n\n" +
                                "<b>ÖN İNCELEME DETAYI:</b>\n" + house.data.reportDetail;

        reportPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Butonlara tıklandığında bu fonksiyon çalışacak. 
    // Parametre: 0=Safe, 1=Repair, 2=Demolish
    public void SubmitAssessment(int choiceIndex)
    {
        int correctIndex = (int)currentHouse.data.correctStatus;

        if (choiceIndex == correctIndex)
        {
            Debug.Log("Analiz Doğru! Para kazanıldı.");
            // Arkadaşının yaptığı Inventory/Para sistemine buradan ulaşabilirsin
            //InventoryManager.Instance.money += currentHouse.data.rewardMoney;
        }
        else
        {
            Debug.Log("Hatalı Analiz! Mühendislik hatası.");
        }

        CloseReport();
    }

    public void CloseReport()
    {
        reportPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}