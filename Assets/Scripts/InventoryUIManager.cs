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
        public GameObject slotAnaObjesi;     // Mor kare kutu (Arka plan)
        public Image esyaIkonResmi;          // İçindeki pikselli ikon
        public TextMeshProUGUI adetYazesi;   // İçindeki Text (Miktar için)
        [HideInInspector] public ItemData icindekiEsya; // O an bu slotta hangi eşya duruyor?
    }

    [Header("Envanter Slotları (Orta Alan)")]
    public List<DinamikUIKutusu> uiKutulari = new List<DinamikUIKutusu>();

    // ====================================================================
    // YENİ: HIZLI KULLANIM SLOTLARI (Sol Alttaki 6 Kutu)
    // ====================================================================
    [Header("Hızlı Kullanım Slotları (Sol Alttaki 6 Kutu)")]
    public List<DinamikUIKutusu> hizliKullanımKutulari = new List<DinamikUIKutusu>();

    // ====================================================================
    // YENİ: SAĞ DETAY PANELİ OBJELERİ (Senin Tasarımdakiler)
    // ====================================================================
    [Header("Sağ Detay Paneli Objeleri")]
    public TextMeshProUGUI detayEsyaAdiYazisi;       // En üstteki turuncu başlık
    public TextMeshProUGUI detayEsyaAciklamaYazisi;  // Deprem onarımı yazısı
    public Image detayEsyaModelResmi;                // Ortadaki pikselli 3D resim
    public GameObject detayPaneliAnaObjesi;          // Başta görünmez yapmak için

    // Hafızada o an seçili duran eşya kartı
    private ItemData seciliEsya;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Başta sağ tarafı ve içini resetle
        ResetleDetayPaneli();
        EnvanterArayuzunuYenile();
    }

    private void ResetleDetayPaneli()
    {
        if (detayPaneliAnaObjesi != null) detayPaneliAnaObjesi.SetActive(false);
        if (detayEsyaAdiYazisi != null) detayEsyaAdiYazisi.text = "";
        if (detayEsyaAciklamaYazisi != null) detayEsyaAciklamaYazisi.text = "";
        if (detayEsyaModelResmi != null) detayEsyaModelResmi.gameObject.SetActive(false);
        seciliEsya = null;
    }

    public void EnvanterArayuzunuYenile()
    {
        if (PlayerInventory.Instance == null) return;

        // 1. ORTA ALANI TEMİZLE VE DOLDUR (Sırayla Dizme)
        List<PlayerInventory.EnvanterSlotu> sahipOlunanEsyalar = PlayerInventory.Instance.GetCantaListesi();

        for (int i = 0; i < uiKutulari.Count; i++)
        {
            var kutu = uiKutulari[i];
            kutu.icindekiEsya = null; // Resetle

            if (kutu.esyaIkonResmi != null) kutu.esyaIkonResmi.gameObject.SetActive(false);
            if (kutu.adetYazesi != null) kutu.adetYazesi.gameObject.SetActive(false);

            if (i < sahipOlunanEsyalar.Count)
            {
                var cantaSlotu = sahipOlunanEsyalar[i];
                kutu.icindekiEsya = cantaSlotu.esya; // Kimlik atadık!

                if (kutu.esyaIkonResmi != null)
                {
                    kutu.esyaIkonResmi.sprite = cantaSlotu.esya.esyaIkonu;
                    kutu.esyaIkonResmi.gameObject.SetActive(true);
                }

                if (kutu.adetYazesi != null)
                {
                    kutu.adetYazesi.text = "x" + cantaSlotu.adet;
                    kutu.adetYazesi.gameObject.SetActive(true);
                }
            }
            uiKutulari[i] = kutu; // Geri atıyoruz
        }
    }

    // ====================================================================
    // YENİ: SLOTA TIKLANDIĞINDA SAĞ PANELİ DOLDURMA (SİHİRLİ FONKSİYON)
    // ====================================================================
    public void SlotSecildi(int slotIndex)
    {
        if (slotIndex >= uiKutulari.Count || uiKutulari[slotIndex].icindekiEsya == null) return;

        seciliEsya = uiKutulari[slotIndex].icindekiEsya;

        // Sağ paneldeki senin o harika yazılarını ve resimlerini güncelle
        if (detayPaneliAnaObjesi != null) detayPaneliAnaObjesi.SetActive(true);
        if (detayEsyaAdiYazisi != null) detayEsyaAdiYazisi.text = seciliEsya.esyaAdi.ToUpper();
        if (detayEsyaAciklamaYazisi != null) detayEsyaAciklamaYazisi.text = seciliEsya.esyaAciklamasi; 
        if (detayEsyaModelResmi != null)
        {
            // İstersen 3D model, istersen 2D pikselli resmi yansıtabiliriz
            // Biz şimdilik ItemData'daki ikonu basıyoruz
            detayEsyaModelResmi.sprite = seciliEsya.esyaIkonu;
            detayEsyaModelResmi.gameObject.SetActive(true);
        }
        Debug.Log($"[Sağ Panel] {seciliEsya.esyaAdi} detayları canlandı!");
    }

    // ====================================================================
    // YENİ: YEŞIL BUTONA BASILDIĞINDA MANUEL YERLEŞTİRME FONKSİYONU
    // ====================================================================
    public void SeciliEsyayıHizliKullanimaGonder()
    {
        if (seciliEsya == null) return;

        // Sol altta bu eşya zaten var mı kontrol et (aynı aletten iki tane koymasın)
        foreach (var kutu in hizliKullanımKutulari)
        {
            if (kutu.icindekiEsya == seciliEsya)
            {
                Debug.Log($"[Hızlı Kullanım] {seciliEsya.esyaAdi} zaten hotbar'da var!");
                return;
            }
        }

        // Sol alttaki boş olan İLK kutuyu bul ve yerleştir
        for (int i = 0; i < hizliKullanımKutulari.Count; i++)
        {
            var kutu = hizliKullanımKutulari[i];
            if (kutu.icindekiEsya == null) // Boş kutu bulduk!
            {
                kutu.icindekiEsya = seciliEsya; // Kimlik verdik!
                if (kutu.esyaIkonResmi != null)
                {
                    kutu.esyaIkonResmi.sprite = seciliEsya.esyaIkonu;
                    kutu.esyaIkonResmi.gameObject.SetActive(true);
                }
                // Adet takibini PlayerInventory'den çekiyoruz dinamik olarak
                int adet = PlayerInventory.Instance.EsyaAdetiniGetir(seciliEsya);
                if (kutu.adetYazesi != null)
                {
                    kutu.adetYazesi.text = "x" + adet;
                    kutu.adetYazesi.gameObject.SetActive(true);
                }
                hizliKullanımKutulari[i] = kutu; // Struct olduğu için geri atıyoruz
                Debug.Log($"[Hızlı Kullanım] {seciliEsya.esyaAdi} başarıyla Slot {i+1}'e yerleşti!");
                return;
            }
        }
        Debug.LogWarning("[Hızlı Kullanım] Hızlı kullanım slotları tamamen dolu!");
    }
}