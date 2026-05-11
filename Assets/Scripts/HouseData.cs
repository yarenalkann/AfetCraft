using UnityEngine;

[CreateAssetMenu(fileName = "YeniEvVerisi", menuName = "Sistem/Ev Verisi")]
public class HouseData : ScriptableObject
{
    public string houseID;         // Evin adı/numarası
    [TextArea(3, 10)] 
    public string houseDescription; // Raporda görünecek teknik açıklama
    
    // Evin gerçek durumu (0: Güvenli, 1: Onarılmalı, 2: Yıkılmalı)
    public enum BuildingStatus { Safe, NeedsRepair, Demolish }
    public BuildingStatus correctStatus; 
    
    public int rewardMoney = 500;  // Doğru bilinirse gelecek para
}