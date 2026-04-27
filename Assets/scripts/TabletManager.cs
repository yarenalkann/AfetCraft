using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    public int cimentoSayisi = 0;
    public int demirSayisi = 0;
    public int tuglaSayisi = 0;
    public int tamirKitiSayisi = 0;

    [Header("Görevler Sistemi (Operasyon Merkezi)")]
    public TMP_Text sagBaslikText;   
    public TMP_Text sagDetayText;
    public TMP_Text sagGereksinimText;

    private int secilenGorevID = -1; 
    private bool isTabletOpen = false;

    [Header("Hedef ve Minimap Sistemi")] // <--- YENİ EKLENEN KISIM
    public GameObject haritaIsaretcisi; // Minimap'teki pin objesi

    void Start()
    {
        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
            isTabletOpen = false;
        }
        
        ParaYazisiniGuncelle();
        MiktarlariGuncelle();
        BasvurulariYenile(); 
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

    // ---- EKONOMİ VE MAĞAZA SİSTEMİ (DOKUNULMADI) ----
    public void EsyaSatinAl(int esyaID)
    {
        int fiyat = 0;
        string esyaAdi = "";
        if (esyaID == 0) { fiyat = 150; esyaAdi = "Çimento"; }
        else if (esyaID == 1) { fiyat = 250; esyaAdi = "Demir"; }
        else if (esyaID == 2) { fiyat = 50;  esyaAdi = "Tuğla"; }
        else if (esyaID == 3) { fiyat = 100; esyaAdi = "Tamir Kiti"; }

        if (oyuncuParasi >= fiyat)
        {
            oyuncuParasi -= fiyat;
            if (esyaID == 0) cimentoSayisi++;
            else if (esyaID == 1) demirSayisi++;
            else if (esyaID == 2) tuglaSayisi++;
            else if (esyaID == 3) tamirKitiSayisi++;
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
    }

    // ---- SAYFA DEĞİŞTİRME SİSTEMİ (DOKUNULMADI) ----
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
    public void BasvurulariYenile()
    {
        Debug.Log("SİSTEM: Ev listesi yenileme işlemi başladı!"); // TEST MESAJI 1

        secilenGununEvleri.Clear();
        List<EvVerisi> geciciListe = new List<EvVerisi>(tumEvHavuzu);

        for (int i = 0; i < 3 && geciciListe.Count > 0; i++)
        {
            int rastgeleIndex = Random.Range(0, geciciListe.Count);
            secilenGununEvleri.Add(geciciListe[rastgeleIndex]);

            if(solButonYazilari[i] != null)
            {
                solButonYazilari[i].text = secilenGununEvleri[i].evAdi;
                Debug.Log("SİSTEM: Buton " + i + " ismi '" + secilenGununEvleri[i].evAdi + "' olarak değiştirildi."); // TEST MESAJI 2
            }
                
            geciciListe.RemoveAt(rastgeleIndex);
        }
    }

    public void GorevAlButonunaBasildi()
    {
        if (suAnkiSeciliEv != null && haritaIsaretcisi != null)
        {
            // İşaretçiyi evin Vector3 konumuna ışınla
            haritaIsaretcisi.transform.position = suAnkiSeciliEv.evKonumu;
            haritaIsaretcisi.SetActive(true);
            
            Debug.Log(suAnkiSeciliEv.evAdi + " için başvuru kabul edildi, işaretçi güncellendi.");
             
        }
    }

    public void BasvuruGoster(int basvuruID)
    {
        if (basvuruID < secilenGununEvleri.Count)
        {
            suAnkiSeciliEv = secilenGununEvleri[basvuruID]; // <--- SEÇİLENİ HAFIZAYA ALDIK
            baslikText.text = suAnkiSeciliEv.evAdi;
            detayText.text = "<b>Başvuru Sahibi:</b> " + suAnkiSeciliEv.basvuruSahibi + 
                             "\n<b>Adres:</b> " + suAnkiSeciliEv.adres + 
                             "\n<b>Tarih:</b> " + suAnkiSeciliEv.tarih + 
                             "\n\n<b>RAPOR:</b> " + suAnkiSeciliEv.raporDetayi;
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

    public void OperasyonuBaslat()
    {
        Debug.Log("<color=green>BUTONA BASILDI!</color>"); 
        Debug.Log("Seçili Görev ID'si: " + secilenGorevID);

        if (secilenGorevID == -1) 
        {
            Debug.Log("<color=red>HATA: Görev seçilmedi!</color>");
            return;
        }

        if (secilenGorevID == 1)
        {
            Debug.Log("Sahne yükleniyor...");
            SceneManager.LoadScene("Level1_Enkaz"); 
        }
    }
}