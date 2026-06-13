using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public static CraftManager Instance;

    [Header("Oyundaki Tüm Üretim Tarifleri")]
    [Tooltip("Oluşturduğun tüm CraftRecipe dosyalarını bu listeye ekle.")]
    public List<CraftRecipe> tumTarifler = new List<CraftRecipe>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ====================================================================
    // 1. KONTROL: Oyuncunun çantası bu tarif için yeterli mi?
    // ====================================================================
    public bool UretimIcinMalzemeYeterliMi(CraftRecipe tarif)
    {
        if (tarif == null || PlayerInventory.Instance == null) return false;

        foreach (var ihtiyac in tarif.gerekliMalzemeler)
        {
            // Oyuncunun çantasındaki güncel adete bakıyoruz
            int elindekiAdet = PlayerInventory.Instance.EsyaAdetiniGetir(ihtiyac.malzemeData);
            
            // Eğer elindeki miktar tarifte istenenden azsa, üretim yapılamaz!
            if (elindekiAdet < ihtiyac.adet)
            {
                return false;
            }
        }
        return true;
    }

    // ====================================================================
    // 2. OPERASYON: Malzemeleri harca ve yeni eşyayı çantaya koy!
    // ====================================================================
    public bool EsyaUret(CraftRecipe tarif)
    {
        // Önce son bir güvenlik kontrolü yapalım
        if (!UretimIcinMalzemeYeterliMi(tarif))
        {
            Debug.LogWarning($"[Craft] {tarif.tarifAdi} için yeterli malzemeniz yok!");
            return false;
        }

        // 1. AŞAMA: Malzemeleri çantadan tek tek düşüyoruz
        foreach (var harcanacak in tarif.gerekliMalzemeler)
        {
            PlayerInventory.Instance.EsyaAzaltYadaSil(harcanacak.malzemeData, harcanacak.adet);
        }

        // 2. AŞAMA: Yeni üretilen eşyayı oyuncunun çantasına ekliyoruz
        PlayerInventory.Instance.EsyaEkle(tarif.uretilecekEsya, tarif.uretilecekAdet);

        Debug.Log($"<color=green>[Craft] Başarılı! {tarif.uretilecekEsya.esyaAdi} üretildi.</color>");
        
        // UI'ın kendini tazeleyebilmesi için haber uçuruyoruz
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.EnvanterArayuzunuYenile();
        }

        return true;
    }
}