using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Mathematics;

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

    // ====================================================================
    // 🛠️ SENİN YENİ HİYERARŞİNE ÖZEL MALZEME YAPISI (GÜNCELLENDİ)
    // ====================================================================
    [System.Serializable]
    public struct CraftGereksinimUI
    {
        public GameObject objeGrup;           // Sahnendeki "slot0" veya "slot1" objesi
        public Image malzemeIkonu;            // slot0/esyalkonu
        public TextMeshProUGUI miktarYazisi;   // slot0/esyaAdeti (Gereken miktar, örn: x2)
        public TextMeshProUGUI sahipOlunanYazisi; // slot0/sahipOlunanSayi (Oyuncudaki miktar, örn: x4)
    }

    [Header("Sayfa Referansları")]
    public GameObject inventoryPage; 
    public GameObject craftPage;     
    public GameObject sagDetayPaneli2; // Sahnendeki sağ craft detay paneli

    [Header("Envanter Slotları (Orta Alan)")]
    public List<DinamikUIKutusu> uiKutulari = new List<DinamikUIKutusu>();

    [Header("Hızlı Kullanım Slotları")]
    public List<DinamikUIKutusu> hizliKullanımKutulari = new List<DinamikUIKutusu>();

    [Header("Sağ Detay Paneli Objeleri (Normal Envanter)")]
    public TextMeshProUGUI detayEsyaAdiYazisi;       
    public TextMeshProUGUI detayEsyaAciklamaYazisi;  
    public Image detayEsyaModelResmi;                
    public GameObject detayPaneliAnaObjesi;          

    [Header("Sağ Panel İşlem Butonları (Normal Envanter)")]
    public GameObject kullanButonu;       
    public GameObject birakButonu;         
    public GameObject hizliKullanimButonu; 
    public GameObject tamirEtButonu; 

    [Header("Kategori Buton Yazıları (TMP)")]
    public TextMeshProUGUI aracGereclerButonYazisi;
    public TextMeshProUGUI uretimMalzemeleriButonYazisi;

    [Header("Renk Paleti")]
    public Color aktifYaziRengi = new Color(1f, 0.6f, 0f);     
    public Color pasifYaziRengi = new Color(0.5f, 0.5f, 0.5f); 

    // ====================================================================
    // 🛠️ SENİN YENİ HİYERARŞİNE ÖZEL ELEMENTLER
    // ====================================================================
    [Header("Craft UI (uretimAlani / slot3)")]
    public Image uretimAlanSonucIkonu;          // uretimAlani/slot3/esyalkonu (Üretilecek ürün)
    public TextMeshProUGUI uretimAlanSonucAdet;  // uretimAlani/slot3/esyaAdeti (Üretilmek istenen adet)

    // === YENİ EKLEYECEĞİN REFERANSLAR (Okun solundaki iki küçük kutu) ===
    public Image uretimAlanGirdi1Ikon;          // Okun solundaki 1. küçük kutunun esyalkonu
    public TextMeshProUGUI uretimAlanGirdi1Adet; // Okun solundaki 1. küçük kutunun esyaAdeti (Oyuncudaki toplam stok)
    
    public Image uretimAlanGirdi2Ikon;          // Okun solundaki 2. küçük kutunun esyalkonu
    public TextMeshProUGUI uretimAlanGirdi2Adet; // Okun solundaki 2. küçük kutunun esyaAdeti (Oyuncudaki toplam stok)

    [Header("Craft UI (GerekliMalzemeler -> slot0 ve slot1)")]
    public List<CraftGereksinimUI> craftGereksinimSlotlari = new List<CraftGereksinimUI>(); 

    [Header("Craft UI (Butonlar ve Counter)")]
    public Button uretBüyükButonu;             // uretButonu objesindeki Button bileşeni
    public TextMeshProUGUI uretimAdetText;     // counterPanel/countText
    public Button adetArttirButonu;           // counterPanel/plusButton
    public Button adetAzaltButonu;            // counterPanel/minusButton

    private ItemData.EnvanterKategorisi mevcutKategori = ItemData.EnvanterKategorisi.AracGerecler;
    private ItemData seciliEsya;
    private int seciliSlotIndex; 
    
    // Craft takibi için değişkenler
    private CraftRecipe seciliTarif;
    private int uretilecekMiktar = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetleDetayPaneli();
        HizliKullanimSlotlariniIlkKezTemizle();
        EnvanterSekmesiniAc(); 
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

    // ====================================================================
    // 📑 SAYFA GEÇİŞLERİ
    // ====================================================================
    public void EnvanterSekmesiniAc()
    {
        if (inventoryPage != null) inventoryPage.SetActive(true);
        if (craftPage != null) craftPage.SetActive(false);
        if (sagDetayPaneli2 != null) sagDetayPaneli2.SetActive(false); 
        if (detayPaneliAnaObjesi != null) detayPaneliAnaObjesi.SetActive(false);
        
        // Sayfa değiştirirken gizlenen eski envanter butonlarını burada geri açıyoruz
        if (kullanButonu != null) kullanButonu.SetActive(true);
        if (birakButonu != null) birakButonu.SetActive(true);
        if (hizliKullanimButonu != null) hizliKullanimButonu.SetActive(true);

        AraçGereçSekmesiniSec();
    }

    public void UretimSekmesiniAc()
    {
        if (inventoryPage != null) inventoryPage.SetActive(false);
        if (craftPage != null) craftPage.SetActive(true);
        
        if (detayPaneliAnaObjesi != null) detayPaneliAnaObjesi.SetActive(false); 
        if (sagDetayPaneli2 != null) sagDetayPaneli2.SetActive(true); 

        // Eski envanter butonlarını craft panelinde kapatıyoruz ki üst üste binmesinler
        if (kullanButonu != null) kullanButonu.SetActive(false);
        if (birakButonu != null) birakButonu.SetActive(false);
        if (hizliKullanimButonu != null) hizliKullanimButonu.SetActive(false);
        if (tamirEtButonu != null) tamirEtButonu.SetActive(false);

        if (CraftManager.Instance != null && CraftManager.Instance.tumTarifler.Count > 0)
        {
            TarifSecildi(CraftManager.Instance.tumTarifler[0]);
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
        if (tamirEtButonu != null) tamirEtButonu.SetActive(false); 
        seciliEsya = null;
    }

    // ====================================================================
    // ⚙️ SENİN YENİ BÖLÜNMÜŞ YAPINA ÖZEL CRAFT MOTORU
    // ====================================================================
    // Sahnede elinle koyduğun o 3 butona tıklandığında çalışan fonksiyon
    public void SolListedenTarifSecildi(int tarifIndex)
    {
        if (CraftManager.Instance == null || tarifIndex >= CraftManager.Instance.tumTarifler.Count) return;
        
        // SİHİRLİ DOKUNUŞ: Butona tıklandığı an sağ panel kapalıysa zorla AÇ!
        if (sagDetayPaneli2 != null) 
        {
            sagDetayPaneli2.SetActive(true);
        }

        // Şimdi verileri güvenle içine basabiliriz
        TarifSecildi(CraftManager.Instance.tumTarifler[tarifIndex]);
    }

    private void TarifSecildi(CraftRecipe secilenTarif)
    {
        if (secilenTarif == null) return;

        seciliTarif = secilenTarif;
        uretilecekMiktar = 1; 
        if (uretimAdetText != null) uretimAdetText.text = uretilecekMiktar.ToString();

        CraftArayuzunuMiktaraGoreGuncelle();
    }

    private void CraftArayuzunuMiktaraGoreGuncelle()
    {
        if (seciliTarif == null || PlayerInventory.Instance == null) return;

        // Okun solundaki pikselli girdi kutularını temizle
        if (uretimAlanGirdi1Ikon != null) uretimAlanGirdi1Ikon.transform.parent.gameObject.SetActive(false);
        if (uretimAlanGirdi2Ikon != null) uretimAlanGirdi2Ikon.transform.parent.gameObject.SetActive(false);

        // ====================================================================
        // 🧮 1. ELDEKİ STOĞA GÖRE MAKSİMUM ÜRETİLEBİLİR KAPASİTEYİ HESAPLA
        // ====================================================================
        int maksimumUretilebilirKapasite = int.MaxValue; 
        for (int i = 0; i < seciliTarif.gerekliMalzemeler.Count; i++)
        {
            var malzemeIhtiyac = seciliTarif.gerekliMalzemeler[i];
            int oyuncununStoğu = PlayerInventory.Instance.EsyaAdetiniGetir(malzemeIhtiyac.malzemeData);
            int tekSeferlikIhtiyac = malzemeIhtiyac.adet;

            if (tekSeferlikIhtiyac > 0)
            {
                int buMalzemeyeGoreKapasite = oyuncununStoğu / tekSeferlikIhtiyac;
                if (buMalzemeyeGoreKapasite < maksimumUretilebilirKapasite)
                    maksimumUretilebilirKapasite = buMalzemeyeGoreKapasite;
            }
        }
        if (maksimumUretilebilirKapasite == int.MaxValue || maksimumUretilebilirKapasite < 0) 
            maksimumUretilebilirKapasite = 0;

        // ====================================================================
        // 🎯 SAYAÇ SINIRLAMALARI (Senin istediğin esnek yapı)
        // ====================================================================
        // Oyuncu eksilete eksilete 1'in altına düşemesin
        if (uretilecekMiktar < 1) uretilecekMiktar = 1;
        
        // Eğer elinde hiç malzeme yoksa sayaç mecburen 0 dursun
        if (maksimumUretilebilirKapasite == 0) uretilecekMiktar = 0;

        // Alttaki küçük (+ 1 -) sayacının metnine oyuncunun kendi seçtiği sayıyı yazıyoruz
        if (uretimAdetText != null) uretimAdetText.text = uretilecekMiktar.ToString();

        // 📦 2. GİRDİ ALANLARINI DOLDURMA (Okun solundaki kutular)
        for (int i = 0; i < seciliTarif.gerekliMalzemeler.Count; i++)
        {
            var malzemeIhtiyac = seciliTarif.gerekliMalzemeler[i];
            int elindekiStok = PlayerInventory.Instance.EsyaAdetiniGetir(malzemeIhtiyac.malzemeData);

            if (i == 0 && uretimAlanGirdi1Ikon != null && uretimAlanGirdi1Adet != null)
            {
                uretimAlanGirdi1Ikon.transform.parent.gameObject.SetActive(true);
                uretimAlanGirdi1Ikon.sprite = malzemeIhtiyac.malzemeData.esyaIkonu;
                uretimAlanGirdi1Adet.text = "x" + elindekiStok;
            }
            else if (i == 1 && uretimAlanGirdi2Ikon != null && uretimAlanGirdi2Adet != null)
            {
                uretimAlanGirdi2Ikon.transform.parent.gameObject.SetActive(true);
                uretimAlanGirdi2Ikon.sprite = malzemeIhtiyac.malzemeData.esyaIkonu;
                uretimAlanGirdi2Adet.text = "x" + elindekiStok;
            }
        }

        // ====================================================================
        // 🧱 TAM İSTEDİĞİN YER: SLOT3 SABİT MAKSİMUM VİTRİNİ OLUYOR
        // ====================================================================
        if (uretimAlanSonucIkonu != null) uretimAlanSonucIkonu.sprite = seciliTarif.uretilecekEsya.esyaIkonu;
        
        if (uretimAlanSonucAdet != null) 
        {
            // Sayaçta ne yazdığı önemli değil, slot3 ELDEKİ malzemeyle üretilebilecek MAKSİMUM sayıyı gösterecek!
            uretimAlanSonucAdet.text = "x" + (seciliTarif.uretilecekAdet * maksimumUretilebilirKapasite);
        }

        // 🧱 3. GEREKSİNİM SLOTLARI (Seçilen sayaç miktarına göre maliyet çarpanı)
        foreach (var slot in craftGereksinimSlotlari)
        {
            if (slot.objeGrup != null) slot.objeGrup.SetActive(false);
        }

        bool uretimIcinHerSeyYeterliMi = true;

        for (int i = 0; i < seciliTarif.gerekliMalzemeler.Count; i++)
        {
            if (i >= craftGereksinimSlotlari.Count) break;

            var ihtiyac = seciliTarif.gerekliMalzemeler[i];
            var uiSlot = craftGereksinimSlotlari[i];

            if (uiSlot.objeGrup != null) uiSlot.objeGrup.SetActive(true);
            if (uiSlot.malzemeIkonu != null) uiSlot.malzemeIkonu.sprite = ihtiyac.malzemeData.esyaIkonu;

            int elindekiAdet = PlayerInventory.Instance.EsyaAdetiniGetir(ihtiyac.malzemeData);
            
            // Maliyet penceresi oyuncunun sayaçta seçtiği miktara göre katlanacak
            int toplamIstenenAdet = ihtiyac.adet * uretilecekMiktar; 

            if (uiSlot.miktarYazisi != null) uiSlot.miktarYazisi.text = "x" + toplamIstenenAdet;
            if (uiSlot.sahipOlunanYazisi != null) uiSlot.sahipOlunanYazisi.text = "x" + elindekiAdet;

            if (elindekiAdet < toplamIstenenAdet)
            {
                if (uiSlot.miktarYazisi != null) uiSlot.miktarYazisi.color = Color.red;
                uretimIcinHerSeyYeterliMi = false;
            }
            else
            {
                if (uiSlot.miktarYazisi != null) uiSlot.miktarYazisi.color = Color.white; 
            }
        }

        // ====================================================================
        // 🔒 ÜRET BUTONU KİLİT MEKANİZMASI (Sayaç > Kapasite ise buton söner!)
        // ====================================================================
        if (uretBüyükButonu != null)
        {
            // Oyuncunun seçtiği adet elindeki maksimum kapasiteden küçük veya eşitse VE ham maddesi yetiyorsa aktif et!
            uretBüyükButonu.interactable = uretimIcinHerSeyYeterliMi && (uretilecekMiktar <= maksimumUretilebilirKapasite) && (uretilecekMiktar > 0);
        }
    }
    public void AdetArttirButonFonksiyonu()
    {
        if (seciliTarif == null) return;
        
        uretilecekMiktar++; // Sayacı özgürce arttır!
        CraftArayuzunuMiktaraGoreGuncelle();
    }

    public void AdetAzaltButonFonksiyonu()
    {
        if (seciliTarif == null || uretilecekMiktar <= 1) return;
        
        uretilecekMiktar--; // Sayacı özgürce azalt!
        CraftArayuzunuMiktaraGoreGuncelle();
    }
    public void EsyaUretBüyükButonFonksiyonu()
    {
        if (seciliTarif == null || CraftManager.Instance == null || PlayerInventory.Instance == null) return;

        // Üretilecek eşyanın kategorisini kesin olarak "Araç-Gereçler" sekmesine yönlendiriyoruz
        if (seciliTarif.uretilecekEsya != null)
        {
            seciliTarif.uretilecekEsya.esyaKategorisi = ItemData.EnvanterKategorisi.AracGerecler;
        }

        // Sayaçta kaç yazıyorsa tam o kadar kez döngüyü çalıştır
        int uretimDonguSayisi = uretilecekMiktar;

        if (uretimDonguSayisi <= 0) return;

        for (int i = 0; i < uretimDonguSayisi; i++)
        {
            CraftManager.Instance.EsyaUret(seciliTarif);
        }

        // 🔄 KRİTİK DOKUNUŞ: Üretim bitti, eşyalar arka planda listeye eklendi.
        // Şimdi oyuncunun normal envanter çantasını da anında kodla yeniliyoruz ki eşyalar ekranda gözüksün!
        EnvanterArayuzunuYenile();

        // Üretim bittiği için sayacı bir sonraki işlem için varsayılan 1'e çekiyoruz
        uretilecekMiktar = 1; 

        // Craft ekranındaki yazıları ve yeni malzeme stok durumlarını ekranda tazeleyelim
        CraftArayuzunuMiktaraGoreGuncelle();
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

                // ====================================================================
                // 🎯 DÜZELTİLDİ: ESKİ DEĞİŞKEN YERİNE "ustUsteBiniyorMu" KONTROLÜ
                // ====================================================================
                if (hedefUIKutusu.adetYazesi != null)
                {
                    // KURAL: Eğer eşya çimento/tuğla gibi üst üste biniyorsa VE adedi 1'den çoksa göster!
                    if (cantaSlotu.esya.ustUsteBiniyorMu && cantaSlotu.adet > 1)
                    {
                        hedefUIKutusu.adetYazesi.text = "x" + cantaSlotu.adet;
                        hedefUIKutusu.adetYazesi.gameObject.SetActive(true);
                    }
                    else // Üst üste binmeyen aletlerde (Balyoz, Çekiç vb.) adet yazısını KESİN OLARAK SÖNDÜR!
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
        // Güvenlik Kontrolü: Seçili bir eşya var mı?
        if (seciliEsya == null || CharacterHandManager.Instance == null) return;

        // Sadece Araç-Gereçler kategorisindeyse ele almayı tetikle!
        if (seciliEsya.esyaKategorisi == ItemData.EnvanterKategorisi.AracGerecler)
        {
            // Karakterin eline bu eşyayı veriyoruz!
            CharacterHandManager.Instance.EsyaEleAl(seciliEsya);

            // Eşyayı eline aldığı için envanter tableti otomatik kapansın (Dünyaya dönsün)
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.TabletiKapat();
            }
        }
    }

    public void SeciliEsyayiBirak()
    {
        if (seciliEsya == null || PlayerInventory.Instance == null) return;

        // Üst üste binen malzemeyse counter'da yazan adet kadar, aletse direkt 1 tane atar
        int atilacakMiktar = seciliEsya.ustUsteBiniyorMu ? uretilecekMiktar : 1;

        // ====================================================================
        // 🚨 YENİ: ELDEKİ EŞYA KONTROLÜ VE ANLIK YOK ETME MOTORU
        // ====================================================================
        // Eğer oyuncunun şu an elinde tuttuğu bir eşya varsa VE bu bıraktığımız eşya ile aynıysa...
        if (CharacterHandManager.Instance != null && CharacterHandManager.Instance.suAnElindekiEsyaData != null)
        {
            if (CharacterHandManager.Instance.suAnElindekiEsyaData.esyaID == seciliEsya.esyaID)
            {
                // Çantadaki toplam adedine bakıyoruz. Eğer oyuncu elindeki tüm adedi (veya tekli aletini) bırakıyorsa...
                int eldekiToplamAdet = PlayerInventory.Instance.EsyaAdetiniGetir(seciliEsya);
                
                if (atilacakMiktar >= eldekiToplamAdet)
                {
                    // ...elindeki o 3D mesh modelini sahnede şak diye yok et ve eli boşalt!
                    CharacterHandManager.Instance.EldekiEsyayiTemizle();
                    Debug.Log($"<color=orange>[El]</color> {seciliEsya.esyaAdi} envanterden atıldığı için elden de temizlendi.");
                }
            }
        }

        // 1. AŞAMA: Sözlük uyumlu fonksiyonumuzla çantadan düşüyoruz
        PlayerInventory.Instance.UIKategoriIndeksineGoreEsyaSilYadaAzalt(seciliSlotIndex, atilacakMiktar, mevcutKategori);

        // 2. AŞAMA: Sağ paneldeki detayları ve yazıları temizle
        ResetleDetayPaneli();

        // 3. AŞAMA: Sayacı bir sonraki işlem için tekrar 1'e çek
        uretilecekMiktar = 1;
        if (uretimAdetText != null) uretimAdetText.text = uretilecekMiktar.ToString();

        // 4. AŞAMA: Sol taraftaki mor slotları ve adetleri canlı canlı tazeleyelim
        EnvanterArayuzunuYenile();
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
                    if (seciliEsya.adetYazesiGosterilsinMi)
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