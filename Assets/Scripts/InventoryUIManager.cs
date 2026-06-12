using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [System.Serializable]
    public struct DinamikUIKutusu
    {
        public GameObject slotAnaObjesi;     
        public Image esyaIkonResmi;          
        public TextMeshProUGUI adetYazesi; 
        public UnityEngine.UI.Image dayaniklilikBarDolgusu;  
        [HideInInspector] public ItemData icindekiEsya; 
    }

    [Header("Envanter Slotları (Orta Alan)")]
    public List<DinamikUIKutusu> uiKutulari = new List<DinamikUIKutusu>();

    [Header("Hızlı Kullanım Slotları")]
    public List<DinamikUIKutusu> hizliKullanımKutulari = new List<DinamikUIKutusu>();

    [Header("Sağ Detay Paneli Objeleri")]
    public TextMeshProUGUI detayEsyaAdiYazisi;       
    public TextMeshProUGUI detayEsyaAciklamaYazisi;  
    public Image detayEsyaModelResmi;                
    public GameObject detayPaneliAnaObjesi;          

    [Header("Sağ Panel İşlem Butonları")]
    public GameObject kullanButonu;       // Mavi buton
    public GameObject birakButonu;         // Kırmızı buton
    public GameObject hizliKullanimButonu; // Yeşil büyük buton

    [Header("Kategori Buton Yazıları (TMP)")]
    public TextMeshProUGUI aracGereclerButonYazisi;
    public TextMeshProUGUI uretimMalzemeleriButonYazisi;

    [Header("Renk Paleti")]
    public Color aktifYaziRengi = new Color(1f, 0.6f, 0f);     // Turuncu/Sarı başlık rengin
    public Color pasifYaziRengi = new Color(0.5f, 0.5f, 0.5f); // Sönük gri rengin

    private ItemData.EnvanterKategorisi mevcutKategori = ItemData.EnvanterKategorisi.AracGerecler;
    private ItemData seciliEsya;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetleDetayPaneli();
        HizliKullanimSlotlariniIlkKezTemizle();
        AraçGereçSekmesiniSec(); 
    }

    private void HizliKullanimSlotlariniIlkKezTemizle()
    {
        for (int i = 0; i < hizliKullanımKutulari.Count; i++)
        {
            var kutu = hizliKullanımKutulari[i];
            kutu.icindekiEsya = null; 

            if (kutu.esyaIkonResmi != null) kutu.esyaIkonResmi.gameObject.SetActive(false);
            if (kutu.adetYazesi != null) kutu.adetYazesi.gameObject.SetActive(false);
            
            // Hızlı kullanımda bar istemediğimiz için ilk açılışta da mutlaka kapatıyoruz
            if (kutu.dayaniklilikBarDolgusu != null)
            {
                kutu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(false);
            }
            
            hizliKullanımKutulari[i] = kutu; 
        }
    }

    public void AraçGereçSekmesiniSec()
    {
        mevcutKategori = ItemData.EnvanterKategorisi.AracGerecler;
        if (aracGereclerButonYazisi != null) aracGereclerButonYazisi.color = aktifYaziRengi;
        if (uretimMalzemeleriButonYazisi != null) uretimMalzemeleriButonYazisi.color = pasifYaziRengi;

        ResetleDetayPaneli();
        EnvanterArayuzunuYenile();
    }

    public void ÜretimMalzemesiSekmesiniSec()
    {
        mevcutKategori = ItemData.EnvanterKategorisi.UretimMalzemeleri;
        if (aracGereclerButonYazisi != null) aracGereclerButonYazisi.color = pasifYaziRengi;
        if (uretimMalzemeleriButonYazisi != null) uretimMalzemeleriButonYazisi.color = aktifYaziRengi;

        ResetleDetayPaneli();
        EnvanterArayuzunuYenile();
    }

    public void ResetleDetayPaneli()
    {
        if (detayPaneliAnaObjesi != null) detayPaneliAnaObjesi.SetActive(false);
        if (detayEsyaAdiYazisi != null) detayEsyaAdiYazisi.text = "";
        if (detayEsyaAciklamaYazisi != null) detayEsyaAciklamaYazisi.text = "";
        if (detayEsyaModelResmi != null) detayEsyaModelResmi.gameObject.SetActive(false);
        seciliEsya = null;
    }

    // GÜNCELLENEN TAM KORUMALI YENİLEME MOTORU
