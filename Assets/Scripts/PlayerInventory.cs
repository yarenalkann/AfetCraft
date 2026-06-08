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

    private void Update()
    {
        // KLAVYEDEN 1 TUŞUNA BASINCA: Müfettiş panelindeki 0. sıradaki eşyayı çantaya sırayla ekler
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (testEsyaKartlari != null && testEsyaKartlari.Count > 0 && testEsyaKartlari[0] != null)
            {
                EsyaEkle(testEsyaKartlari[0], 1);
                Debug.Log($"<color=cyan>[Bağımsız Test] 1'e basıldı: {testEsyaKartlari[0].esyaAdi} envantere sırayla yerleşti!</color>");
            }
        }

        // KLAVYEDEN 2 TUŞUNA BASINCA: Müfettiş panelindeki 1. sıradaki eşyayı çantaya sırayla ekler
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (testEsyaKartlari != null && testEsyaKartlari.Count > 1 && testEsyaKartlari[1] != null)
            {
                EsyaEkle(testEsyaKartlari[1], 1);
                Debug.Log($"<color=cyan>[Bağımsız Test] 2'ye basıldı: {testEsyaKartlari[1].esyaAdi} envantere sırayla yerleşti!</color>");
            }
        }
    }
    // ====================================================================

    // EŞYA EKLEME (Dükkandan satın alınınca veya dünyadan toplanınca)
    public void EsyaEkle(ItemData yeniEsya, int miktar)
    {
        if (yeniEsya == null) return;

        // Eğer kalıcı cihazsa (Eğim ölçer vb.) ve çantada zaten 1 tane varsa, tekrar ekleme!
        if (yeniEsya.esyaTipi == ItemData.EsyaTuru.KaliciCihaz && cantaIcerigi.ContainsKey(yeniEsya.esyaID))
        {
            Debug.Log($"[Envanter] {yeniEsya.esyaAdi} kalıcı bir cihazdır, 1 taneden fazlasına gerek yok.");
            return;
        }

        if (cantaIcerigi.ContainsKey(yeniEsya.esyaID))
        {
            cantaIcerigi[yeniEsya.esyaID].adet += miktar;
        }
        else
        {
            cantaIcerigi.Add(yeniEsya.esyaID, new EnvanterSlotu(yeniEsya, miktar));
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
}