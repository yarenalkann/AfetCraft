using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // <-- IŞINLANMA MOTORU (YENİ EKLENDİ)

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

    [Header("Görevler Sistemi (Operasyon Merkezi)")]
    public TMP_Text sagBaslikText;   
    public TMP_Text sagDetayText;
   // Sağdaki büyük görev başlığı
    public TMP_Text sagGereksinimText;  // Gerekli ekipman yazısı
    
    // Arka planda hangi görevin seçili olduğunu tutarız
    private int secilenGorevID = -1; 

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

            if (isTabletOpen)
            {
                OpenBasvurular(); // Açıldığında ilk sayfayı göster
                // Fareyi serbest bırak
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Fareyi oyuna geri kilitle
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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

    // ---- BAŞVURU SİSTEMİ ----

    public void BasvuruGoster(int basvuruID)
    {
        // Eğer detay penceresi kapalıysa butona basınca aç
        if (tabletPanel != null) 
        {
            // Eğer RaporKapsayici diye bir değişkenin varsa onu aktif et
            // Şimdilik sadece yazıları değiştiriyoruz:
        }

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

    // ---- GÖREVLER (OPERASYON) SİSTEMİ ----

    // Sol taraftaki butonlara tıklandığında çalışacak metod
   public void GorevSec(int gorevID)
    {
        secilenGorevID = gorevID;

        if (gorevID == 1)
        {
            sagBaslikText.text = "Görev 1: Enkaz Altından Sesler";
            sagDetayText.text = "İhbar: Binanın 1. katından yardım sesleri geliyor. İçeri gir ve afetzedeyi güvenli bölgeye taşı.";
            sagGereksinimText.text = "Gerekli Ekipman: Yok";
            sagGereksinimText.color = Color.white;
            // Detay metnini de buraya ekleyebiliriz (İsteğe bağlı)
        }
        else if (gorevID == 2)
        {
            sagBaslikText.text = "Görev 2: Gaz Sızıntısı";
            sagDetayText.text = "İhbar: Binada yoğun gaz kokusu var. Kıvılcım çıkmadan vanayı tamir etmelisin!";
            sagGereksinimText.text = "Gerekli Ekipman: Tamir Kiti";
            sagGereksinimText.color = Color.yellow;
        }
        else if (gorevID == 3)
        {
            sagBaslikText.text = "Görev 3: Çatlak Kolon Desteği";
            sagDetayText.text = "İhbar: Ana taşıyıcı kolonlar çatlamış. Bina çökmeden kolonları beton ve demirle güçlendir.";
            sagGereksinimText.text = "Gerekli Ekipman: Çimento ve Demir";
            sagGereksinimText.color = Color.cyan;
        }
    }

    // Yeşil "OPERASYONU BAŞLAT" butonuna tıklandığında çalışacak metod
    public void OperasyonuBaslat()
    {
        if (secilenGorevID == -1) 
        {
            Debug.Log("Önce sol taraftan bir görev seçmelisin!");
            return;
        }

        if (secilenGorevID == 1)
        {
            Debug.Log("Level 1 Yükleniyor! Eğitime başlanıyor...");
            // İŞTE IŞINLANMA KODU BURASI:
            SceneManager.LoadScene("Level1_Enkaz"); 
        }
        else if (secilenGorevID == 2)
        {
            // Level 2 için envanter kontrolü!
            if (tamirKitiSayisi > 0)
            {
                tamirKitiSayisi--; // Kiti kullandık
                MiktarlariGuncelle(); // Ekranda sayıyı düşürdük
                Debug.Log("Tamir kiti kullanıldı! Level 2'ye giriliyor...");
                
                // İleride Level 2 sahnesini açtığında buradaki // işaretlerini sileceksin:
                // SceneManager.LoadScene("Level2_GazSizintisi"); 
            }
            else
            {
                // Çantada kit yoksa oyuncuyu uyar!
                sagGereksinimText.color = Color.red;
                sagGereksinimText.text = "YETERSİZ EKİPMAN! Mağazadan Tamir Kiti Almalısın.";
                Debug.Log("Giremezsin, tamir kiti lazım!");
            }
        }
    }
}