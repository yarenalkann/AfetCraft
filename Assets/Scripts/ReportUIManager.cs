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

    [Header("Buton Tik Objeleri")]
    public GameObject tikGuvenli;
    public GameObject tikOnarilmali;
    public GameObject tikYikilmali;

    private HouseInspector currentHouse;

    // Hafızada oyuncunun seçimini tutmak için geçici değişkenler
    private HouseData.BuildingStatus geciciOyuncuSecimi;

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (ozelPikselliImlec != null)
        {
            Cursor.SetCursor(ozelPikselliImlec, Vector2.zero, CursorMode.Auto);
        }
    }

    private void ResetTicks()
    {
        if (tikGuvenli != null) tikGuvenli.SetActive(false);
        if (tikOnarilmali != null) tikOnarilmali.SetActive(false);
        if (tikYikilmali != null) tikYikilmali.SetActive(false);
    }

    public void CloseReport()
    {
        reportPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    
    public void SubmitAssessment(int statusIndex)
    {
        if (currentHouse == null) return;

        ResetTicks(); 
        if (statusIndex == 0 && tikGuvenli != null) tikGuvenli.SetActive(true);
        else if (statusIndex == 1 && tikOnarilmali != null) tikOnarilmali.SetActive(true);
        else if (statusIndex == 2 && tikYikilmali != null) tikYikilmali.SetActive(true);

        geciciOyuncuSecimi = (HouseData.BuildingStatus)statusIndex;

        // ====================================================================
        // 🎯 GÜVENLİK KİLİDİ: Oyuncu butona bastığı an bu evin E tuşu hakkı kapanır!
        // ====================================================================
        currentHouse.raporGonderildiMi = true; 

        Invoke("RaporuKapatVeSonucuAc", 0.8f); 
    }

    // 3. ADIM: 0.8 saniye sonra çalışan gizli mühendislik fonksiyonumuz
    private void RaporuKapatVeSonucuAc()
    {
        // Ana rapor parşömenini şimdi kapatıyoruz (Tik işareti görevini yaptı, göründü)
        reportPanel.SetActive(false);

        // Dünyaya dönerken imleci kilitliyoruz ki kamera rahat dönsün
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        // Sonuç sayfasını Vertex Color ayarıyla tetikliyoruz
        if (FeedbackPopupManager.Instance != null && currentHouse != null)
        {
            HouseData.BuildingStatus correctChoice = currentHouse.data.correctStatus;

            // ====================================================================
            // 🎯 DOĞRU SEÇİM YAPILDIĞINDA ÇALIŞAN ALAN
            // ====================================================================
            if (geciciOyuncuSecimi == correctChoice)
            {
                string dogruMesaj = "Hasar tespiti başarıyla sonuçlandı.\nBölge bütçesinden " + currentHouse.data.rewardMoney + " TL aktarıldı.";
                FeedbackPopupManager.Instance.UyariSayfasiniAc(dogruMesaj, Color.green);

                // 🔨 SİHİRLİ DOKUNUŞ: Eğer doğru şık "Onarılmalı" ise...
                if (correctChoice == (HouseData.BuildingStatus)1)
                {
                    // Tam senin HouseInspector içindeki fonksiyon isminle tetikliyoruz!
                    currentHouse.OnarimIzniniAktifEt();
                }
            }
            // ====================================================================
            // ❌ YANLIŞ SEÇİM YAPILDIĞINDA ÇALIŞAN ALAN
            // ====================================================================
            else
            {
                string yanlisMesaj = "Hatalı değerlendirme yapıldı!!\nBakanlık cezası: Kasadan 2000 TL kesildi!";
                FeedbackPopupManager.Instance.UyariSayfasiniAc(yanlisMesaj, Color.black);
            }
        }
    }
}