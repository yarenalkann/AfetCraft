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
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            HotbarEsyasiniEleAl(0); // 1. Hızlı Kullanım Slotu (İndex 0)
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            HotbarEsyasiniEleAl(1); // 2. Hızlı Kullanım Slotu (İndex 1)
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            HotbarEsyasiniEleAl(2); // 3. Hızlı Kullanım Slotu (İndex 2)
        }
    }


    public void HotbarEsyasiniEleAl(int slotIndex)
    {
        if (InventoryUIManager.Instance == null || CharacterHandManager.Instance == null) return;

        var envanterdekiHizliListeler = InventoryUIManager.Instance.hizliKullanımKutulari;

        // Geçerli bir slot aralığı mı kontrolü
        if (envanterdekiHizliListeler == null || slotIndex >= envanterdekiHizliListeler.Count) return;

        ItemData basilanSlotunEsyasi = envanterdekiHizliListeler[slotIndex].icindekiEsya;

        // ====================================================================
        // 🎯 1. DURUM: BOŞ BARA BASILDIYSA -> Eldeki eşyayı temizle ve boşalt
        // ====================================================================
        if (basilanSlotunEsyasi == null)
        {
            suAnSeciliHotbarIndeksi = -1; // Seçimi sıfırla
            CharacterHandManager.Instance.EldekiEsyayiTemizle();
            Debug.Log("<color=orange>[Hotbar]</color> Boş slot tetiklendi, karakterin eli temizlendi.");
            return;
        }

        // ====================================================================
        // 🎯 2. DURUM: ZATEN ELİNDE OLAN SLOTUN TUŞUNA TEKRAR BASILDIYSA -> Geri kaldır
        // ====================================================================
        if (suAnSeciliHotbarIndeksi == slotIndex)
        {
            suAnSeciliHotbarIndeksi = -1; // Seçimi kaldır
            CharacterHandManager.Instance.EldekiEsyayiTemizle();
            Debug.Log($"<color=orange>[Hotbar]</color> {basilanSlotunEsyasi.esyaAdi} kınına geri sokuldu, el boşaltıldı.");
            return;
        }

        // ====================================================================
        // 🎯 3. DURUM: İÇİ DOLU YENİ BİR SLOT TETİKLENDİYSE -> Eşyayı kuşan!
        // ====================================================================
        suAnSeciliHotbarIndeksi = slotIndex;
        CharacterHandManager.Instance.EsyaEleAl(basilanSlotunEsyasi);
        Debug.Log($"<color=cyan>[Hotbar]</color> {basilanSlotunEsyasi.esyaAdi} başarıyla ele alındı.");
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
    // 🎨 EKRANDAKİ İKON VE ADETLERİ CANLI TAZELEME (KESİN ÇÖZÜM)
    // ====================================================================
    public void HotbarArayuzunuGuncelle()
    {
        // Güvenlik Kontrolü: Envanter sistemi sahnede var mı?
        if (PlayerInventory.Instance == null || InventoryUIManager.Instance == null) return;

        var envanterdekiHizliListeler = InventoryUIManager.Instance.hizliKullanımKutulari;

        for (int i = 0; i < hotbarSlotlari.Count; i++)
        {
            if (hotbarSlotlari[i].esyaIkonu == null) continue;

            // Eğer o slot indeksi envanterdeki listede varsa ve içi boş değilse
            if (envanterdekiHizliListeler != null && i < envanterdekiHizliListeler.Count && envanterdekiHizliListeler[i].icindekiEsya != null)
            {
                ItemData esya = envanterdekiHizliListeler[i].icindekiEsya;
                
                // 1. İkon Resmini Aç ve Görseli Bas
                hotbarSlotlari[i].esyaIkonu.gameObject.SetActive(true);
                hotbarSlotlari[i].esyaIkonu.sprite = esya.esyaIkonu;

                // 2. Adet Kontrolü
                int adet = PlayerInventory.Instance.EsyaAdetiniGetir(esya);
                if (hotbarSlotlari[i].adetYazisi != null)
                {
                    if (esya.ustUsteBiniyorMu && adet > 1)
                    {
                        hotbarSlotlari[i].adetYazisi.gameObject.SetActive(true);
                        hotbarSlotlari[i].adetYazisi.text = "x" + adet;
                    }
                    else
                    {
                        hotbarSlotlari[i].adetYazisi.gameObject.SetActive(false);
                    }
                }

                // 3. 🎯 EN KRİTİK DÜZELTME: 
                // Tablet slotunda can barı olmasa bile, oyun ekranındaki hotbar slotunun 
                // kendi can barına doğrudan PlayerInventory sözlüğünden canı basıyoruz!
                if (hotbarSlotlari[i].canBariDolulukGörseli != null)
                {
                    if (esya.esyaTipi == ItemData.EsyaTuru.DayanikliAlet)
                    {
                        // Tablete hiç sormuyoruz! Doğrudan hafızadaki ana envanter sözlüğünden canı çek!
                        float guncelCan = PlayerInventory.Instance.EsyaDayaniklilikGetir(esya);
                        float dolulukOrani = guncelCan / esya.maksimumDayaniklilik;

                        hotbarSlotlari[i].canBariDolulukGörseli.gameObject.SetActive(true);
                        hotbarSlotlari[i].canBariDolulukGörseli.fillAmount = dolulukOrani;

                        // Can durumuna göre renk ayarı
                        if (dolulukOrani <= 0.3f)
                            hotbarSlotlari[i].canBariDolulukGörseli.color = Color.red;
                        else
                            hotbarSlotlari[i].canBariDolulukGörseli.color = Color.green;
                    }
                    else
                    {
                        // Alet değilse (Beton Blok gibi sarf malzemesiyse) can barını gizle
                        hotbarSlotlari[i].canBariDolulukGörseli.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // Eğer slot boşsa her şeyi gizle
                hotbarSlotlari[i].esyaIkonu.gameObject.SetActive(false);
                if (hotbarSlotlari[i].adetYazisi != null) hotbarSlotlari[i].adetYazisi.gameObject.SetActive(false);
                if (hotbarSlotlari[i].canBariDolulukGörseli != null) hotbarSlotlari[i].canBariDolulukGörseli.gameObject.SetActive(false);
            }
        }
    }
}