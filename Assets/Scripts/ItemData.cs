using UnityEngine;

[CreateAssetMenu(fileName = "YeniEsyaVerisi", menuName = "AfetCraft/Esya Verisi")]
public class ItemData : ScriptableObject
{
    public enum EsyaTuru { SarfMalzemesi, DayanikliAlet, KaliciCihaz }

    [Header("Eşya Genel Bilgileri")]
    public int esyaID;               
    public string esyaAdi;

    [TextArea(2, 5)] // Müfettişte açıklamalar daha rahat yazılsın diye tatlı bir dokunuş
    public string esyaAciklamasi;           
    
    public Sprite esyaIkonu;         
    public int satinAlmaFiyati;      

    [Header("Mekanik Ayarları (YENİ)")]
    public EsyaTuru esyaTipi;        // Müfettişten çekiç için "DayanikliAlet", eğim ölçer için "KaliciCihaz" seçeceksin.
    public float maksimumDayaniklilik = 100f; // Çekiç ve balyoz için toplam can barı

    public enum EnvanterKategorisi
    {
        AracGerecler,     // Balyoz, Çekiç, Cam, Beton, Sertlik Ölçer, Eğim Ölçer
        UretimMalzemeleri // Çimento, Tuğla, Tamir Kiti, Yardım Kiti, Demir
    }

    [Header("Kategori Ayarı")]
    public EnvanterKategorisi esyaKategorisi;

    [Header("Gelişmiş Envanter Ayarları")]
    [Tooltip("Eğer işaretliyse bu eşya slotta üst üste birikir (Çimento gibi). İşaretli değilse tek tek slot kaplar (Balyoz gibi).")]
    public bool ustUsteBiniyorMu = true; 

    [Tooltip("Eğer işaretliyse slotun altında adet yazısı (x5 vb.) görünür. Kalıcı cihazlar için bunu kapatabilirsin.")]
    public bool adetYazesiGosterilsinMi = true; // InventoryUIManager ile senkron kalması için senin orijinal ismin korundu

    // ====================================================================
    // 🛒 AFETCRAFT MAĞAZA VE CRAFT KORUMA AYARLARI (YENİ EKLEDİĞİMİZ ALAN)
    // ====================================================================
    [Header("Mağaza & Üretim Filtreleri")]
    [Tooltip("Eğer işaretliyse bu eşya dükkanda listelenip satın alınabilir. Kolonlar için bunu kapatacaksın!")]
    public bool magazadaSatilabilir = true; 

    [Tooltip("Eğer işaretliyse bu eşya sadece craft tezgahında üretilerek elde edilir.")]
    public bool sadeceCraftIleEldeEdilir = false; 

    [Header("Eldeki Görünüm Settings")]
    [Tooltip("Bu eşya ele alındığında karakterin elinde görünecek olan 3D Model Prefabı (Örn: El Çekici, El Cam Küpü).")]
    public GameObject elModelPrefab;
}