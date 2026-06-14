using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance;

    [System.Serializable]
    public struct HotbarSlotUI
    {
        public Image slotArkaplan;
        public Image esyaIkonu;
        public TextMeshProUGUI adetYazisi;
        public Image canBariDolulukGörseli;
    }

    [Header("Arayüz Elemanları (3 Adet Slot Ekle)")]
    public List<HotbarSlotUI> hotbarSlotlari = new List<HotbarSlotUI>();

    [Header("Görsel Ayarlar")]
    public Color seciliSlotRengi = Color.yellow;   // Aktif olan slotun etrafı yansın
    public Color normalSlotRengi = Color.white;

    // Arka planda o an hotbar'da duran eşyaların verisi (Maksimum 3 tane)
    private List<ItemData> hotbardakiEsyalar = new List<ItemData>();
    private int suAnSeciliHotbarIndeksi = -1; // -1 demek el boş demek

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        HotbarArayuzunuGuncelle();
    }

    private void Update()
    {
        // 🎯 TAM SENİN KODUNA UYARLANDI: 
        // PlayerInventory içindeki anaTabletUIObjesi sahnede aktif mi (açık mı) diye bakıyoruz.
        if (PlayerInventory.Instance != null && 
            PlayerInventory.Instance.anaTabletUIObjesi != null && 
            PlayerInventory.Instance.anaTabletUIObjesi.activeSelf) 
        {
            return; // Tablet ekranda açıkken hotbar kısayol tuşlarını kilitle, aşağıya geçme!
        }

        // ⌨️ Klavye Tuş Kontrolleri (Tablet kapalıysa canavar gibi çalışırlar)
        if (Input.GetKeyDown(KeyCode.Alpha7)) HotbarSlotunaBasildi(0);
        if (Input.GetKeyDown(KeyCode.Alpha8)) HotbarSlotunaBasildi(1);
        if (Input.GetKeyDown(KeyCode.Alpha9)) HotbarSlotunaBasildi(2);
    }

    // ====================================================================
    // 🔄 ENVANTERDEN TETİKLENECEK SENKRONİZASYON MOTORU
    // ====================================================================
    public void HotbariEnvanterleSenkronizeEt(List<PlayerInventory.EnvanterSlotu> aracGereclerListesi)
    {
        hotbardakiEsyalar.Clear();

        // Araç-gereçler listesindeki ilk 3 dolu eşyayı hotbar'a kopyalıyoruz
        for (int i = 0; i < 3; i++)
        {
            if (i < aracGereclerListesi.Count && aracGereclerListesi[i] != null && aracGereclerListesi[i].esya != null)
            {
                hotbardakiEsyalar.Add(aracGereclerListesi[i].esya);
            }
            else
            {
                hotbardakiEsyalar.Add(null); // Boş slot
            }
        }

        HotbarArayuzunuGuncelle();
    }

    // ====================================================================
    // 🖱️ SLOTLARA TIKLANDIĞINDA VEYA TUŞA BASILDIĞINDA ÇALIŞACAK MANTIK
    // ====================================================================
    public void HotbarSlotunaBasildi(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbardakiEsyalar.Count) return;

        ItemData basilEsya = hotbardakiEsyalar[slotIndex];

        // 1. DURUM: Boş bir slota basıldıysa -> Eli temizle, eşyalar arkada kalsın
        if (basilEsya == null)
        {
            SlotSeciminiKapat();
            if (CharacterHandManager.Instance != null) CharacterHandManager.Instance.EldekiEsyayiTemizle();
            Debug.Log("[Hotbar] Boş slota basıldı, el temizlendi.");
            return;
        }

        // 2. DURUM: Zaten elinde olan eşyanın slotuna tekrar bastıysa -> Eşyayı geri kaldır (Kılıcı kınına sokmak gibi)
        if (suAnSeciliHotbarIndeksi == slotIndex)
        {
            SlotSeciminiKapat();
            if (CharacterHandManager.Instance != null) CharacterHandManager.Instance.EldekiEsyayiTemizle();
            Debug.Log($"[Hotbar] {basilEsya.esyaAdi} geri envantere çekildi, el boşaltıldı.");
            return;
        }

        // 3. DURUM: Yeni bir eşyaya bastıysa -> Onu eline ver!
        suAnSeciliHotbarIndeksi = slotIndex;
        SlotGorselleriniYaz(slotIndex);

        if (CharacterHandManager.Instance != null)
        {
            CharacterHandManager.Instance.EsyaEleAl(basilEsya);
        }
    }

    private void SlotSeciminiKapat()
    {
        suAnSeciliHotbarIndeksi = -1;
        for (int i = 0; i < hotbarSlotlari.Count; i++)
        {
            hotbarSlotlari[i].slotArkaplan.color = normalSlotRengi;
        }
    }

    private void SlotGorselleriniYaz(int seciliIndex)
    {
        for (int i = 0; i < hotbarSlotlari.Count; i++)
        {
            hotbarSlotlari[i].slotArkaplan.color = (i == seciliIndex) ? seciliSlotRengi : normalSlotRengi;
        }
    }

    // ====================================================================
    // 🎨 EKRANDAKİ İKON VE ADETLERİ CANLI TAZELEME
    // ====================================================================
    public void HotbarArayuzunuGuncelle()
    {
        for (int i = 0; i < hotbarSlotlari.Count; i++)
        {
            // 🚨 DÜZELTİLDİ: Slotun ana çerçevesi ekranda HEP AKTİF KALSIN, asla kapanmasın!
            if (hotbarSlotlari[i].slotArkaplan != null)
            {
                hotbarSlotlari[i].slotArkaplan.gameObject.SetActive(true);
            }

            if (i < hotbardakiEsyalar.Count && hotbardakiEsyalar[i] != null)
            {
                ItemData esya = hotbardakiEsyalar[i];
                
                // Slot doluysa ikonu göster ve sprite'ı bas
                if (hotbarSlotlari[i].esyaIkonu != null)
                {
                    hotbarSlotlari[i].esyaIkonu.gameObject.SetActive(true);
                    hotbarSlotlari[i].esyaIkonu.sprite = esya.esyaIkonu;
                }

                // 📦 1. Adet Yazısı Kontrolü
                int adet = PlayerInventory.Instance.EsyaAdetiniGetir(esya);
                if (hotbarSlotlari[i].adetYazisi != null)
                {
                    if (esya.ustUsteBiniyorMu && adet > 1)
                    {
                        hotbarSlotlari[i].adetYazisi.gameObject.SetActive(true);
                        hotbarSlotlari[i].adetYazisi.text = adet.ToString();
                    }
                    else
                    {
                        hotbarSlotlari[i].adetYazisi.gameObject.SetActive(false);
                    }
                }

                // 🔋 2. Dayanıklılık (Can Bar) Kontrolü
                if (hotbarSlotlari[i].canBariDolulukGörseli != null)
                {
                    if (esya.esyaTipi == ItemData.EsyaTuru.DayanikliAlet)
                    {
                        float guncelCan = PlayerInventory.Instance.EsyaDayaniklilikGetir(esya);
                        float maksimumCan = esya.maksimumDayaniklilik;
                        float dolulukOrani = guncelCan / maksimumCan;

                        // Sadece can barının kendisini ve üst kapsayıcısını açıyoruz
                        hotbarSlotlari[i].canBariDolulukGörseli.transform.parent.gameObject.SetActive(true);
                        hotbarSlotlari[i].canBariDolulukGörseli.fillAmount = dolulukOrani;

                        if (dolulukOrani <= 0.3f)
                            hotbarSlotlari[i].canBariDolulukGörseli.color = Color.red;
                        else
                            hotbarSlotlari[i].canBariDolulukGörseli.color = Color.green;
                    }
                    else
                    {
                        hotbarSlotlari[i].canBariDolulukGörseli.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // ====================================================================
                // 🎯 SLOT BOŞSA: Çerçeve kalır, sadece İÇİNDEKİLER gizlenir!
                // ====================================================================
                if (hotbarSlotlari[i].esyaIkonu != null) 
                    hotbarSlotlari[i].esyaIkonu.gameObject.SetActive(false);
                    
                if (hotbarSlotlari[i].adetYazisi != null) 
                    hotbarSlotlari[i].adetYazisi.gameObject.SetActive(false);
                    
                if (hotbarSlotlari[i].canBariDolulukGörseli != null)
                    hotbarSlotlari[i].canBariDolulukGörseli.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}