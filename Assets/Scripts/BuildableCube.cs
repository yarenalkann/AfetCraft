using UnityEngine;

public class BuildableCube : MonoBehaviour
{
    [Header("Blok Ayarları")]
    public int blokCani = 2; // Balyozla kaç vuruşta kırılsın?

    // 🔨 Balyoz vurulduğunda çağrılacak fonksiyon
    public void DarbeAl()
    {
        blokCani--;
        Debug.Log($"[İnşaat] Blok Darbe Aldı! Kalan Can: {blokCani}");

        if (blokCani <= 0)
        {
            Debug.Log($"<color=red>[Yıkım]</color> Blok parça parça oldu!");
            Destroy(gameObject); // Küpü dünyadan sil
        }
    }
}