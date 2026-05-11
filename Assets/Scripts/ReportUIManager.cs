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
        titleText.text = "BİNA ANALİZİ: " + house.data.houseID;
        descriptionText.text = house.data.houseDescription;

        reportPanel.SetActive(true);
        
        // Fareyi serbest bırakıyoruz
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