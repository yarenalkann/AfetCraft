using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "YeniTarif", menuName = "Envanter/Craft Tarifi")]
public class CraftRecipe : ScriptableObject
{
    [System.Serializable]
    public class GerekliMalzeme
    {
        public ItemData malzemeData; // Hangi malzeme? (Örn: Çimento)
        public int adet;             // Kaç tane lazım? (Örn: 2)
    }

    [Header("Formül Bilgileri")]
    public string tarifAdi;          // Üretim panelinde görünecek başlık
    public List<GerekliMalzeme> gerekliMalzemeler = new List<GerekliMalzeme>();

    [Header("Üretim Çıktısı")]
    public ItemData uretilecekEsya;  // İşlem bitince oyuncuya verilecek eşya (Örn: Tamir Kiti)
    public int uretilecekAdet = 1;   // Tek seferde kaç adet üretilsin?
}