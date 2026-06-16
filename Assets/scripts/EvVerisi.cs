using UnityEngine;

[CreateAssetMenu(fileName = "YeniEvVerisi", menuName = "Sistem/Ev Verisi")]
public class EvVerisi : ScriptableObject
{

    public string houseID;          // Evin adı/numarası
    public string houseName;        // Ev adı
    public string applicantName;    // Başvuru sahibi
    public string date;             // Başvuru tarihi

    public string address;          // Adres bilgisi
    public Vector3 houseLocation; // X, Y, Z Koordinatları artık burada kabak gibi görünecek!


    [TextArea(3, 10)]
    public string reportDetail;     // Rapor detayı
    
    public enum BuildingStatus { Safe, NeedsRepair, Demolish }
    public BuildingStatus correctStatus; 
    
    public int rewardMoney = 500;   
}