using UnityEngine;

public class BinaKontrol : MonoBehaviour
{
    private bool kontrolEdildi = false;

    void OnMouseDown() // Küpe tıklandığında çalışır
    {
        if (!kontrolEdildi)
        {
            kontrolEdildi = true;
            // TabletManager'daki sayacı artır (Önceki konuşmalarımızdaki fonksiyon)
            FindObjectOfType<TabletManager>().BinaKontrolEt(); 
            
            // Görsel geri bildirim: Kontrol edilen bina yeşil olsun
            GetComponent<Renderer>().material.color = Color.green;
            Debug.Log(gameObject.name + " kontrol edildi!");
        }
    }
}