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

    // EŞYA KULLANMA VE HASAR VERME FONKSİYONU (İşte senin istediğin o sihirli mekanizma!)
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
                // Eğim ölçer ve beton ölçer hep kalır, hasar yemez, eksilmez!
                Debug.Log($"[Envanter] {esya.esyaAdi} pürüzsüzce kullanıldı. Kalıcı cihaz olduğu için hasar almadı.");
                return true;

            case ItemData.EsyaTuru.SarfMalzemesi:
                // Çimento, tuğla vb. direkt adet olarak 1 azalır
                slot.adet--;
                if (slot.adet <= 0) cantaIcerigi.Remove(esya.esyaID);
                Debug.Log($"[Envanter] {esya.esyaAdi} tüketildi. Kalan: {(cantaIcerigi.ContainsKey(esya.esyaID) ? slot.adet : 0)}");
                ArayuzuTazele();
                return true;

            case ItemData.EsyaTuru.DayanikliAlet:
                // Çekiç ve balyoz adet eksiltmez, can barından düşer!
                slot.guncelDayaniklilik -= alinacakHasar;
                Debug.Log($"[Envanter] {esya.esyaAdi} hasar yedi! Kalan Can: %{slot.guncelDayaniklilik}");

                // Eğer aletin canı tamamen bittiyse
                if (slot.guncelDayaniklilik <= 0)
                {
                    slot.adet--; // 1 adet çekiç kırıldı!
                    Debug.Log($"[Envanter] 1 adet {esya.esyaAdi} tamamen kırıldı ve yok oldu!");

                    if (slot.adet <= 0)
                    {
                        cantaIcerigi.Remove(esya.esyaID); // Elinde hiç çekiç kalmadıysa çantadan sil
                    }
                    else
                    {
                        slot.guncelDayaniklilik = esya.maksimumDayaniklilik; // Yedek çekiç varsa onun canını fulle
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

    // Arayüz sorguları için can barı yüzdesini getiren yardımcı fonksiyon (UI'da göstermek için)
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
}