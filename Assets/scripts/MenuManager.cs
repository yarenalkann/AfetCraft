using UnityEngine;
using UnityEngine.SceneManagement; // Sahneler arası geçiş yapabilmek için bu kütüphane ŞART!

public class MenuManager : MonoBehaviour
{
    [Header("Panel Ayarları")]
    public GameObject ayarlarPaneli; // Unity Editör'den hiyerarşideki AyarlarPaneli'ni buraya sürükleyeceğiz

    // BAŞLA butonuna tıklandığında bu fonksiyon çalışacak
    public void OyunuBaslat()
    {
        // "OyunSahnesi" yazan yere senin binalarının, tabletinin olduğu asıl oyun sahnesinin adı neyse onu yazmalıyız.
        // Eğer asıl sahnenin adı "SampleScene" ise burayı "SampleScene" yap.
        SceneManager.LoadScene("0 (18)");
    }

    // AYARLAR butonuna tıklandığında bu fonksiyon çalışacak
    public void AyarlarAc()
    {
        if (ayarlarPaneli != null)
        {
            ayarlarPaneli.SetActive(true); // Ayarlar panelini görünür yapar
        }
    }

    // Ayarlar panelindeki KAPAT butonuna tıklandığında bu fonksiyon çalışacak
    public void AyarlarKapat()
    {
        if (ayarlarPaneli != null)
        {
            ayarlarPaneli.SetActive(false); // Ayarlar panelini gizler
        }
    }

    // ÇIKIŞ butonuna tıklandığında bu fonksiyon çalışacak
    public void OyundanCik()
    {
        Application.Quit(); // Oyunu tamamen kapatır (Telefon veya PC'de çalışır)
        Debug.Log("SİSTEM: Oyundan başarıyla çıkıldı."); // Unity içinde çalıştığını görmek için
    }
}