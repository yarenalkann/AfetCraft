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
        [HideInInspector] public ItemData icindekiEsya; 
    }

    [Header("Envanter Slotları (Orta Alan)")]
    public List<DinamikUIKutusu> uiKutulari = new List<DinamikUIKutusu>();

    [Header("Hızlı Kullanım Slotları (Sol Alttaki 6 Kutu)")]
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

    // O an hangi kategorinin açık olduğunu tutar (Başta Araç Gereçler açık)
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
        AraçGereçSekmesiniSec(); // Oyun açıldığında araç gereçler listelensin
    }

    // ====================================================================
    // BUTONLARA ATANACAK KATEGORİ SEÇİM FONKSİYONLARI
    // ====================================================================
    public void AraçGereçSekmesiniSec()
    {
        mevcutKategori = ItemData.EnvanterKategorisi.AracGerecler;
        ResetleDetayPaneli();
        EnvanterArayuzunuYenile();
    }

    public void ÜretimMalzemesiSekmesiniSec()
    {
        mevcutKategori = ItemData.EnvanterKategorisi.UretimMalzemeleri;
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

    // GÜNCELLENEN AKILLI FİLTRELEME MOTORU
    public void EnvanterArayuzunuYenile()
    {
        if (PlayerInventory.Instance == null) return;

        // 1. ÖNCE TÜM ORTA ALAN SLOTLARINI SIFIRLA VE İÇİNİ GİZLE
        for (int i = 0; i < uiKutulari.Count; i++)
        {
            var kutu = uiKutulari[i];
            kutu.icindekiEsya = null;
            if (kutu.esyaIkonResmi != null) kutu.esyaIkonResmi.gameObject.SetActive(false);
            if (kutu.adetYazesi != null) kutu.adetYazesi.gameObject.SetActive(false);
            uiKutulari[i] = kutu;
        }

        // Çantadaki ham listeyi çekiyoruz
        List<PlayerInventory.EnvanterSlotu> cantaListesi = PlayerInventory.Instance.GetCantaListesi();
        
        // 2. SADECE O AN SEÇİLİ KATEGORİYE UYALANLARI FİLTRELEYİP YENİ BİR LİSTEYE ATIYORUZ
        List<PlayerInventory.EnvanterSlotu> filtrelenmisListe = new List<PlayerInventory.EnvanterSlotu>();
        foreach (var slot in cantaListesi)
        {
            if (slot.esya.esyaKategorisi == mevcutKategori)
            {
                filtrelenmisListe.Add(slot);
            }
        }

        // 3. FİLTRELENMİŞ EŞYALARI SIRAYLA MOR KUTULARA YERLEŞTİR
        for (int i = 0; i < filtrelenmisListe.Count; i++)
        {
            if (i >= uiKutulari.Count) break;

            var cantaSlotu = filtrelenmisListe[i];
            var hedefUIKutusu = uiKutulari[i];

            hedefUIKutusu.icindekiEsya = cantaSlotu.esya;

            if (hedefUIKutusu.esyaIkonResmi != null)
            {
                hedefUIKutusu.esyaIkonResmi.sprite = cantaSlotu.esya.esyaIkonu;
                hedefUIKutusu.esyaIkonResmi.gameObject.SetActive(true);
            }

            if (hedefUIKutusu.adetYazesi != null)
            {
                hedefUIKutusu.adetYazesi.text = "x" + cantaSlotu.adet;
                hedefUIKutusu.adetYazesi.gameObject.SetActive(true);
            }
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
            hizliKullanımKutulari[i] = kutu;
        }
    }

    // SAĞ DETAY PANELİNİ DOLDURURKEN BUTONLARI AÇIP KAPATAN KRİTİK YER
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

        // ====================================================================
        // TAM İSTEDİĞİN BUTON GİZLEME SİHRE BURASI:
        // ====================================================================
        if (seciliEsya.esyaKategorisi == ItemData.EnvanterKategorisi.UretimMalzemeleri)
        {
            // Eğer çimento, demir vb. ise butonları kapat, sadece açıklama ve resim kalsın!
            if (kullanButonu != null) kullanButonu.SetActive(false);
            if (birakButonu != null) birakButonu.SetActive(false);
            if (hizliKullanimButonu != null) hizliKullanimButonu.SetActive(false);
        }
        else
        {
            // Eğer alet edevatsa tüm butonları geri aç!
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
        if (seciliEsya == null) return;

        foreach (var kutu in hizliKullanımKutulari)
        {
            if (kutu.icindekiEsya == seciliEsya) return;
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
                int adet = PlayerInventory.Instance.EsyaAdetiniGetir(seciliEsya);
                if (kutu.adetYazesi != null)
                {
                    kutu.adetYazesi.text = "x" + adet;
                    kutu.adetYazesi.gameObject.SetActive(true);
                }
                hizliKullanımKutulari[i] = kutu; 
                return;
            }
        }
    }
}