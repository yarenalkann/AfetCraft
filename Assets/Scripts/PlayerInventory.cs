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
        public float guncelDayaniklilik; // Aletlerin anlık canı (Örn: %80)

        public EnvanterSlotu(ItemData esya, int adet)
        {
            this.esya = esya;
            this.adet = adet;
            this.guncelDayaniklilik = esya.maksimumDayaniklilik;
        }
    }

    // Eşya ID'sine göre envanterdeki slotları tutan listemiz
    private Dictionary<int, EnvanterSlotu> cantaIcerigi = new Dictionary<int, EnvanterSlotu>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ====================================================================
    // YENİ: KODUN ÇALIŞMASI VE TEST İÇİN START VE UPDATE FONKSİYONLARI
    // ====================================================================
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
        // [BAĞIMSIZ TEST MODU] (Klavyeden 1 ve 2 tuşları aynen kalıyor)
        // ====================================================================
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (testEsyaKartlari != null && testEsyaKartlari.Count > 0 && testEsyaKartlari[0] != null)
            {
                EsyaEkle(testEsyaKartlari[0], 1);
                Debug.Log($"<color=cyan>[Bağımsız Test] 1'e basıldı: {testEsyaKartlari[0].esyaAdi} eklendi!</color>");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (testEsyaKartlari != null && testEsyaKartlari.Count > 1 && testEsyaKartlari[1] != null)
            {
                EsyaEkle(testEsyaKartlari[1], 1);
                Debug.Log($"<color=cyan>[Bağımsız Test] 2'ye basıldı: {testEsyaKartlari[1].esyaAdi} eklendi!</color>");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (testEsyaKartlari != null && testEsyaKartlari.Count > 2 && testEsyaKartlari[2] != null)
            {
                EsyaEkle(testEsyaKartlari[2], 1);
                Debug.Log($"<color=cyan>[Bağımsız Test] 3'e basıldı: {testEsyaKartlari[2].esyaAdi} eklendi!</color>");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (testEsyaKartlari != null && testEsyaKartlari.Count > 3 && testEsyaKartlari[3] != null)
            {
                EsyaEkle(testEsyaKartlari[3], 1);
                Debug.Log($"<color=cyan>[Bağımsız Test] 4'e basıldı: {testEsyaKartlari[3].esyaAdi} eklendi!</color>");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (testEsyaKartlari != null && testEsyaKartlari.Count > 4 && testEsyaKartlari[4] != null)
            {
                EsyaEkle(testEsyaKartlari[4], 1);
                Debug.Log($"<color=cyan>[Bağımsız Test] 5'e basıldı: {testEsyaKartlari[4].esyaAdi} eklendi!</color>");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (testEsyaKartlari != null && testEsyaKartlari.Count > 5 && testEsyaKartlari[5] != null)
            {
                EsyaEkle(testEsyaKartlari[5], 1);
                Debug.Log($"<color=cyan>[Bağımsız Test] 5'e basıldı: {testEsyaKartlari[5].esyaAdi} eklendi!</color>");
            }
        }
    }
    // ====================================================================

    // EŞYA EKLEME (Dükkandan satın alınınca veya dünyadan toplanınca)
    // ====================================================================
    // GÜNCELLENEN AKILLI EŞYA EKLEME FONKSİYONU (DICTIONARY UYUMLU)
    // ====================================================================
    public void EsyaEkle(ItemData yeniEsya, int miktar)
    {
        if (yeniEsya == null) return;

        // 1. DURUM: Eğer üst üste binebilen bir sarf malzemesiyse (Çimento, tuğla vb.)
        if (yeniEsya.ustUsteBiniyorMu)
        {
            // Sözlükte bu orijinal ID zaten varsa adedini artır
            if (cantaIcerigi.ContainsKey(yeniEsya.esyaID))
            {
                cantaIcerigi[yeniEsya.esyaID].adet += miktar;
            }
            else // İlk defa ekleniyorsa orijinal ID'si ile sözlüğe kaydet
            {
                cantaIcerigi.Add(yeniEsya.esyaID, new EnvanterSlotu(yeniEsya, miktar));
            }
        }
        // 2. DURUM: Eğer üst üste BİNMEYEN bir aletse (Balyoz, Eğim ölçer vb.)
        else
        {
            // Kaç adet eklendiyse her biri için sözlüğe tamamen benzersiz uydurma bir ID ile ekliyoruz!
            for (int i = 0; i < miktar; i++)
            {
                // Rastgele ve benzersiz bir eksi sayı üretiyoruz (Örn: -4729384) 
                // Böylece orijinal pozitif ID'lerle (1, 2, 3) asla çakışmaz ve Dictionary hata vermez!
                int benzersizUydurmaID = Random.Range(-9999999, -1000);
                
                // Eğer şans eseri o sayı sözlükte varsa, benzersiz olana kadar yeni sayı seç
                while (cantaIcerigi.ContainsKey(benzersizUydurmaID))
                {
                    benzersizUydurmaID = Random.Range(-9999999, -1000);
                }

                // Her bir aleti tek tek (adet = 1) olacak şekilde sözlüğe ekle
                cantaIcerigi.Add(benzersizUydurmaID, new EnvanterSlotu(yeniEsya, 1));
            }
        }

        ArayuzuTazele();
    }

    // EŞYA KULLANMA VE HASAR VERME FONKSİYONU
    public bool EsyaKullan(ItemData esya, float alinacakHasar = 20f)
    {
        if (esya == null || !cantaIcerigi.ContainsKey(esya.esyaID))
        {
            Debug.LogWarning($"[Envanter] {esya.esyaAdi} elinizde hiç yok!");
            return false;
        }

        EnvanterSlotu slot = cantaIcerigi[esya.esyaID];

        switch (esya.esyaTipi)
        {
            case ItemData.EsyaTuru.KaliciCihaz:
                Debug.Log($"[Envanter] {esya.esyaAdi} pürüzsüzce kullanıldı. Kalıcı cihaz olduğu için hasar almadı.");
                return true;

            case ItemData.EsyaTuru.SarfMalzemesi:
                slot.adet--;
                if (slot.adet <= 0) cantaIcerigi.Remove(esya.esyaID);
                Debug.Log($"[Envanter] {esya.esyaAdi} tüketildi. Kalan: {(cantaIcerigi.ContainsKey(esya.esyaID) ? slot.adet : 0)}");
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
                        cantaIcerigi.Remove(esya.esyaID);
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

    // Arayüz sorguları için adet getiren yardımcı fonksiyon
    public int EsyaAdetiniGetir(ItemData esya)
    {
        if (esya != null && cantaIcerigi.ContainsKey(esya.esyaID))
        {
            return cantaIcerigi[esya.esyaID].adet;
        }
        return 0;
    }

    // Arayüz sorguları için can barı yüzdesini getiren yardımcı fonksiyon
    public float EsyaDayaniklilikGetir(ItemData esya)
    {
        if (esya != null && cantaIcerigi.ContainsKey(esya.esyaID))
        {
            return cantaIcerigi[esya.esyaID].guncelDayaniklilik;
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

    // ====================================================================
    // YENİ: SIRALI VE BOŞLUKSUZ YERLEŞİM SAĞLAYAN YARDIMCI FONKSİYON
    // ====================================================================
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

    // ====================================================================
    // YENİ: X BUTONUNA BASILDIĞINDA TABLETİ KAPATAN SİHİRLİ FONKSİYON
    // ====================================================================
    public void TabletiKapat()
    {
        if (anaTabletUIObjesi != null)
        {
            anaTabletUIObjesi.SetActive(false); // Tableti komple gizle

            Time.timeScale = 1f; // Dünyayı geri akıt, zaman normal aksın!
            
            Cursor.lockState = CursorLockMode.Locked; // Fareyi tekrar oyuna kilitle
            Cursor.visible = false;                   // İmleci gizle ki FPS moduna dönsün

            // İmleci tekrar varsayılana çekiyoruz
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            
            Debug.Log("[Tablet] X butonuna basıldı, tablet kapatıldı ve dünya geri aktı.");
        }
    }
}