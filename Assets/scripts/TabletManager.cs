using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class TabletManager : MonoBehaviour
{
    // ====================================================================
    // YENİ: ENVANTER VE VERİ MİMARİSİ KÖPRÜLERİ
    // ====================================================================
    public static TabletManager Instance; // Rapor sisteminin buraya ulaşabilmesi için köprü

    [Header("Envanter Veri Ayarları (YENİ)")]
    public List<ItemData> tumEsyalar; // Oluşturduğumuz ScriptableObject kartları buraya gelecek
    // ====================================================================

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
    private EvVerisi suAnkiSeciliEv; 

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
    public TMP_Text cekicMiktarText; 
    public TMP_Text balyozMiktarText;
    public TMP_Text egimOlcerMiktarText;
    public TMP_Text betonSertlikMiktarText;
    public TMP_Text yardimKitiMiktarText;
    public TMP_Text BetonMiktarText;
    public TMP_Text CamMiktarText;

    public int balyozSayisi = 0;
    public int cekicSayisi = 0; 
    public int cimentoSayisi = 0;
    public int demirSayisi = 0;
    public int tuglaSayisi = 0;
    public int tamirKitiSayisi = 0;
    public int egimOlcerSayisi = 0;
    public int betonSertlikOlcerSayisi = 0;
    public int yardimKitiSayisi = 0;
    public int CamSayisi = 0;
    public int BetonSayisi = 0;

    [Header("Görevler Sistemi (Operasyon Merkezi)")]
    public TMP_Text sagBaslikText;
    public TMP_Text sagDetayText;
    public TMP_Text sagGereksinimText;
    public int kontrolEdilenBinaSayisi = 0; 
    private int secilenGorevID = -1;
    private bool isTabletOpen = false;

    [Header("Hedef ve Minimap Sistemi")] 
    public GameObject haritaIsaretcisi; 
    public TextMeshProUGUI hataMesaji;
    public int oyuncuSeviyesi = 1; 
    public Button[] gorevButonlari; 
    public int bilincPuani = 0; 
    public int seviyeAtlamaSiniri = 100; 
    [Header("Seviye ve Puan Sistemi")]

    [Header("Dinamik Görev Sistemi")]
    public GameObject bildirimUnlemi; 
    public GameObject gorevBasariliPanel; 

    public TMP_Text bilincPuaniText; 
    public GameObject[] gorevGruplari; 
    public Transform operasyonBolgesi; 
    public GameObject player; 
    public Light anaIsik; 
    public AudioSource sesKaynagi;
    bool gorev2Bitti = false; 
    public bool isGorevBinasi = false; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
            isTabletOpen = false;
        }

        oyuncuParasi = PlayerPrefs.GetInt("K_Para", 500);
        cimentoSayisi = PlayerPrefs.GetInt("K_Cimento", 0);
        demirSayisi = PlayerPrefs.GetInt("K_Demir", 0);
        tamirKitiSayisi = PlayerPrefs.GetInt("K_TamirKiti", 0);

        kontrolEdilenBinaSayisi = 0;

        ParaYazisiniGuncelle();
        MiktarlariGuncelle();
        BasvurulariYenile();
        SeviyeKontrolEt();
    }

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

    // ====================================================================
    // MAĞAZA FONKSİYONU: SCRIPTABLEOBJECT KARTLARINA VE GÜVENLİ KÖPRÜYE BAĞLANDI
    // ====================================================================
    public void EsyaSatinAl(int esyaID)
    {
        ItemData alinacakEsya = tumEsyalar.Find(x => x.esyaID == esyaID);

        if (alinacakEsya == null) return;

        int fiyat = alinacakEsya.satinAlmaFiyati;
        string esyaAdi = alinacakEsya.esyaAdi;

        if (oyuncuParasi >= fiyat)
        {
            oyuncuParasi -= fiyat;
            if (esyaID == 0) cimentoSayisi++;
            else if (esyaID == 1) demirSayisi++;
            else if (esyaID == 2) tuglaSayisi++;
            else if (esyaID == 3) tamirKitiSayisi++;
            else if (esyaID == 4) cekicSayisi++;
            else if (esyaID == 5) balyozSayisi++;
            else if (esyaID == 6) egimOlcerSayisi++;
            else if (esyaID == 7) betonSertlikOlcerSayisi++;
            else if (esyaID == 8) yardimKitiSayisi++;
            else if (esyaID == 9) CamSayisi++;
            else if (esyaID == 10) BetonSayisi++;

            ParaYazisiniGuncelle();
            MiktarlariGuncelle();
            
            Debug.Log("<color=yellow>[Mağaza] Başarıyla satın alındı: </color>" + esyaAdi);

            // ====================================================================
            // GÜVENLİ ENVANTER BAĞLANTI SİHİRBAZI (Hata Almayı Önler)
            // Arkadaşının bilgisayarında PlayerInventory olmasa bile hata VERMEZ!
            // ====================================================================
            System.Type envanterTipi = System.Type.GetType("PlayerInventory");
            if (envanterTipi != null)
            {
                Component envanter = FindObjectOfType(envanterTipi) as Component;
                if (envanter != null)
                {
                    envanter.GetType().GetMethod("EsyaEkle")?.Invoke(envanter, new object[] { alinacakEsya, 1 });
                }
            }
            // ====================================================================
        }
    }

    public void ParaYazisiniGuncelle() { if (bakiyeEkrani != null) bakiyeEkrani.text = oyuncuParasi + " TL"; }
    
    // BİZİM SİLECEĞİMİZ YER TAM OLARAK BURASIYDI:
    // MiktarlariGuncelle fonksiyonunun içindeki o dükkan textlerini ezen kısımları sildik.
    // Arkadaşın dükkandaki textlerin bağlantısını Inspector'dan tamamen kopardığı an, buton fiyatları sabit kalacak!
    public void MiktarlariGuncelle()
    {
        if (cimentoMiktarText != null) cimentoMiktarText.text = "x" + cimentoSayisi;
        if (demirMiktarText != null) demirMiktarText.text = "x" + demirSayisi;
        if (tuglaMiktarText != null) tuglaMiktarText.text = "x" + tuglaSayisi;
        if (tamirKitiMiktarText != null) tamirKitiMiktarText.text = "x" + tamirKitiSayisi;
        if (cekicMiktarText != null) cekicMiktarText.text = "x" + cekicSayisi;
        if (balyozMiktarText != null) balyozMiktarText.text = "x" + balyozSayisi;
        if (egimOlcerMiktarText != null) egimOlcerMiktarText.text = "x" + egimOlcerSayisi;
        if (betonSertlikMiktarText != null) betonSertlikMiktarText.text = "x" + betonSertlikOlcerSayisi;
        if (yardimKitiMiktarText != null) yardimKitiMiktarText.text = "x" + yardimKitiSayisi;
        if (BetonMiktarText != null) BetonMiktarText.text = "x" + BetonSayisi;
        if (CamMiktarText != null) CamMiktarText.text = "x" + CamSayisi;
    }

    public void OpenBasvurular() { KapatTumSayfalar(); basvurularPage.SetActive(true); }
    public void OpenGorevler()
    { 
        KapatTumSayfalar();
        gorevlerPage.SetActive(true);

        if (bildirimUnlemi != null)
        {
            bildirimUnlemi.SetActive(false);
        }
    } 
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

    public void BasvurulariYenile()
    {
        Debug.Log("SİSTEM: Ev listesi yenileme işlemi başladı!"); 

        secilenGununEvleri.Clear();
        List<EvVerisi> geciciListe = new List<EvVerisi>(tumEvHavuzu);

        for (int i = 0; i < 3 && geciciListe.Count > 0; i++)
        {
            int rastgeleIndex = Random.Range(0, geciciListe.Count);
            secilenGununEvleri.Add(geciciListe[rastgeleIndex]);

            if (solButonYazilari[i] != null)
            {
                solButonYazilari[i].text = secilenGununEvleri[i].houseName;
                Debug.Log("SİSTEM: Buton " + i + " ismi '" + secilenGununEvleri[i].houseName + "' olarak değiştirildi."); 
            }

            geciciListe.RemoveAt(rastgeleIndex);
        }
    }

    public void GorevTamamlandi()
    {
        BilincPuaniArtir(100);

        if (gorevBasariliPanel != null)
        {
            gorevBasariliPanel.SetActive(true);
        }

        if (gorevHUD != null)
        {
            gorevHUD.SetActive(false);
        }

        Invoke("KapatBasariEkrani", 3f);
    }

    void KapatBasariEkrani()
    {
        if (gorevBasariliPanel != null)
        {
            gorevBasariliPanel.SetActive(false);
        }

        isTabletOpen = false;
        tabletPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GorevAlButonunaBasildi()
    {
        if (suAnkiSeciliEv != null && haritaIsaretcisi != null)
        {
            haritaIsaretcisi.transform.position = suAnkiSeciliEv.houseLocation;
            haritaIsaretcisi.SetActive(true);
            Debug.Log(suAnkiSeciliEv.houseName + " için başvuru kabul edildi, işaretçi güncellenedi.");
        }
    }
    public void BinaKontrolEt()
    {
        kontrolEdilenBinaSayisi++; 
        Debug.Log("Kontrol Edilen Bina: " + kontrolEdilenBinaSayisi);

        if (kontrolEdilenBinaSayisi >= 3)
        {
            GorevTamamlandi(); 
            kontrolEdilenBinaSayisi = 0; 
        }
    }
    public void UpdateUI()
    {
        if (bakiyeEkrani != null)
            bakiyeEkrani.text = oyuncuParasi.ToString() + " TL";

        if (bilincPuaniText != null)
            bilincPuaniText.text = "Bilinç Puanı: " + bilincPuani + " / " + seviyeAtlamaSiniri;

        Debug.Log("Arayüz ve Puanlar güncellendi.");
    }

    public void BilincPuaniArtir(int miktar)
    {
        bilincPuani += miktar;
        Debug.Log("Puan Eklendi! Yeni Puan: " + bilincPuani);

        if (bilincPuani >= seviyeAtlamaSiniri && oyuncuSeviyesi == 1)
        {
            oyuncuSeviyesi = 2;
            SeviyeKontrolEt(); 
            HataMesajiniGoster("TEBRİKLER! Seviye 2'ye ulaştınız.");
        }

        UpdateUI(); 
        VerileriKaydet(); 
    }

    public void HataMesajiniGoster(string mesaj)
    {
        hataMesaji.text = mesaj;
        hataMesaji.gameObject.SetActive(true); 
        Invoke("HataMesajiniGizle", 5f); 
    }

    public void SonrakiGoreviHazirla(int tamamlananGorevIndex)
    {
        gorevGruplari[tamamlananGorevIndex].SetActive(false);

        if (tamamlananGorevIndex + 1 < gorevGruplari.Length)
        {
            gorevGruplari[tamamlananGorevIndex + 1].SetActive(true);
            SeviyeKontrolEt();
            YeniGorevBildirimi(); 
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
            suAnkiSeciliEv = secilenGununEvleri[basvuruID]; 
            baslikText.text = suAnkiSeciliEv.houseName;
            detayText.text = "<b>Başvuru Sahibi:</b> " + suAnkiSeciliEv.applicantName +
                             "\n<b>Adres:</b> " + suAnkiSeciliEv.address +
                             "\n<b>Tarih:</b> " + suAnkiSeciliEv.date +
                             "\n\n<b>RAPOR:</b> " + suAnkiSeciliEv.reportDetail;
        }
    }

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
        gorevButonlari[0].gameObject.SetActive(true);

        if (oyuncuSeviyesi >= 2)
        {
            if (gorevButonlari[1].gameObject.activeSelf == false) 
            {
                gorevButonlari[1].gameObject.SetActive(true); 
                bildirimUnlemi.SetActive(true); 
            }
        }
        else
        {
            gorevButonlari[1].gameObject.SetActive(false); 
            if (oyuncuSeviyesi >= 3)
            {
                if (gorevButonlari[2].gameObject.activeSelf == false)
                {
                    gorevButonlari[2].gameObject.SetActive(true);
                    bildirimUnlemi.SetActive(true); 
                }
            }
            else
            {
                gorevButonlari[2].gameObject.SetActive(false);
            }
        }
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

        if (secilenGorevID == 1)
        {
            BaslatVeIsinla(); 
        }
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

    private void BaslatVeIsinla()
    {
        VerileriKaydet();
        if (sesKaynagi != null) sesKaynagi.Play();
        if (anaIsik != null) anaIsik.color = new Color(0.7f, 0.7f, 0.8f);
        anaIsik.intensity = 0.5f; 
        if (tabletPanel != null) tabletPanel.SetActive(false);
        if (gorev2Bitti) 
        {
            YeniGorevBildirimi(); 
        }
        isTabletOpen = false;

        if (gorevHUD != null) gorevHUD.SetActive(true);

        for (int i = 0; i < gorevGruplari.Length; i++)
        {
            gorevGruplari[i].SetActive(i == (secilenGorevID - 1));
        }

        if (player != null && operasyonBolgesi != null)
        {
            player.transform.position = operasyonBolgesi.position;
            player.transform.rotation = operasyonBolgesi.rotation;
        }
    }
}