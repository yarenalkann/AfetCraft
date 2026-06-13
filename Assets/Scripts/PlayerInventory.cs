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

    [Header("Bağımsız Test Ayarları")]
    [Tooltip("Test etmek istediğin ItemData kartlarını (Çimento, Balyoz vb.) sırayla buraya sürükle.")]
    public List<ItemData> testEsyaKartlari = new List<ItemData>();

    [Header("Arayüz Açma/Kapatma Ayarları")]
    [Tooltip("Sahnede tasarladığın o en dıştaki siyah çerçeveli ana Tablet UI objesini buraya sürükle.")]
    public GameObject anaTabletUIObjesi; 

    [Header("Özel İmleç (Cursor) Ayarları")]
    [Tooltip("Envanter açıldığında görünecek olan kendi tasarladığın pikselli imleç resmini (Sprite/Texture2D) buraya sürükle.")]
    public Texture2D seninOzelImlecin;
    public Vector2 imlecTiklamaNoktasi = Vector2.zero; // İmlecin tam ucuyla tıklaması için (Genelde 0,0)

    private void Update()
    {
        // ====================================================================
        // I TUŞUNA BASINCA TABLETİ AÇMA / KAPATMA, OYUNU DURDURMA VE ÖZEL FARE
        // ====================================================================
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

        // ====================================================================
        // [BAĞIMSIZ TEST MODU]
        // ====================================================================
        if (Input.GetKeyDown(KeyCode.Alpha1)) { TestEsyaEkleByIndex(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { TestEsyaEkleByIndex(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { TestEsyaEkleByIndex(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { TestEsyaEkleByIndex(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { TestEsyaEkleByIndex(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { TestEsyaEkleByIndex(5); }
    }

    private void TestEsyaEkleByIndex(int index)
    {
        if (testEsyaKartlari != null && testEsyaKartlari.Count > index && testEsyaKartlari[index] != null)
        {
            EsyaEkle(testEsyaKartlari[index], 1);
            Debug.Log($"<color=cyan>[Bağımsız Test] {index + 1}'e basıldı: {testEsyaKartlari[index].esyaAdi} eklendi!</color>");
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

        ArayuzuTazele();
    }

    // ====================================================================
    // EŞYA KULLANMA VE HASAR VERME FONKSİYONU (Sözlük taraması güncellendi)
    // ====================================================================
    public bool EsyaKullan(ItemData esya, float alinacakHasar = 20f)
    {
        if (esya == null) return false;

        // Aletler benzersiz ID ile tutulduğu için sözlükte esyaID yerine eşleşen ilk slotu bulmalıyız
        int bulunanAnahtar = -1;
        EnvanterSlotu slot = null;

        foreach (var kp in cantaIcerigi)
        {
            if (kp.Value.esya != null && kp.Value.esya.esyaID == esya.esyaID && kp.Value.adet > 0)
            {
                bulunanAnahtar = kp.Key;
                slot = kp.Value;
                break; // İlk bulduğumuzu kullanalım
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
                Debug.Log($"[Envanter] {esya.esyaAdi} pürüzsüzce kullanıldı. Kalıcı cihaz olduğu için hasar almadı.");
                return true;

            case ItemData.EsyaTuru.SarfMalzemesi:
                slot.adet--;
                if (slot.adet <= 0) cantaIcerigi.Remove(bulunanAnahtar);
                Debug.Log($"[Envanter] {esya.esyaAdi} tüketildi. Kalan: {(cantaIcerigi.ContainsKey(bulunanAnahtar) ? slot.adet : 0)}");
                ArayuzuTazele();
                return true;

            case ItemData.EsyaTuru.DayanikliAlet:
                slot.guncelDayaniklilik -= alinacakHasar;
                Debug.Log($"[Envanter] {esya.esyaAdi} hasar yedi! Kalan Can: %{slot.guncelDayaniklilik}");

                if (slot.guncelDayaniklilik <= 0)
                {
                    slot.adet--;
                    Debug.Log($"[Envanter] 1 adet {esya.esyaAdi} tamamen kırıldı ve yok oldu!");

                    if (slot.adet <= 0)
                    {
                        cantaIcerigi.Remove(bulunanAnahtar);
                    }
                    else
                    {
                        slot.guncelDayaniklilik = esya.maksimumDayaniklilik;
                    }
                }
                ArayuzuTazele();
                return true;
        }
        return false;
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
}