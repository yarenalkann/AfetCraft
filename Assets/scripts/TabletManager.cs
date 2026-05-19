using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class TabletManager : MonoBehaviour
{
    public GameObject gorevHUD;

    [Header("Ana Tablet Objesi")]
    public GameObject tabletPanel;

    [Header("Tablet Sayfaları (Paneller)")]
    public GameObject basvurularPage;
    public GameObject gorevlerPage;
    public GameObject magazaPage;
    public GameObject hasarKriterleriPage;
    public GameObject insaKriterleriPage;

    [Header("Dinamik Başvuru Sistemi")]
    public List<EvVerisi> tumEvHavuzu;
    private List<EvVerisi> secilenGununEvleri = new List<EvVerisi>();
    public TMP_Text[] solButonYazilari;
    private EvVerisi suAnkiSeciliEv; // <--- HEDEFLER İÇİN SEÇİLİ EVİ BURADA TUTUYORUZ

    [Header("Başvuru Detay Sayfası")]
    public TMP_Text baslikText;
    public TMP_Text detayText;

    [Header("Ekonomi ve Envanter Sistemi")]
    public int oyuncuParasi = 1000;
    public TMP_Text bakiyeEkrani;

    [Header("Envanter Yazıları (Slotlardaki Sayılar)")]
    public TMP_Text cimentoMiktarText;
    public TMP_Text demirMiktarText;
    public TMP_Text tuglaMiktarText;
    public TMP_Text tamirKitiMiktarText;
    public TMP_Text cekicMiktarText; // Çekiç sayısı yazısı
    public TMP_Text balyozMiktarText;

    public int balyozSayisi = 0;
    public int cekicSayisi = 0; // Kaç çekiç var?
    public int cimentoSayisi = 0;
    public int demirSayisi = 0;
    public int tuglaSayisi = 0;
    public int tamirKitiSayisi = 0;

    [Header("Görevler Sistemi (Operasyon Merkezi)")]
    public TMP_Text sagBaslikText;
    public TMP_Text sagDetayText;
    public TMP_Text sagGereksinimText;
    public int kontrolEdilenBinaSayisi = 0; // Kaç bina kontrol edildiğini tutan değişken
    private int secilenGorevID = -1;
    private bool isTabletOpen = false;

    [Header("Hedef ve Minimap Sistemi")] // <--- YENİ EKLENEN KISIM
    public GameObject haritaIsaretcisi; // Minimap'teki pin objesi
    public TextMeshProUGUI hataMesaji;
    public int oyuncuSeviyesi = 1; // Başlangıç seviyesi
    public Button[] gorevButonlari; // 3 adet görev butonunu tutacak dizi
    public int bilincPuani = 0; // Mevcut puan
    public int seviyeAtlamaSiniri = 100; // Seviye 2'ye geçmek için gereken toplam puan
    [Header("Seviye ve Puan Sistemi")]

    [Header("Dinamik Görev Sistemi")]
    public GameObject bildirimUnlemi; // Unity'den kırmızı ünlemi buraya sürükle
    public GameObject gorevBasariliPanel; // Bunu eklediğin an Inspector'da kutucuk çıkacak

    public TMP_Text bilincPuaniText; // Unity'den sürükleyeceğin yazı objesi
    public GameObject[] gorevGruplari; // 3 görev grubunu buraya bağlayacağız
    public Transform operasyonBolgesi; // Binaların olduğu yerin koordinatı
    public GameObject player; // Senin karakterin
    public Light anaIsik; // Sahnedeki Directional Light'ı buraya sürükleyeceksin
    public AudioSource sesKaynagi;
    bool gorev2Bitti = false; // Başlangıçta görev bitmediği için false yapıyoruz
    public bool isGorevBinasi = false; // Bunu ekleyince 15. satırdaki hata gidecek
    void Start()
    {
        // 1. Tabletin başlangıç durumu
        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
            isTabletOpen = false;
        }

        // 2. KAYITLI VERİLERİ YÜKLE (PlayerPrefs)
        // Eğer cihazda kayıt yoksa; para 500, diğerleri 0 olarak gelir.
        oyuncuParasi = PlayerPrefs.GetInt("K_Para", 500);
        cimentoSayisi = PlayerPrefs.GetInt("K_Cimento", 0);
        demirSayisi = PlayerPrefs.GetInt("K_Demir", 0);
        tamirKitiSayisi = PlayerPrefs.GetInt("K_TamirKiti", 0);

        kontrolEdilenBinaSayisi = 0;

        // 3. EKRAN YAZILARINI VE SİSTEMLERİ GÜNCELLE
        ParaYazisiniGuncelle();

        MiktarlariGuncelle();
        BasvurulariYenile();
        SeviyeKontrolEt();
    }



    // Bu fonksiyon Update'in dışında olmalı!

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTabletOpen = !isTabletOpen;
            tabletPanel.SetActive(isTabletOpen);

            if (isTabletOpen)
            {
                OpenBasvurular();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }


    }

    // ---- EKONOMİ VE MAĞAZA SİSTEMİ (DOKUNULMADI) ----
    public void EsyaSatinAl(int esyaID)
    {
        int fiyat = 0;
        string esyaAdi = "";
        if (esyaID == 0) { fiyat = 150; esyaAdi = "Çimento"; }
        else if (esyaID == 1) { fiyat = 250; esyaAdi = "Demir"; }
        else if (esyaID == 2) { fiyat = 50; esyaAdi = "Tuğla"; }
        else if (esyaID == 3) { fiyat = 100; esyaAdi = "Tamir Kiti"; }
        else if (esyaID == 4) { fiyat = 300; esyaAdi = "Çekiç"; }
        else if (esyaID == 5) { fiyat = 500; esyaAdi = "Balyoz"; }

        if (oyuncuParasi >= fiyat)
        {
            oyuncuParasi -= fiyat;
            if (esyaID == 0) cimentoSayisi++;
            else if (esyaID == 1) demirSayisi++;
            else if (esyaID == 2) tuglaSayisi++;
            else if (esyaID == 3) tamirKitiSayisi++;
            else if (esyaID == 4) cekicSayisi++;
            else if (esyaID == 5) balyozSayisi++;
            ParaYazisiniGuncelle();
            MiktarlariGuncelle();
        }
    }

    private void ParaYazisiniGuncelle() { if (bakiyeEkrani != null) bakiyeEkrani.text = "Bakiye: " + oyuncuParasi + " TL"; }
    private void MiktarlariGuncelle()
    {
        if (cimentoMiktarText != null) cimentoMiktarText.text = "x" + cimentoSayisi;
        if (demirMiktarText != null) demirMiktarText.text = "x" + demirSayisi;
        if (tuglaMiktarText != null) tuglaMiktarText.text = "x" + tuglaSayisi;
        if (tamirKitiMiktarText != null) tamirKitiMiktarText.text = "x" + tamirKitiSayisi;
        if (cekicMiktarText != null) cekicMiktarText.text = "x" + cekicSayisi;
        if (balyozMiktarText != null) balyozMiktarText.text = "x" + balyozSayisi;
    }

    // ---- SAYFA DEĞİŞTİRME SİSTEMİ (DOKUNULMADI) ----
    public void OpenBasvurular() { KapatTumSayfalar(); basvurularPage.SetActive(true); }
    public void OpenGorevler()
    { // Fonksiyon burada başlar
        KapatTumSayfalar();
        gorevlerPage.SetActive(true);

        if (bildirimUnlemi != null)
        {
            bildirimUnlemi.SetActive(false);
        }
    } // Fonksiyon burada biter
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
    public void BasvurulariYenile()
    {
        Debug.Log("SİSTEM: Ev listesi yenileme işlemi başladı!"); // TEST MESAJI 1

        secilenGununEvleri.Clear();
        List<EvVerisi> geciciListe = new List<EvVerisi>(tumEvHavuzu);

        for (int i = 0; i < 3 && geciciListe.Count > 0; i++)
        {
            int rastgeleIndex = Random.Range(0, geciciListe.Count);
            secilenGununEvleri.Add(geciciListe[rastgeleIndex]);

            if (solButonYazilari[i] != null)
            {
                solButonYazilari[i].text = secilenGununEvleri[i].houseName;
                Debug.Log("SİSTEM: Buton " + i + " ismi '" + secilenGununEvleri[i].houseName + "' olarak değiştirildi."); // TEST MESAJI 2
            }

            geciciListe.RemoveAt(rastgeleIndex);
        }
    }

    public void GorevTamamlandi()
    {
        // 1. Puanı ver (Seviye 2'ye geçişi tetikler)
        BilincPuaniArtir(100);

        // 2. Hazırladığın siyah paneli ekrana getir
        if (gorevBasariliPanel != null)
        {
            gorevBasariliPanel.SetActive(true);
        }

        if (gorevHUD != null)
        {
            gorevHUD.SetActive(false);
        }

        // 3. 3 saniye sonra her şeyi kapatmak için zamanlayıcıyı çalıştır
        Invoke("KapatBasariEkrani", 3f);
    }

    void KapatBasariEkrani()
    {
        if (gorevBasariliPanel != null)
        {
            gorevBasariliPanel.SetActive(false);
        }

        // Tableti kapat ve fareyi normal moda döndür
        isTabletOpen = false;
        tabletPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GorevAlButonunaBasildi()
    {
        if (suAnkiSeciliEv != null && haritaIsaretcisi != null)
        {
            // İşaretçiyi evin Vector3 konumuna ışınla
            haritaIsaretcisi.transform.position = suAnkiSeciliEv.houseLocation;
            haritaIsaretcisi.SetActive(true);

            Debug.Log(suAnkiSeciliEv.houseName + " için başvuru kabul edildi, işaretçi güncellendi.");

        }
    }
    public void BinaKontrolEt()
    {
        kontrolEdilenBinaSayisi++; // Her bina bittiğinde bu sayı 1 artar
        Debug.Log("Kontrol Edilen Bina: " + kontrolEdilenBinaSayisi);

        // EĞER 3 BİNAYA ULAŞILDIYSA:
        if (kontrolEdilenBinaSayisi >= 3)
        {
            GorevTamamlandi(); // Daha önce yazdığımız o siyah ekranlı fonksiyonu çağırır
            kontrolEdilenBinaSayisi = 0; // Bir sonraki görev için sayacı sıfırlıyoruz
        }
    }
    public void UpdateUI()
    {
        // Bakiye kontrolü
        if (bakiyeEkrani != null)
            bakiyeEkrani.text = oyuncuParasi.ToString() + " TL";

        // Puan kontrolü
        if (bilincPuaniText != null)
            bilincPuaniText.text = "Bilinç Puanı: " + bilincPuani + " / " + seviyeAtlamaSiniri;

        Debug.Log("Arayüz ve Puanlar güncellendi.");
    }



    public void BilincPuaniArtir(int miktar)
    {
        bilincPuani += miktar;
        Debug.Log("Puan Eklendi! Yeni Puan: " + bilincPuani);

        // Seviye Atlama Kontrolü
        if (bilincPuani >= seviyeAtlamaSiniri && oyuncuSeviyesi == 1)
        {
            oyuncuSeviyesi = 2;
            SeviyeKontrolEt(); // Butonların kilidini açar
            HataMesajiniGoster("TEBRİKLER! Seviye 2'ye ulaştınız.");
        }

        UpdateUI(); // Yazıları tazele
        VerileriKaydet(); // Veriyi hafızaya al
    }


    public void HataMesajiniGoster(string mesaj)
    {
        hataMesaji.text = mesaj;
        hataMesaji.gameObject.SetActive(true); // Yazıyı görünür yap
        Invoke("HataMesajiniGizle", 5f); // 3 saniye sonra gizle fonksiyonunu çağır
    }

    public void SonrakiGoreviHazirla(int tamamlananGorevIndex)
    {
        // Mevcut (biten) görev binalarını gizle
        gorevGruplari[tamamlananGorevIndex].SetActive(false);

        // Bir sonraki görev grubunu aç (Eğer varsa)
        if (tamamlananGorevIndex + 1 < gorevGruplari.Length)
        {
            gorevGruplari[tamamlananGorevIndex + 1].SetActive(true);
            SeviyeKontrolEt();
            YeniGorevBildirimi(); // Daha önce yazdığımız ünlem fonksiyonu!
            
        }
    }



    public void HataMesajiniGizle()
    {
        hataMesaji.gameObject.SetActive(false);
    }
    public void BasvuruGoster(int basvuruID)
    {
        if (basvuruID < secilenGununEvleri.Count)
        {
            suAnkiSeciliEv = secilenGununEvleri[basvuruID]; // <--- SEÇİLENİ HAFIZAYA ALDIK
            baslikText.text = suAnkiSeciliEv.houseName;
            detayText.text = "<b>Başvuru Sahibi:</b> " + suAnkiSeciliEv.applicantName +
                             "\n<b>Adres:</b> " + suAnkiSeciliEv.address +
                             "\n<b>Tarih:</b> " + suAnkiSeciliEv.date +
                             "\n\n<b>RAPOR:</b> " + suAnkiSeciliEv.reportDetail;
        }
    }

    // ---- DİĞER SİSTEMLER (DOKUNULMADI) ----
    public void HaritadaGosterButonunaBasildi()
    {
        isTabletOpen = false;
        tabletPanel.SetActive(false);
        Debug.Log(baslikText.text + " haritada işaretlendi!");
    }

    public void GorevSec(int gorevID)
    {
        secilenGorevID = gorevID;
        if (gorevID == 1)
        {
            sagBaslikText.text = "Görev 1: Enkaz Altından Sesler";
            sagDetayText.text = "İhbar: Binanın 1. katından yardım sesleri geliyor. İçeri gir ve afetzedeyi güvenli bölgeye taşı.";
            sagGereksinimText.text = "Gerekli Ekipman: Yok";
            sagGereksinimText.color = Color.white;
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




    public void VerileriKaydet()
    {
        PlayerPrefs.SetInt("K_Para", oyuncuParasi);
        PlayerPrefs.SetInt("K_Cimento", cimentoSayisi);
        PlayerPrefs.SetInt("K_Demir", demirSayisi);
        PlayerPrefs.SetInt("K_TamirKiti", tamirKitiSayisi);
        PlayerPrefs.SetInt("K_Cekic", cekicSayisi);
        PlayerPrefs.SetInt("K_Balyoz", balyozSayisi);
        PlayerPrefs.Save();
        Debug.Log("<color=green>SİSTEM: Veriler Hafızaya Yazıldı!</color>");
    }

    public void SeviyeKontrolEt()
    {
        Debug.Log("Sistem: Tablet butonları şu an güncelleniyor...");
        Debug.Log("Şu anki oyuncu seviyesi: " + oyuncuSeviyesi); // Console panelinde seviyeyi görürüz
        // Görev 1 hep görünür olsun
        gorevButonlari[0].gameObject.SetActive(true);

        // Seviye 2 ise Görev 2'yi "yoktan var et"
        if (oyuncuSeviyesi >= 2)
        {
            if (gorevButonlari[1].gameObject.activeSelf == false) // Eğer henüz kapalıysa
            {
                gorevButonlari[1].gameObject.SetActive(true); // Görünür yap
                bildirimUnlemi.SetActive(true); // Ünlemi yak!
            }
        }
        else
        {
            gorevButonlari[1].gameObject.SetActive(false); // Seviye yetmiyorsa listede hiç olmasın
                                                           // Seviye 3 ise Görev 3'ü aç
        if (oyuncuSeviyesi >= 3)
        {
            if (gorevButonlari[2].gameObject.activeSelf == false)
            {
                gorevButonlari[2].gameObject.SetActive(true);
                bildirimUnlemi.SetActive(true); // Ünlemi yak!
            }
        }
        else
        {
            gorevButonlari[2].gameObject.SetActive(false);
        }
        }

        // Görev 3 için de aynısını yapabilirsin...
    }



    public void YeniGorevBildirimi()
    {
        if (bildirimUnlemi != null)
        {
            bildirimUnlemi.SetActive(true);
        }
    }



    public void OperasyonuBaslat()
    {
        VerileriKaydet();

        if (secilenGorevID == -1)
        {
            HataMesajiniGoster("Lütfen önce bir görev seçin!");
            return;
        }

        // GÖREV 1 KONTROLÜ
        if (secilenGorevID == 1)
        {
            BaslatVeIsinla(); // Ekipman gerekmiyorsa direkt başla
        }
        // GÖREV 2 KONTROLÜ
        else if (secilenGorevID == 2)
        {
            if (tamirKitiSayisi >= 1)
            {
                BaslatVeIsinla();
            }
            else
            {
                HataMesajiniGoster("Tamir Kiti gerekli!");
            }
        }
        // GÖREV 3 KONTROLÜ
        else if (secilenGorevID == 3)
        {
            if (cimentoSayisi >= 1 && demirSayisi >= 1)
            {
                BaslatVeIsinla();
            }
            else
            {
                if (cimentoSayisi < 1 && demirSayisi < 1)
                    HataMesajiniGoster("Çimento ve Demir eksik!");
                else if (cimentoSayisi < 1)
                    HataMesajiniGoster("Çimento eksik!");
                else
                    HataMesajiniGoster("Demir eksik!");
            }
        }
    }

    // Ortak ışınlanma ve tablet kapatma fonksiyonu
    private void BaslatVeIsinla()
    {
        // 1. Verileri kaydet ve tableti kapat
        VerileriKaydet();
        if (sesKaynagi != null) sesKaynagi.Play();
        if (anaIsik != null) anaIsik.color = new Color(0.7f, 0.7f, 0.8f);
        anaIsik.intensity = 0.5f; // Normali 1'dir, 0.5 yaparak ortamı loşlaştırırız.
        if (tabletPanel != null) tabletPanel.SetActive(false);
        if (gorev2Bitti) // Veya senin sisteminde görevlerin bittiğini anlatan şart neyse
        {
            YeniGorevBildirimi(); // İşte bu satır gidip o ünlemi yakar!
        }
        isTabletOpen = false;

        // 2. Görev yazılarını (HUD) ekranda göster
        if (gorevHUD != null) gorevHUD.SetActive(true);

        // 3. İlgili görev grubunu aktif et, diğerlerini kapat
        for (int i = 0; i < gorevGruplari.Length; i++)
        {
            // Eğer döngüdeki index, seçilen görevin bir eksiği ise onu aç
            // (ID 1 ise index 0'ı açar)
            gorevGruplari[i].SetActive(i == (secilenGorevID - 1));
        }

        // 4. Karakteri veya Kamerayı ışınla
        if (player != null && operasyonBolgesi != null)
        {
            player.transform.position = operasyonBolgesi.position;
            if (player != null && operasyonBolgesi != null)
            {
                // Bakış açısını da başlangıç noktasına göre eşitleyelim:
                player.transform.rotation = operasyonBolgesi.rotation;
            }
        }

    }
}