using UnityEngine;

[CreateAssetMenu(fileName = "YeniEsyaVerisi", menuName = "AfetCraft/Esya Verisi")]
public class ItemData : ScriptableObject
{
    [Header("Eşya Genel Bilgileri")]
    public int esyaID;               // Mağazadaki ID'si (0: Çimento, 1: Demir vb.)
    public string esyaAdi;           // Eşyanın ekranda görünecek adı

    [Header("Görsel Ayarlar")]
    public Sprite esyaIkonu;         // Envanterde ve dükkanda görünecek pikselli resim

    [Header("Ekonomi Ayarları")]
    public int satinAlmaFiyati;      // Mağazadaki fiyatı
}