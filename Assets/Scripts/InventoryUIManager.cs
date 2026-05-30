using UnityEngine;
using TMPro;
using UnityEngine.UI; // Resimleri kontrol etmek için bunu ekledik
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [System.Serializable]
    public struct EnvanterSlotGrafigi
    {
        public ItemData esyaVerisi;          // Hangi eşya? (Cimento_Data vb.)
        public GameObject slotAnaObjesi;     // Kutucuğun en dıştaki paneli/arkaplanı (Görünmez yapmak için)
        public Image esyaIkonResmi;          // Eşyanın pikselli görseli
        public TextMeshProUGUI adetYazisi;   // "x1" yazan yer
    }

    [Header("Envanter Slot Eşleşmeleri")]
    public List<EnvanterSlotGrafigi> envanterSlotlari = new List<EnvanterSlotGrafigi>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Oyun ilk açıldığında çanta bomboş gözüksün diye arayüzü bir kere tetikliyoruz
        EnvanterArayuzunuYenile();
    }

    public void EnvanterArayuzunuYenile()
    {
        if (PlayerInventory.Instance == null) return;

        foreach (var slot in envanterSlotlari)
        {
            if (slot.esyaVerisi != null)
            {
                // Çantada bu eşyadan kaç tane var sorgula
                int guncelAdet = PlayerInventory.Instance.EsyaAdetiniGetir(slot.esyaVerisi);

                if (guncelAdet > 0)
                {
                    // --------------------------------------------------------
                    // EŞYA SATIN ALINDIYSA: SLOTU GÖRÜNÜR YAP VE DOLDUR
                    // --------------------------------------------------------
                    if (slot.slotAnaObjesi != null) slot.slotAnaObjesi.SetActive(true);
                    
                    if (slot.esyaIkonResmi != null)
                    {
                        slot.esyaIkonResmi.sprite = slot.esyaVerisi.esyaIkonu; // Pikselli resmi yükle
                        slot.esyaIkonResmi.gameObject.SetActive(true);
                    }

                    if (slot.adetYazisi != null)
                    {
                        slot.adetYazisi.text = "x" + guncelAdet; // Adet bilgisini yaz
                        slot.adetYazisi.gameObject.SetActive(true);
                    }
                }
                else
                {
                    // --------------------------------------------------------
                    // OYUNCUDA BU EŞYA YOKSA (BAŞLANGIÇTA): SLOTU TAMAMEN GİZLE
                    // --------------------------------------------------------
                    if (slot.slotAnaObjesi != null)
                    {
                        // Kutucuğu tamamen gizle (Böylece envanter tertemiz bomboş durur)
                        slot.slotAnaObjesi.SetActive(false); 
                    }
                    else
                    {
                        // Eğer sadece içindekileri gizlemek istersen alternatif güvenlik:
                        if (slot.esyaIkonResmi != null) slot.esyaIkonResmi.gameObject.SetActive(false);
                        if (slot.adetYazisi != null) slot.adetYazisi.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}