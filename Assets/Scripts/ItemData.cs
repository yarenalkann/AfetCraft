using UnityEngine;

[CreateAssetMenu(fileName = "YeniEsyaVerisi", menuName = "Afet Kiraat/Esya Verisi")]
public class ItemData : ScriptableObject
{
    public enum EsyaTuru { SarfMalzemesi, DayanikliAlet, KaliciCihaz }

    [Header("Eşya Genel Bilgileri")]
    public int esyaID;               
    public string esyaAdi;

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
    public bool adetYazisiGosterilsinMi = true;
}