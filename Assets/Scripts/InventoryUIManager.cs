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
        public TextMeshProUGUI adetYazesi; // Kod içindeki değişken adın adetYazesi olduğu için senkronize tutuldu
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
    public GameObject kullanButonu;       
    public GameObject birakButonu;         
    public GameObject hizliKullanimButonu; 
    public GameObject tamirEtButonu; // Sahnede tasarladığın o yeni butonu buraya sürükleyeceksin!

    [Header("Kategori Buton Yazıları (TMP)")]
    public TextMeshProUGUI aracGereclerButonYazisi;
    public TextMeshProUGUI uretimMalzemeleriButonYazisi;

    [Header("Renk Paleti")]
    public Color aktifYaziRengi = new Color(1f, 0.6f, 0f);     
    public Color pasifYaziRengi = new Color(0.5f, 0.5f, 0.5f); 

    private ItemData.EnvanterKategorisi mevcutKategori = ItemData.EnvanterKategorisi.AracGerecler;
    private ItemData seciliEsya;
    private int seciliSlotIndex; // YENİ: Tamir motorunun hangi aleti tamir edeceğini bilmesi için gizli hafıza

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
        if (tamirEtButonu != null) tamirEtButonu.SetActive(false); // Panel sıfırlanınca buton da gizlensin
        seciliEsya = null;
    }

    public void EnvanterArayuzunuYenile()
    {
        if (PlayerInventory.Instance == null) return;

        List<PlayerInventory.EnvanterSlotu> cantaListesi = PlayerInventory.Instance.GetCantaListesi();
        List<PlayerInventory.EnvanterSlotu> filtrelenmisListe = new List<PlayerInventory.EnvanterSlotu>();
        
        foreach (var slot in cantaListesi)
        {
            if (slot.esya.esyaKategorisi == mevcutKategori)
            {
                filtrelenmisListe.Add(slot);
            }
        }

        for (int i = 0; i < uiKutulari.Count; i++)
        {
            var hedefUIKutusu = uiKutulari[i];

            if (i < filtrelenmisListe.Count)
            {
                var cantaSlotu = filtrelenmisListe[i];
                hedefUIKutusu.icindekiEsya = cantaSlotu.esya;

                if (hedefUIKutusu.esyaIkonResmi != null)
                {
                    hedefUIKutusu.esyaIkonResmi.sprite = cantaSlotu.esya.esyaIkonu;
                    hedefUIKutusu.esyaIkonResmi.gameObject.SetActive(true);
                }

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

                if (hedefUIKutusu.dayaniklilikBarDolgusu != null)
                {
                    if (cantaSlotu.esya.esyaTipi == ItemData.EsyaTuru.DayanikliAlet)
                    {
                        float canYuzdesi = cantaSlotu.guncelDayaniklilik / cantaSlotu.esya.maksimumDayaniklilik;
                        
                        hedefUIKutusu.dayaniklilikBarDolgusu.rectTransform.localScale = new Vector3(canYuzdesi, 1f, 1f);
                        hedefUIKutusu.dayaniklilikBarDolgusu.color = Color.Lerp(Color.red, Color.green, canYuzdesi);
                        
                        hedefUIKutusu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        hedefUIKutusu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
            else 
            {
                hedefUIKutusu.icindekiEsya = null;

                if (hedefUIKutusu.esyaIkonResmi != null) 
                    hedefUIKutusu.esyaIkonResmi.gameObject.SetActive(false);
                
                if (hedefUIKutusu.adetYazesi != null) 
                    hedefUIKutusu.adetYazesi.gameObject.SetActive(false);

                if (hedefUIKutusu.dayaniklilikBarDolgusu != null)
                {
                    hedefUIKutusu.dayaniklilikBarDolgusu.transform.parent.gameObject.SetActive(false);
                }
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

        seciliSlotIndex = slotIndex; // Hangi slotun seçildiğini hafızaya aldık
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

        // ====================================================================
        // GÜNCELLENDİ: SADECE SADE BUTON KONTROLÜ (SAYAÇ YAZISI KALDIRILDI)
        // ====================================================================
        if (tamirEtButonu != null)
        {
            if (seciliEsya.esyaTipi == ItemData.EsyaTuru.DayanikliAlet)
            {
                // 1. Çantada "Tamir Kiti" var mı kontrol et
                bool tamirKitiVarMi = PlayerInventory.Instance.CantamdaBuEsyadanVarMi("Tamir Kiti");
                
                // 2. Bu aletin gizli tamir sayacını kontrol et
                List<PlayerInventory.EnvanterSlotu> guncelCanta = PlayerInventory.Instance.GetCantaListesi();
                int guncelTamirSayisi = guncelCanta[slotIndex].tamirEdilmeSayisi;

                // Kit varsa VE 3 hakkı dolmadıysa butonu göster, yoksa gizle!
                if (tamirKitiVarMi && guncelTamirSayisi < 3)
                {
                    tamirEtButonu.SetActive(true);
                }
                else
                {
                    tamirEtButonu.SetActive(false);
                }
            }
            else
            {
                tamirEtButonu.SetActive(false);
            }
        }
    }

    // ====================================================================
    // YENİ: KÜÇÜK TAMİR BUTONUNA BASILDIĞINDA ÇALIŞACAK MİSTİK TETİKLEYİCİ
    // ====================================================================
    public void SeciliAletiTamirEtButonFonksiyonu()
    {
        // SİHİRLİ SATIR: "Tıklama olayını burada tüket, arkadaki objelere geçirme!"
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        if (seciliEsya == null || PlayerInventory.Instance == null) return;

        ItemData kitData = PlayerInventory.Instance.EsyaDataGetirAdla("Tamir Kiti");
        
        // PlayerInventory içindeki o yazdığımız tamir motorunu çalıştırıyoruz
        bool tamirBasarili = PlayerInventory.Instance.AletiTamirEt(seciliSlotIndex, kitData);
        
        if (tamirBasarili)
        {
            // Başarılıysa çantadan 1 adet tamir kitini düşüyoruz
            PlayerInventory.Instance.EsyaAzaltYadaSil(kitData, 1);
            
            // Butonun durumunu (hakkı bitti mi diye) ve arayüzü anlık tazelemek için:
            SlotSecildi(seciliSlotIndex);
            EnvanterArayuzunuYenile();
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