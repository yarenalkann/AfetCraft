using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    // Hafızada eşyaların can durumunu ve adetlerini tutan gelişmiş yapı
    [System.Serializable]
    public class EnvanterSlotu
    {
        public ItemData esya;
        public int adet;
        public float guncelDayaniklilik; 
        
        // ====================================================================
        // YENİ: BU SPESİFİK ALETİN KAÇ KEZ TAMIR EDİLDİĞİNİ TUTAN SAYAÇ
        // ====================================================================
        public int tamirEdilmeSayisi = 0; 

        public EnvanterSlotu(ItemData esya, int adet)
        {
            this.esya = esya;
            this.adet = adet;
            this.guncelDayaniklilik = esya.maksimumDayaniklilik;
            this.tamirEdilmeSayisi = 0; // İlk alındığında sıfır
        }
    }
    
    // Eşya ID'sine göre envanterdeki slotları tutan listemiz
    // (Aletler için benzersiz negatif ID'ler, sarf malzemeleri için orijinal ID'ler tutulur)
    private Dictionary<int, EnvanterSlotu> cantaIcerigi = new Dictionary<int, EnvanterSlotu>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Oyun açıldığında arayüzü sıfırlasın diye tetikliyoruz
        ArayuzuTazele();
    }

    [Header("Arayüz Açma/Kapatma Ayarları")]
    [Tooltip("Sahnede tasarladığın o en dıştaki siyah çerçeveli ana Tablet UI objesini buraya sürükle.")]
    public GameObject anaTabletUIObjesi; 
    public GameObject tabletPanel;

    [Header("Özel İmleç (Cursor) Ayarları")]
    [Tooltip("Envanter açıldığında görünecek olan kendi tasarladığın pikselli imleç resmini (Sprite/Texture2D) buraya sürükle.")]
    public Texture2D seninOzelImlecin;
    public Vector2 imlecTiklamaNoktasi = Vector2.zero; // İmlecin tam ucuyla tıklaması için (Genelde 0,0)

    private void Update()
    {
        // ====================================================================
        // I TUŞUNA BASINCA TABLETİ AÇMA / KAPATMA, OYUNU DURDURMA VE ÖZEL FARE
        // ====================================================================
        if (tabletPanel.activeSelf) return; // Tablet açıksa envanteri açma!

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (anaTabletUIObjesi != null)
            {
                bool suAnkiDurum = anaTabletUIObjesi.activeSelf;
                bool yeniDurum = !suAnkiDurum;
                
                anaTabletUIObjesi.SetActive(yeniDurum);

                if (yeniDurum == true) // Envanter açıldıysa
                {
                    Time.timeScale = 0f; // SİHİRLİ DOKUNUŞ: Oyunu ve dünyayı arkada tamamen dondur!
                    
                    Cursor.lockState = CursorLockMode.None; // Fareyi kilitten kurtar
                    Cursor.visible = true;                  // İmleci görünür yap
                    
                    // Kendi tasarladığın imleci ekrana basıyoruz!
                    if (seninOzelImlecin != null)
                    {
                        Cursor.SetCursor(seninOzelImlecin, imlecTiklamaNoktasi, CursorMode.Auto);
                    }
                }
                else // Envanter kapatıldıysa
                {
                    Time.timeScale = 1f; // Oyunu normal hızına geri döndür, dünya aksın!
                    
                    Cursor.lockState = CursorLockMode.Locked; // Fareyi merkeze kilitle
                    Cursor.visible = false;                   // İmleci gizle
                    
                    // İmleci tekrar varsayılana çekiyoruz ki oyun içinde ekranda kalmasın
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }

                if (yeniDurum && InventoryUIManager.Instance != null)
                {
                    InventoryUIManager.Instance.EnvanterArayuzunuYenile();
                }
                
                Debug.Log($"[Tablet] Dünya Durdu mu: {yeniDurum} | Özel İmleç Aktif mi: {yeniDurum}");
            }
            else
            {
                Debug.LogWarning("[Tablet] Ana Tablet UI Objesi PlayerInventory scriptine atanmamış!");
            }
        }
    }


    // ====================================================================
    // EŞYA EKLEME (Dükkandan satın alınınca veya dünyadan toplanınca)
    // ====================================================================
    public void EsyaEkle(ItemData yeniEsya, int miktar)
    {
        if (yeniEsya == null) return;

        // 1. DURUM: Eğer üst üste binebilen bir sarf malzemesiyse (Çimento, tuğla vb.)
        if (yeniEsya.ustUsteBiniyorMu)
        {
            if (cantaIcerigi.ContainsKey(yeniEsya.esyaID))
            {
                cantaIcerigi[yeniEsya.esyaID].adet += miktar;
            }
            else
            {
                cantaIcerigi.Add(yeniEsya.esyaID, new EnvanterSlotu(yeniEsya, miktar));
            }
        }
        // 2. DURUM: Eğer üst üste BİNMEYEN bir aletse (Balyoz, Eğim ölçer vb.)
        else
        {
            for (int i = 0; i < miktar; i++)
            {
                int benzersizUydurmaID = Random.Range(-9999999, -1000);
                
                while (cantaIcerigi.ContainsKey(benzersizUydurmaID))
                {
                    benzersizUydurmaID = Random.Range(-9999999, -1000);
                }

                cantaIcerigi.Add(benzersizUydurmaID, new EnvanterSlotu(yeniEsya, 1));
            }
        }

         TümArayüzleriVeHotbariSenkronizeEt(); // Hem mor tableti hem hotbarı aynı anda yenilesin!
    }

    // ====================================================================
    // EŞYA KULLANMA VE HASAR VERME FONKSİYONU (Sözlük taraması güncellendi)
    // ====================================================================
    public bool EsyaKullan(ItemData esya, float alinacakHasar = 10f)
    {
        if (esya == null) return false;

        int bulunanAnahtar = -1;
        EnvanterSlotu slot = null;

        // Çantadaki eşyayı ara
        foreach (var kp in cantaIcerigi)
        {
            if (kp.Value.esya != null && kp.Value.esya.esyaID == esya.esyaID && kp.Value.adet > 0)
            {
                bulunanAnahtar = kp.Key;
                slot = kp.Value;
                break; 
            }
        }

        if (slot == null)
        {
            Debug.LogWarning($"[Envanter] {esya.esyaAdi} elinizde hiç yok!");
            return false;
        }

        switch (esya.esyaTipi)
        {
            case ItemData.EsyaTuru.KaliciCihaz:
                Debug.Log($"[Envanter] {esya.esyaAdi} kullanıldı.");
                TümArayüzleriVeHotbariSenkronizeEt();
                return true;

            case ItemData.EsyaTuru.SarfMalzemesi: // 🧱 Beton Blok yerleştirildiğinde burası çalışır
                slot.adet--;
                if (slot.adet <= 0) cantaIcerigi.Remove(bulunanAnahtar);
                Debug.Log($"[Envanter] {esya.esyaAdi} tüketildi. Kalan: {(cantaIcerigi.ContainsKey(bulunanAnahtar) ? slot.adet : 0)}");
                
                TümArayüzleriVeHotbariSenkronizeEt();
                return true;

            case ItemData.EsyaTuru.DayanikliAlet: // 🔨 Balyoz ve Çekiç buraya düşer
                slot.guncelDayaniklilik -= alinacakHasar;
                Debug.Log($"[Envanter] {esya.esyaAdi} hasar yedi! Kalan Can: %{slot.guncelDayaniklilik}");

                if (slot.guncelDayaniklilik <= 0)
                {
                    slot.adet--;
                    Debug.Log($"[Envanter] 1 adet {esya.esyaAdi} tamamen kırıldı!");

                    if (slot.adet <= 0)
                    {
                        cantaIcerigi.Remove(bulunanAnahtar);
                        
                        // Eğer elindeki balyoz tamamen kırıldıysa el modelini yok et
                        if (CharacterHandManager.Instance != null && CharacterHandManager.Instance.suAnElindekiEsyaData == esya)
                        {
                            CharacterHandManager.Instance.EldekiEsyayiTemizle();
                        }
                    }
                    else
                    {
                        slot.guncelDayaniklilik = esya.maksimumDayaniklilik;
                    }
                }
                
                // 🎯 VERİ DEĞİŞTİĞİ AN HEMEN ARAYÜZLERİ TETİKLE!
                TümArayüzleriVeHotbariSenkronizeEt();
                return true;
        }
        return false;
    }

    // ====================================================================
    // ⚡ İKİZ ARAYÜZ SENKRONİZASYON MOTORU
    // Hem mor tableti hem de alt taraftaki hotbarı aynı salisede günceller!
    // ====================================================================
    private void TümArayüzleriVeHotbariSenkronizeEt()
    {
        // 1. Mor Tablet Ekranını Güncelle
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.EnvanterArayuzunuYenile();
        }

        // 2. 🎯 SİHİRLİ SATIR: Ekranın altındaki can barlarını ve adetleri anlık eşitle!
        if (HotbarManager.Instance != null)
        {
            HotbarManager.Instance.HotbarArayuzunuGuncelle();
        }
    }
    // Arayüz sorguları için toplam adet getiren yardımcı fonksiyon
    public int EsyaAdetiniGetir(ItemData esya)
    {
        if (esya == null) return 0;
        
        int toplam = 0;
        foreach (var slot in cantaIcerigi.Values)
        {
            if (slot.esya != null && slot.esya.esyaID == esya.esyaID)
            {
                toplam += slot.adet;
            }
        }
        return toplam;
    }

    // Arayüz sorguları için can barı yüzdesini getiren yardımcı fonksiyon (İlk bulduğunu döner)
    public float EsyaDayaniklilikGetir(ItemData esya)
    {
        if (esya == null) return 0;

        foreach (var slot in cantaIcerigi.Values)
        {
            if (slot.esya != null && slot.esya.esyaID == esya.esyaID)
            {
                return slot.guncelDayaniklilik;
            }
        }
        return 0;
    }

    private void ArayuzuTazele()
    {
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.EnvanterArayuzunuYenile();
        }
    }

    public List<EnvanterSlotu> GetCantaListesi()
    {
        List<EnvanterSlotu> liste = new List<EnvanterSlotu>();
        foreach (var anahtarDegerCifti in cantaIcerigi)
        {
            if (anahtarDegerCifti.Value.adet > 0)
            {
                liste.Add(anahtarDegerCifti.Value);
            }
        }
        return liste;
    }

    public void TabletiKapat()
    {
        if (anaTabletUIObjesi != null)
        {
            anaTabletUIObjesi.SetActive(false);

            Time.timeScale = 1f;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            
            Debug.Log("[Tablet] X butonuna basıldı, tablet kapatıldı ve dünya geri aktı.");
        }
    }

    // ====================================================================
    // YENİ SİHİRLİ FONKSİYONLAR (NEREDEN ÇIKTI DENİLEN EKSİKLER BURADA)
    // ====================================================================

    // 1. Çantada isme göre eşya var mı kontrolü
    public bool CantamdaBuEsyadanVarMi(string esyaAdi)
    {
        foreach (var slot in cantaIcerigi.Values)
        {
            if (slot.esya != null && slot.esya.esyaAdi == esyaAdi && slot.adet > 0)
            {
                return true; 
            }
        }
        return false; 
    }

    // 2. Adı verilen eşyanın Scriptable Object verisini (Data) getirmek
    public ItemData EsyaDataGetirAdla(string esyaAdi)
    {
        foreach (var slot in cantaIcerigi.Values)
        {
            if (slot.esya != null && slot.esya.esyaAdi == esyaAdi)
            {
                return slot.esya; 
            }
        }
        Debug.LogWarning($"[Envanter] {esyaAdi} isimli eşyanın datasını çantada bulamadım!");
        return null;
    }

    // 3. Tamir kiti veya malzeme harcandığında adedini azaltan motor
    public void EsyaAzaltYadaSil(ItemData esya, int miktar)
    {
        if (esya == null) return;

        int silinecekAnahtar = -1;
        EnvanterSlotu slot = null;

        // Sözlükte bu ItemData'ya ait olan slotu arıyoruz
        foreach (var kp in cantaIcerigi)
        {
            if (kp.Value.esya != null && kp.Value.esya.esyaID == esya.esyaID)
            {
                silinecekAnahtar = kp.Key;
                slot = kp.Value;
                break;
            }
        }

        if (slot != null)
        {
            slot.adet -= miktar;
            if (slot.adet <= 0)
            {
                cantaIcerigi.Remove(silinecekAnahtar);
            }
            ArayuzuTazele();
        }
    }

    // ====================================================================
    // ALETI TAMIR EDEN MÜHENDİSLİK FONKSİYONU
    // ====================================================================
    public bool AletiTamirEt(int slotIndex, ItemData tamirKitiEsyasi)
    {
        List<EnvanterSlotu> guncelListe = GetCantaListesi();
        
        if (slotIndex >= guncelListe.Count) return false;
        
        EnvanterSlotu tamirEdilecekSlot = guncelListe[slotIndex];

        if (tamirEdilecekSlot.esya.esyaTipi != ItemData.EsyaTuru.DayanikliAlet)
        {
            Debug.LogWarning("[Tamir] Sadece dayanıklı aletler tamir edilebilir!");
            return false;
        }

        if (tamirEdilecekSlot.tamirEdilmeSayisi >= 3)
        {
            Debug.LogWarning($"[Tamir] {tamirEdilecekSlot.esya.esyaAdi} artık hurdaya çıkmış! 3 kez tamir edildiği için daha fazla tamir edilemez.");
            return false;
        }

        if (tamirEdilecekSlot.guncelDayaniklilik >= tamirEdilecekSlot.esya.maksimumDayaniklilik)
        {
            Debug.Log("[Tamir] Aletin canı zaten tamamen dolu!");
            return false;
        }

        // --- OPERASYON ---
        tamirEdilecekSlot.guncelDayaniklilik = tamirEdilecekSlot.esya.maksimumDayaniklilik; // Canı fulle!
        tamirEdilecekSlot.tamirEdilmeSayisi++; // Sayaç tık attı

        Debug.Log($"[Tamir] {tamirEdilecekSlot.esya.esyaAdi} başarıyla tamir edildi! Güncel Tamir Durumu: {tamirEdilecekSlot.tamirEdilmeSayisi}/3");

        ArayuzuTazele();
        return true;
    }

    // ====================================================================
    // 🗑️ DÜZELTİLDİ: SEKME/KATEGORİ UYUMLU ESYA DÜŞÜRME MOTORU
    // ====================================================================
    public void UIKategoriIndeksineGoreEsyaSilYadaAzalt(int uiSlotIndex, int miktar, ItemData.EnvanterKategorisi aktifSekme)
    {
        // Sadece o an ekranda açık olan kategoriye ait sözlük anahtarlarını topluyoruz!
        List<int> filtrelenmisSozlukAnahtarlari = new List<int>();
        
        foreach (var anahtar in cantaIcerigi.Keys)
        {
            // Eşya boş değilse, adedi varsa VE o an açık olan sekmeyle kategorisi eşleşiyorsa listeye al!
            if (cantaIcerigi[anahtar].esya != null && 
                cantaIcerigi[anahtar].esya.esyaKategorisi == aktifSekme && 
                cantaIcerigi[anahtar].adet > 0)
            {
                filtrelenmisSozlukAnahtarlari.Add(anahtar);
            }
        }

        // Güvenlik Kontrolü: Seçilen filtrelenmiş sıra mevcut sınırları aşıyor mu?
        if (uiSlotIndex < 0 || uiSlotIndex >= filtrelenmisSozlukAnahtarlari.Count) return;

        int hedefSozlukAnahtari = filtrelenmisSozlukAnahtarlari[uiSlotIndex];
        EnvanterSlotu slot = cantaIcerigi[hedefSozlukAnahtari];

        if (slot != null && slot.esya != null)
        {
            slot.adet -= miktar;

            if (slot.adet <= 0)
            {
                cantaIcerigi.Remove(hedefSozlukAnahtari);
                Debug.Log($"[Envanter] {slot.esya.esyaAdi} kategorisinden tamamen silindi.");
            }
            else
            {
                cantaIcerigi[hedefSozlukAnahtari] = slot;
                Debug.Log($"[Envanter] {slot.esya.esyaAdi} adedi azaltıldı. Kalan: {slot.adet}");
            }

            ArayuzuTazele(); 
        }
    }
}