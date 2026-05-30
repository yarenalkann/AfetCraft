using UnityEngine;

[CreateAssetMenu(fileName = "YeniEsyaVerisi", menuName = "Afet Kiraat/Esya Verisi")]
public class ItemData : ScriptableObject
{
    public enum EsyaTuru { SarfMalzemesi, DayanikliAlet, KaliciCihaz }

    [Header("Eşya Genel Bilgileri")]
    public int esyaID;               
    public string esyaAdi;           
    
    public Sprite esyaIkonu;         
    public int satinAlmaFiyati;      

    [Header("Mekanik Ayarları (YENİ)")]
    public EsyaTuru esyaTipi;        // Müfettişten çekiç için "DayanikliAlet", eğim ölçer için "KaliciCihaz" seçeceksin.
    public float maksimumDayaniklilik = 100f; // Çekiç ve balyoz için toplam can barı
}