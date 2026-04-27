using UnityEngine;

[CreateAssetMenu(fileName = "YeniEv", menuName = "AfetCraft/Ev Verisi")]
public class EvVerisi : ScriptableObject
{
    public string evAdi;
    public string basvuruSahibi;
    public string adres;
    public string tarih;
    public Vector3 evKonumu; // Evin sahnede durduğu X, Y, Z koordinatı
    [TextArea] public string raporDetayi;
}