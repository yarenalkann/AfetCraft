using UnityEngine;

public class GorevTakipSistemi : MonoBehaviour
{
    [Header("Ayarlar")]
    public int hedefBinaSayisi = 3;
    private int kontrolEdilenBina = 0;

    [Header("Bağlantılar")]
    public TabletManager tabletManager;

    public void BinaKontrolEdildi()
    {
        kontrolEdilenBina++;
        Debug.Log("SİSTEM: Bir bina daha tamamlandı. Mevcut: " + kontrolEdilenBina);

        if (kontrolEdilenBina >= hedefBinaSayisi)
        {
            // TabletManager'daki o havalı başarı ekranını çağırıyoruz
            if (tabletManager != null)
            {
                tabletManager.GorevTamamlandi();
            }
            
            // Görev bittiği için sayacı sıfırlıyoruz
            kontrolEdilenBina = 0;
        }
    }
}