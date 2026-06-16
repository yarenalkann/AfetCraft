using UnityEngine;
using TMPro;

public class FeedbackPopupManager : MonoBehaviour
{
    public static FeedbackPopupManager Instance;

    [Header("Yeni Sayfa Tasarımı Referansları")]
    public GameObject yanlisSayfasiPaneli; 
    public TextMeshProUGUI txtUyariMesaji; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Bu fonksiyonu çağırarak ekrana yazıyı fırlatacağız
    public void UyariSayfasiniAc(string mesaj, Color hedefRenk)
    {
        txtUyariMesaji.text = mesaj; 
        txtUyariMesaji.color = hedefRenk; 
        yanlisSayfasiPaneli.SetActive(true); 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 💰💰💰 PARA GÜNCELLEME SİHRETMİZ TAM BURADA BAŞLIYOR 💰💰💰
        
// 📊 Paranın tutulduğu scripti sahnede buluyoruz
    TabletManager envanter = FindAnyObjectByType<TabletManager>(); 

    if (envanter != null)
    {
        // 🎯 MÜHENDİSLİK HİLESİ: Gelen rengin yeşillik oranını (g) ve kırmızılık oranını (r) kıyaslıyoruz.
        // Tonu ne olursa olsun (açık yeşil, koyu yeşil, zeytin yeşili vb.) yeşil baskınsa DOĞRU kabul edilir!
        if (hedefRenk.g > hedefRenk.r)
        {
            envanter.oyuncuParasi += 2000; // 500 TL Ödül verdik!
        }
        else // Kırmızı baskınsa veya yeşil değilse YANLIŞ kabul edilir!
        {
            envanter.oyuncuParasi -= 1000; // 250 TL Ceza kestik!
            
            // Paranın sıfırın altına düşmesini engelliyoruz
            if (envanter.oyuncuParasi < 0) envanter.oyuncuParasi = 0;
        }

        // Arayüzdeki o pixel art paneli güncelliyoruz
        envanter.ParaYazisiniGuncelle(); 
    }
    }

    public void SayfayiKapat()
    {
        if (yanlisSayfasiPaneli != null)
        {
            yanlisSayfasiPaneli.SetActive(false); 
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}