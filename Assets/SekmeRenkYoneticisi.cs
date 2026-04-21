using UnityEngine;
using UnityEngine.UI;

public class SekmeRenkYoneticisi : MonoBehaviour
{
    [Header("Tüm Sekme Butonları")]
    public Image[] tumButonlar;

    [Header("Renk Ayarları")]
    public Color aktifRenk = new Color(0.96f, 0.87f, 0.70f); // Açık Bej (Başvurular sekmesinin rengi)
    public Color pasifRenk = new Color(0.65f, 0.35f, 0.15f); // Koyu Kahverengi

    // Butona tıklandığında çalışacak fonksiyon
    public void SekmeyiAktifYap(Image tiklananButon)
    {
        // 1. Önce tüm butonları pasif renge (koyu) boya
        foreach (Image btn in tumButonlar)
        {
            btn.color = pasifRenk;
        }

        // 2. Sadece tıklanan butonu aktif renge (açık bej) boya
        tiklananButon.color = aktifRenk;
    }
}