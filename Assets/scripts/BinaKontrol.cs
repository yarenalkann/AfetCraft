using UnityEngine;

public class BinaKontrol : MonoBehaviour
{
    private bool kontrolEdildi = false;
    public bool isGorevBinasi = false;  

    void OnMouseDown() // Küpe tıklandığında çalışır
    {
        if (!kontrolEdildi)
        {
            kontrolEdildi = true;
            GetComponent<Renderer>().material.color = Color.green;
        
        // Eğer bu bir görev binasıysa seviye artsın
            if (isGorevBinasi)
            {
                TabletManager tablet = FindFirstObjectByType<TabletManager>();
                if (tablet != null)
                {
                    tablet.oyuncuSeviyesi++;
                    tablet.SeviyeKontrolEt();
                    Debug.Log("GÖREV TAMAMLANDI: Seviye arttı.");
                }
            }
            else 
            {
                Debug.Log("BAŞVURU İNCELENDİ: Seviye artmadı.");
            // İstersen buraya sadece para puan veya bilinç puanı ekleyebilirsin
            }
        }
    }
}
// ----------------------------
            
            // Görsel geri bildirim: Kontrol edilen bina yeşil olsun
            
            
        
    
