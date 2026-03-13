using UnityEngine;
using TMPro;

public class TabletManager : MonoBehaviour
{
    [Header("Ana Tablet Objesi")]
    public GameObject tabletPanel; 

    [Header("Tablet Sayfaları (Paneller)")]
    public GameObject basvurularPage;
    public GameObject gorevlerPage;
    public GameObject magazaPage;
    public GameObject hasarKriterleriPage;
    public GameObject insaKriterleriPage;

    [Header("Başvuru Detay Sayfası")]
    public TMP_Text baslikText; 
    public TMP_Text detayText;  

    [Header("Ekonomi ve Envanter Sistemi")]
    public int oyuncuParasi = 1000; // Başlangıç bütçesi
    public TMP_Text bakiyeEkrani;   // Sol üstteki bakiye yazısı

    [Header("Envanter Yazıları (Slotlardaki Sayılar)")]
    public TMP_Text cimentoMiktarText;
    public TMP_Text demirMiktarText;
    public TMP_Text tuglaMiktarText;
    public TMP_Text tamirKitiMiktarText;

    // Sahip olduğumuz miktarlar (Arka planda tutulan sayılar)
    public int cimentoSayisi = 0;
    public int demirSayisi = 0;
    public int tuglaSayisi = 0;
    public int tamirKitiSayisi = 0;

    private bool isTabletOpen = false;

    // ---- OYUN BAŞLANGICI VE TUŞ KONTROLLERİ ----

    void Start()
    {
        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
            isTabletOpen = false;
        }
        
        // Oyun başladığında tüm UI yazılarını sıfırla/güncelle
        ParaYazisiniGuncelle();
        MiktarlariGuncelle();
    }

    void Update()
    {
        // Tab tuşu ile tableti aç/kapat
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTabletOpen = !isTabletOpen; 
            tabletPanel.SetActive(isTabletOpen);

            // Tablet açıldığında otomatik Başvurular sayfasını göster
            if (isTabletOpen)
            {
                OpenBasvurular();
            }
        }
    }

    // ---- EKONOMİ VE MAĞAZA SİSTEMİ ----

    public void EsyaSatinAl(int esyaID)
    {
        int fiyat = 0;
        string esyaAdi = "";

        // Hangi eşyanın seçildiğini ve fiyatını belirliyoruz
        if (esyaID == 0) { fiyat = 150; esyaAdi = "Çimento"; }
        else if (esyaID == 1) { fiyat = 250; esyaAdi = "Demir"; }
        else if (esyaID == 2) { fiyat = 50;  esyaAdi = "Tuğla"; }
        else if (esyaID == 3) { fiyat = 100; esyaAdi = "Tamir Kiti"; }

        // Parası yetiyorsa işlemi yap
        if (oyuncuParasi >= fiyat)
        {
            oyuncuParasi -= fiyat;  // Parayı kes
            
            // Eşyayı Envantere (cebe) ekle
            if (esyaID == 0) cimentoSayisi++;
            else if (esyaID == 1) demirSayisi++;
            else if (esyaID == 2) tuglaSayisi++;
            else if (esyaID == 3) tamirKitiSayisi++;

            // Ekranda görünen tüm yazıları yenile
            ParaYazisiniGuncelle();
            MiktarlariGuncelle();

            Debug.Log("BAŞARILI! 1 Adet " + esyaAdi + " alındı. Kalan Para: " + oyuncuParasi + " TL");
        }
        else
        {
            Debug.Log("HATA: " + esyaAdi + " için paran yetersiz! Gereken: " + fiyat + " TL");
        }
    }

    private void ParaYazisiniGuncelle()
    {
        if (bakiyeEkrani != null)
            bakiyeEkrani.text = "Bakiye: " + oyuncuParasi + " TL";
    }

    private void MiktarlariGuncelle()
    {
        // Slotlardaki x0, x1 gibi yazıları günceller
        if (cimentoMiktarText != null) cimentoMiktarText.text = "x" + cimentoSayisi;
        if (demirMiktarText != null) demirMiktarText.text = "x" + demirSayisi;
        if (tuglaMiktarText != null) tuglaMiktarText.text = "x" + tuglaSayisi;
        if (tamirKitiMiktarText != null) tamirKitiMiktarText.text = "x" + tamirKitiSayisi;
    }

    // ---- SAYFA DEĞİŞTİRME SİSTEMİ ----

    public void OpenBasvurular() { KapatTumSayfalar(); basvurularPage.SetActive(true); }
    public void OpenGorevler() { KapatTumSayfalar(); gorevlerPage.SetActive(true); }
    public void OpenMagaza() { KapatTumSayfalar(); magazaPage.SetActive(true); }
    public void OpenHasarKriterleri() { KapatTumSayfalar(); hasarKriterleriPage.SetActive(true); }
    public void OpenInsaKriterleri() { KapatTumSayfalar(); insaKriterleriPage.SetActive(true); }

    private void KapatTumSayfalar()
    {
        basvurularPage.SetActive(false);
        gorevlerPage.SetActive(false);
        magazaPage.SetActive(false);
        hasarKriterleriPage.SetActive(false);
        insaKriterleriPage.SetActive(false);
    }

    // ---- BAŞVURU (GÖREV) SİSTEMİ ----

    public void BasvuruGoster(int basvuruID)
    {
        if (basvuruID == 0)
        {
            baslikText.text = "Yeşil Apartmanı";
            detayText.text = "Başvuru Sahibi: Ayşegül Yılmaz\nAdres: Ahmet Yesevi Sokak, Cadde, 204\nBaşvuru Tarihi: 12.04.2024";
        }
        else if (basvuruID == 1)
        {
            baslikText.text = "Kahramanlar Sitesi";
            detayText.text = "Başvuru Sahibi: Mehmet Demir\nAdres: Cumhuriyet Bulvarı, No: 12\nBaşvuru Tarihi: 14.04.2024";
        }
        else if (basvuruID == 2)
        {
            baslikText.text = "Toros Apartmanı";
            detayText.text = "Başvuru Sahibi: Elif Kaya\nAdres: Toroslar Mahallesi, Çamlık Sokak\nBaşvuru Tarihi: 15.04.2024";
        }
    }

    public void HaritadaGosterButonunaBasildi()
    {
        isTabletOpen = false;
        tabletPanel.SetActive(false);
        Debug.Log(baslikText.text + " haritada işaretlendi!");
    }
}