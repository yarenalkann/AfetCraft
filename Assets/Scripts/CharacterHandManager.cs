using UnityEngine;

public class CharacterHandManager : MonoBehaviour
{
    public static CharacterHandManager Instance;

    [Header("El Referansı")]
    [Tooltip("Karakterin sağ elinin altına açtığın o boş 'ElPozisyonu' objesini buraya sürükle.")]
    public Transform elTutucuTransform;

    // Oyuncunun şu an elinde tuttuğu güncel eşyanın verisi
    public ItemData suAnElindekiEsyaData { get; private set; }
    
    // Sahnede elinde anlık olarak klonlanan 3D model objesi
    private GameObject eldekiGuncelModelObjesi;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // ====================================================================
    // ⚔️ EŞYAYI ELE ALMA (EQUIP) MOTORU
    // ====================================================================
    public void EsyaEleAl(ItemData yeniEsya)
    {
        if (yeniEsya == null || elTutucuTransform == null) return;

        // 1. KURAL: Eğer sadece Araç-Gereçler kısmındaki eşyalar ele alınabilecekse kontrol et!
        if (yeniEsya.esyaKategorisi != ItemData.EnvanterKategorisi.AracGerecler)
        {
            Debug.LogWarning($"[El] {yeniEsya.esyaAdi} bir üretim malzemesidir, ele alınamaz!");
            return;
        }

        // Eğer zaten elimizde bir şey varsa onu temizle
        EldekiEsyayiTemizle();

        suAnElindekiEsyaData = yeniEsya;

        // 2. AŞAMA: Eşyanın 3D modelini el tutucunun altında sahnede yaratıyoruz
        if (yeniEsya.elModelPrefab != null)
        {
            eldekiGuncelModelObjesi = Instantiate(yeniEsya.elModelPrefab, elTutucuTransform);
            
            // Lokasyonunu el tutucuya milimetrik sıfırlıyoruz
            eldekiGuncelModelObjesi.transform.localPosition = Vector3.zero;
            eldekiGuncelModelObjesi.transform.localRotation = Quaternion.identity;
        }

        Debug.Log($"<color=cyan>[El]</color> {yeniEsya.esyaAdi} başarıyla ele alındı!");
    }

    public void EldekiEsyayiTemizle()
    {
        if (eldekiGuncelModelObjesi != null)
        {
            Destroy(eldekiGuncelModelObjesi);
        }
        suAnElindekiEsyaData = null;
    }
}