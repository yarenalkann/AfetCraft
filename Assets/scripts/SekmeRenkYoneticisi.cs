using UnityEngine;
using UnityEngine.UI;

public class SekmeRenkYoneticisi : MonoBehaviour
{
    [Header("Tüm Sekme Butonları")]
    public Image[] tumButonlar;

    [Header("Renk Ayarları")]
    public Color aktifRenk = Color.black; // Basılan buton Siyah
    public Color pasifRenk = new Color(0.12f, 0.12f, 0.12f, 1f); // Basılmayanlar #1F1F1E Grisi

    private void Awake()
    {
        // Oyun ilk açıldığı EN İLK SANİYEDE renkleri sıfırla (Hepsini önce gri yap)
        ResetlemeButonRenkleri();

        // İlk sekmeyi varsayılan olarak AKTİF (Siyah) yap
        if (tumButonlar != null && tumButonlar.Length > 0 && tumButonlar[0] != null)
        {
            tumButonlar[0].color = aktifRenk;
        }
    }

    // Butona tıklandığında çalışacak fonksiyon
    public void SekmeyiAktifYap(Image tiklananButon)
    {
        ResetlemeButonRenkleri();

        // 2. Sadece tıklanan butonun rengini aktif (siyah) yap
        if (tiklananButon != null) tiklananButon.color = aktifRenk;
    }

    // Tüm butonları pasif gri renge boyayan yardımcı fonksiyon
    private void ResetlemeButonRenkleri()
    {
        foreach (Image btn in tumButonlar)
        {
            if (btn != null) btn.color = pasifRenk;
        }
    }
}