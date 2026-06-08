using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [System.Serializable]
    public struct DinamikUIKutusu
    {
        public GameObject slotAnaObjesi;     // Sahnede hep görünecek olan o mor kare kutu (Arka plan)
        public Image esyaIkonResmi;          // İçindeki boş Image bileşeni (Pikselli ikon için)
        public TextMeshProUGUI adetYazisi;   // İçindeki TextMeshPro bileşeni (x1, x5 yazısı için)
    }

    [Header("Envanter Slotları (Sıralı Boş Kutular)")]
    public List<DinamikUIKutusu> uiKutulari = new List<DinamikUIKutusu>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        EnvanterArayuzunuYenile();
    }

    // GÜNCELLENEN SİHİRLİ YENİLEME FONKSİYONU
    public void EnvanterArayuzunuYenile()
    {
        if (PlayerInventory.Instance == null) return;

        // 1. ADIM: Tüm kutuları ekranda AÇIK bırakıyoruz ama içindeki resim ve yazıları GİZLİYORUZ
        foreach (var kutu in uiKutulari)
        {
            if (kutu.slotAnaObjesi != null) 
            {
                kutu.slotAnaObjesi.SetActive(true); // Tasarladığın o güzel mor kareler hep görünür kalacak!
            }
            if (kutu.esyaIkonResmi != null) 
            {
                kutu.esyaIkonResmi.gameObject.SetActive(false); // Başta içindeki resim yok
            }
            if (kutu.adetYazisi != null) 
            {
                kutu.adetYazisi.gameObject.SetActive(false); // Başta miktar yazısı yok
            }
        }

        // 2. ADIM: Çantadaki dolu eşyaları sırayla çekiyoruz
        List<PlayerInventory.EnvanterSlotu> sahipOlunanEsyalar = PlayerInventory.Instance.GetCantaListesi();

        // 3. ADIM: Satın alınan eşyaları, sıradaki boş kutuların İÇİNE giydiriyoruz
        for (int i = 0; i < sahipOlunanEsyalar.Count; i++)
        {
            if (i >= uiKutulari.Count) break;

            var cantaSlotu = sahipOlunanEsyalar[i];
            var hedefUIKutusu = uiKutulari[i]; // Sıradaki kare slot

            // Kare zaten açık, şimdi sadece içindeki resmi ve yazıyı aktifleştirip dolduruyoruz
            if (hedefUIKutusu.esyaIkonResmi != null)
            {
                hedefUIKutusu.esyaIkonResmi.sprite = cantaSlotu.esya.esyaIkonu;
                hedefUIKutusu.esyaIkonResmi.gameObject.SetActive(true);
            }

            if (hedefUIKutusu.adetYazisi != null)
            {
                hedefUIKutusu.adetYazisi.text = "x" + cantaSlotu.adet;
                hedefUIKutusu.adetYazisi.gameObject.SetActive(true);
            }
        }
    }
}