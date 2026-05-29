using UnityEngine;
using TMPro;

public class FeedbackPopupManager : MonoBehaviour
{
    public static FeedbackPopupManager Instance;

    [Header("Yeni Sayfa Tasarımı Referansları")]
    public GameObject yanlisSayfasiPaneli; // Tasarladığın SonucSayfasiPaneli buraya gelecek
    public TextMeshProUGUI txtUyariMesaji; // İçindeki TxtSonucMesaji buraya gelecek

    private void Awake()
    {
        // Diğer kodlardan bu sayfaya rahatça erişebilmek için köprü kuruyoruz
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Bu fonksiyonu çağırarak ekrana yazıyı fırlatacağız
    public void UyariSayfasiniAc(string mesaj, Color hedefRenk)
    {
        // 1. Adım: Yeni kağıdın içindeki TextMeshPro alanına bizim gönderdiğimiz yazıyı yazar
        txtUyariMesaji.text = mesaj; 
    
        // 2. Adım: Gönderdiğin resimdeki "Vertex Color" kutucuğunun rengini ayarlar (Yeşil veya Kırmızı)
        txtUyariMesaji.color = hedefRenk; 

        // 3. Adım: Sahnedeki o gizli (Deaktif) olan sonuç panelini şak diye görünür yapar
        yanlisSayfasiPaneli.SetActive(true); 

        // 4. Adım: Fare imlecini serbest bırakır ki oyuncu "Tamam" butonuna basabilsin
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // "Tamam" butonuna basıldığında oyuna geri döndüren fonksiyon
    public void SayfayiKapat()
    {
        if (yanlisSayfasiPaneli != null)
        {
            yanlisSayfasiPaneli.SetActive(false); // Sayfayı kapat
        }

        // Fareyi tekrar gizleyip oyuna geri döndürüyoruz
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}