using UnityEngine;
using TMPro;

public class ReportUIManager : MonoBehaviour
{
    public static ReportUIManager Instance;

    [Header("Özel İmleç Ayarları")]
    public Texture2D ozelPikselliImlec;

    [Header("UI Panelleri")]
    public GameObject reportPanel;

    [Header("Dinamik Metin Alanları")]
    public TextMeshProUGUI txtSutunDegerler;
    public TextMeshProUGUI txtOnIncelemeDetayi;

    // ====================================================================
    // YENİ: BUTONLARIN TİK İŞARETLERİ
    // ====================================================================
    [Header("Buton Tik Objeleri")]
    public GameObject tikGuvenli;
    public GameObject tikOnarilmali;
    public GameObject tikYikilmali;
    // ====================================================================

    private HouseInspector currentHouse;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenReport(HouseInspector house)
    {
        currentHouse = house;
        ResetTicks();

        txtSutunDegerler.text = house.data.houseName + "\n" +
                                house.data.applicantName + "\n" +
                                house.data.date + "\n" +
                                house.data.address;
        
        if (txtOnIncelemeDetayi != null)
        {
            txtOnIncelemeDetayi.text = house.data.reportDetail;
        }

        reportPanel.SetActive(true);

        // ====================================================================
        // Sadece Rapor Açıldığında Fareyi Görünür Yap ve Özel İmleci Tak!
        // ====================================================================
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (ozelPikselliImlec != null)
        {
            // Vector2.zero -> Okun tam sol üst ucunun tıklamasını sağlar (Hotspot)
            Cursor.SetCursor(ozelPikselliImlec, Vector2.zero, CursorMode.Auto);
        }
    }

    // Tikleri sıfırlama fonksiyonu
    private void ResetTicks()
    {
        if (tikGuvenli != null) tikGuvenli.SetActive(false);
        if (tikOnarilmali != null) tikOnarilmali.SetActive(false);
        if (tikYikilmali != null) tikYikilmali.SetActive(false);
    }

    public void CloseReport()
    {
        reportPanel.SetActive(false);

        // Rapor kapanınca fareyi yine kilitliyoruz ve gizliyoruz
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // DIKKAT: Cursor.SetCursor(null, ...); SATIRINI SİLDIK!
        // Resmi sıfırlamadığımız için, ileride envanter kodunda sadece 
        // "Cursor.visible = true;" yaptığın an bu pikselli imleç otomatik çıkacak!
    }
    
    public void SubmitAssessment(int statusIndex)
    {
        if (currentHouse == null) return;

        // Basılan butona göre tiki gösteriyoruz (Burası aynen kalıyor)
        ResetTicks(); 
        if (statusIndex == 0 && tikGuvenli != null) tikGuvenli.SetActive(true);
        else if (statusIndex == 1 && tikOnarilmali != null) tikOnarilmali.SetActive(true);
        else if (statusIndex == 2 && tikYikilmali != null) tikYikilmali.SetActive(true);

        HouseData.BuildingStatus playerChoice = (HouseData.BuildingStatus)statusIndex;

        if (playerChoice == currentHouse.data.correctStatus)
        {
            Debug.Log("<color=green>[Afet Kıraat] Rapor Doğru! Para kazanıldı: </color>" + currentHouse.data.rewardMoney);
        }
        else
        {
            Debug.Log("<color=red>[Afet Kıraat] Rapor Yanlış! Kasadan 200 TL düştü.</color>");
        }

        // Evin etkileşimini kapatma mantığı (Burası da aynen kalıyor)
        currentHouse.gameObject.layer = 0; 
        Transform altModel = currentHouse.transform.Find("default");
        if (altModel != null) altModel.gameObject.layer = 0;

        // ====================================================================
        // ÇÖZÜM: ANINDA KAPATMAK YERİNE ZAMAN AYARLI KİLİT
        // ====================================================================
        // Direkt CloseReport(); yazan eski satırı SİL! yerine şu satırı yaz:
        // Bu komut, "CloseReport" fonksiyonunu tam 0.8 saniye sonra çalıştırır.
        Invoke("CloseReport", 0.8f); 
        // ====================================================================
    }
}