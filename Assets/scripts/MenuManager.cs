using UnityEngine;
using UnityEngine.SceneManagement; // Sahneler arası geçiş yapabilmek için bu kütüphane ŞART!

public class MenuManager : MonoBehaviour
{
    // BAŞLA butonuna tıklandığında bu fonksiyon çalışacak
    public void OyunuBaslat()
    {
        // "OyunSahnesi" yazan yere senin binalarının, tabletinin olduğu asıl oyun sahnesinin adı neyse onu yazmalısın!
        // Eğer asıl sahnenin adı "SampleScene" ise burayı "SampleScene" yap.
        SceneManager.LoadScene("_Recovery/0"); 
        
    }

    // ÇIKIŞ butonuna tıklandığında bu fonksiyon çalışacak
    public void OyundanCik()
    {
        Application.Quit(); // Oyunu tamamen kapatır (Telefon veya PC'de çalışır)
        Debug.Log("SİSTEM: Oyundan başarıyla çıkıldı."); // Unity içinde çalıştığını görmek için
    }
}