// ====================================================================
    // TAM KORUMALI VE KESİN ÇÖZÜMLÜ YENİLEME MOTORU
    // ====================================================================
    public void EnvanterArayuzunuYenile()
    {
        if (PlayerInventory.Instance == null) return;

        // 1. ADIM: Çantadaki güncel filtrelenmiş listeyi hazırla
        List<PlayerInventory.EnvanterSlotu> cantaListesi = PlayerInventory.Instance.GetCantaListesi();
        List<PlayerInventory.EnvanterSlotu> filtrelenmisListe = new List<PlayerInventory.EnvanterSlotu>();
        
        foreach (var slot in cantaListesi)
        {
            if (slot.esya.esyaKategorisi == mevcutKategori)
            {
                filtrelenmisListe.Add(slot);
            }
        }

        // 2. ADIM: Tek bir döngüde tüm slotları ya doldur ya da KESİN OLARAK KARTLARINI SÖNDÜR!
        for (int i = 0; i < uiKutulari.Count; i++)
        {
            var hedefUIKutusu = uiKutulari[i];

            // Eğer o indekste bir eşya VARSA (Slot DOLUYSA)
            if (i < filtrelenmisListe.Count)
            {
                var cantaSlotu = filtrelenmisListe[i];
                hedefUIKutusu.icindekiEsya = cantaSlotu.esya;

                // İkonu aç ve yerleştir
                if (hedefUIKutusu.esyaIkonResmi != null)
                {
                    hedefUIKutusu.esyaIkonResmi.sprite = cantaSlotu.esya.esyaIkonu;
                    hedefUIKutusu.esyaIkonResmi.gameObject.SetActive(true);
                }

                // Adet Yazısı Kontrolü
                if (hedefUIKutusu.adetYazesi != null)
                {
                    if (cantaSlotu.esya.adetYazisiGosterilsinMi)
                    {
                        hedefUIKutusu.adetYazesi.text = "x" + cantaSlotu.adet;
                        hedefUIKutusu.adetYazesi.gameObject.SetActive(true);
                    }
                    else
                    {
                        hedefUIKutusu.adetYazesi.gameObject.SetActive(false);
                    }
                }

                // Dayanıklılık Barı Kontrolü
                if (hedefUIKutusu.dayaniklilikBarDolgusu != null)
                {
                    if (cantaSlotu.esya.esyaTipi == ItemData.EsyaTuru.DayanikliAlet)
                    {
                        float canYuzdesi = cantaSlotu.guncelDayaniklilik / cantaSlotu.esya.maksimumDayaniklilik;
                        
                        hedefUIKutusu.dayaniklilikBarDolgusu.rectTransform.localScale = new Vector3(canYuzdesi, 1f, 1f);
                        hedefUIKutusu.dayaniklilikBarDolgusu.color = Color.Lerp(Color.red, Color.green, canYuzdesi);
                        
                        // Siyah çerçeveyi (parent) görünür yap
                        hedefUIKutusu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        // Dayanıklı alet değilse (Çimento veya Kalıcı cihazsa) barı gizle
                        hedefUIKutusu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
            else // SİHİRLİ DOKUNUŞ: Eğer o indekste hiçbir eşya yoksa (Slot BOŞSA) her şeyi KESİN OLARAK SÖNDÜR!
            {
                hedefUIKutusu.icindekiEsya = null;

                if (hedefUIKutusu.esyaIkonResmi != null) 
                    hedefUIKutusu.esyaIkonResmi.gameObject.SetActive(false);
                
                if (hedefUIKutusu.adetYazesi != null) 
                    hedefUIKutusu.adetYazesi.gameObject.SetActive(false);

                // Boş slotta hayalet bar kalmasını engelleyen nokta atışı darbe:
                if (hedefUIKutusu.dayaniklilikBarDolgusu != null)
                {
                    hedefUIKutusu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(false);
                }
            }

            // Struct yapısını güncelleyip listeye geri yazıyoruz
            uiKutulari[i] = hedefUIKutusu;
        }

        HizliKullanımAdetleriniGuncelle();
    }

    private void HizliKullanımAdetleriniGuncelle()
    {
        if (PlayerInventory.Instance == null) return;

        for (int i = 0; i < hizliKullanımKutulari.Count; i++)
        {
            var kutu = hizliKullanımKutulari[i];
            
            if (kutu.icindekiEsya != null)
            {
                if (kutu.icindekiEsya.ustUsteBiniyorMu)
                {
                    int guncelAdet = PlayerInventory.Instance.EsyaAdetiniGetir(kutu.icindekiEsya);
                    
                    if (guncelAdet <= 0) 
                    {
                        kutu.icindekiEsya = null;
                        if (kutu.esyaIkonResmi != null) kutu.esyaIkonResmi.gameObject.SetActive(false);
                        if (kutu.adetYazesi != null) kutu.adetYazesi.gameObject.SetActive(false);
                    }
                    else 
                    {
                        if (kutu.adetYazesi != null) kutu.adetYazesi.text = "x" + guncelAdet;
                    }
                }
                else
                {
                    var guncelCantaListesi = PlayerInventory.Instance.GetCantaListesi();
                    bool aletHalaCantadaVarMi = false;

                    foreach (var slot in guncelCantaListesi)
                    {
                        if (slot.esya == kutu.icindekiEsya)
                        {
                            aletHalaCantadaVarMi = true;
                            break;
                        }
                    }

                    if (!aletHalaCantadaVarMi)
                    {
                        kutu.icindekiEsya = null;
                        if (kutu.esyaIkonResmi != null) kutu.esyaIkonResmi.gameObject.SetActive(false);
                        if (kutu.adetYazesi != null) kutu.adetYazesi.gameObject.SetActive(false);
                    }
                }
            }
            
            // Hızlı kullanımda bar istemediğimiz için ne olursa olsun gizliyoruz
            if (kutu.dayaniklilikBarDolgusu != null)
            {
                kutu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(false);
            }

            hizliKullanımKutulari[i] = kutu; 
        }
    }

    public void SlotSecildi(int slotIndex)
    {
        Debug.Log($"<color=yellow>[Tıklama Testi] {slotIndex}. slot tıklandı!</color>");
        
        if (slotIndex >= uiKutulari.Count || uiKutulari[slotIndex].icindekiEsya == null) return;

        seciliEsya = uiKutulari[slotIndex].icindekiEsya;

        if (detayPaneliAnaObjesi != null) detayPaneliAnaObjesi.SetActive(true);
        if (detayEsyaAdiYazisi != null) detayEsyaAdiYazisi.text = seciliEsya.esyaAdi.ToUpper();
        if (detayEsyaAciklamaYazisi != null) detayEsyaAciklamaYazisi.text = seciliEsya.esyaAciklamasi; 
        if (detayEsyaModelResmi != null)
        {
            detayEsyaModelResmi.sprite = seciliEsya.esyaIkonu;
            detayEsyaModelResmi.gameObject.SetActive(true);
        }

        if (seciliEsya.esyaKategorisi == ItemData.EnvanterKategorisi.UretimMalzemeleri)
        {
            if (kullanButonu != null) kullanButonu.SetActive(false);
            if (birakButonu != null) birakButonu.SetActive(false);
            if (hizliKullanimButonu != null) hizliKullanimButonu.SetActive(false);
        }
        else
        {
            if (kullanButonu != null) kullanButonu.SetActive(true);
            if (birakButonu != null) birakButonu.SetActive(true);
            if (hizliKullanimButonu != null) hizliKullanimButonu.SetActive(true);
        }
    }

    public void SeciliEsyayiKullan()
    {
        if (seciliEsya == null || PlayerInventory.Instance == null) return;
        bool basariliMi = PlayerInventory.Instance.EsyaKullan(seciliEsya, 20f);
        if (basariliMi)
        {
            if (PlayerInventory.Instance.EsyaAdetiniGetir(seciliEsya) <= 0) ResetleDetayPaneli();
            EnvanterArayuzunuYenile();
        }
    }

    public void SeciliEsyayiBirak()
    {
        if (seciliEsya == null || PlayerInventory.Instance == null) return;
        bool basariliMi = PlayerInventory.Instance.EsyaKullan(seciliEsya, 100f);
        if (basariliMi)
        {
            if (PlayerInventory.Instance.EsyaAdetiniGetir(seciliEsya) <= 0) ResetleDetayPaneli();
            EnvanterArayuzunuYenile();
        }
    }

    public void SeciliEsyayıHizliKullanimaGonder()
    {
        if (seciliEsya == null || PlayerInventory.Instance == null) return;

        foreach (var kutu in hizliKullanımKutulari)
        {
            if (kutu.icindekiEsya == seciliEsya)
            {
                Debug.Log($"[Hızlı Kullanım] {seciliEsya.esyaAdi} zaten listede ekli!");
                return; 
            }
        }

        for (int i = 0; i < hizliKullanımKutulari.Count; i++)
        {
            var kutu = hizliKullanımKutulari[i];
            
            if (kutu.icindekiEsya == null) 
            {
                kutu.icindekiEsya = seciliEsya; 

                if (kutu.esyaIkonResmi != null)
                {
                    kutu.esyaIkonResmi.sprite = seciliEsya.esyaIkonu;
                    kutu.esyaIkonResmi.gameObject.SetActive(true);
                }

                if (kutu.adetYazesi != null)
                {
                    if (seciliEsya.adetYazisiGosterilsinMi)
                    {
                        int guncelAdet = PlayerInventory.Instance.EsyaAdetiniGetir(seciliEsya);
                        kutu.adetYazesi.text = "x" + guncelAdet;
                        kutu.adetYazesi.gameObject.SetActive(true);
                    }
                    else
                    {
                        kutu.adetYazesi.gameObject.SetActive(false);
                    }
                }

                // Hızlı kullanımda barı burada da kapatıyoruz
                if (kutu.dayaniklilikBarDolgusu != null)
                {
                    kutu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(false);
                }

                hizliKullanımKutulari[i] = kutu; 
                Debug.Log($"[Hızlı Kullanım] {seciliEsya.esyaAdi} başarıyla Hızlı Kullanım Slot {i} alanına atandı!");
                return;
            }
        }

        Debug.LogWarning("[Hızlı Kullanım] Tüm hızlı kullanım slotları dolu!");
    }

    public void HizliKullanimSagTiklandi(int slotIndex)
    {
        if (slotIndex >= hizliKullanımKutulari.Count) return;

        var kutu = hizliKullanımKutulari[slotIndex];
        if (kutu.icindekiEsya == null) return;

        Debug.Log($"[Hızlı Kullanım] {kutu.icindekiEsya.esyaAdi} hızlı kullanımdan çıkarıldı!");

        kutu.icindekiEsya = null;
        if (kutu.esyaIkonResmi != null) kutu.esyaIkonResmi.gameObject.SetActive(false);
        if (kutu.adetYazesi != null) kutu.adetYazesi.gameObject.SetActive(false);
        
        if (kutu.dayaniklilikBarDolgusu != null)
        {
            kutu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(false);
        }

        hizliKullanımKutulari[slotIndex] = kutu;
        EnvanterArayuzunuYenile();
    }
}