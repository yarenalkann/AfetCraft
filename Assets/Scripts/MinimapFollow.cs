using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [Header("Takip Edilecek Karakter")]
    public Transform playerTransform;

    [Header("Kameranın Yüksekliği (Y Ekseni)")]
    public float height = 30f;

    void LateUpdate()
    {
        // Karakter silindiyse veya atanmadıysa hata vermemesi için kontrol
        if (playerTransform == null) return;

        // Kameranın yeni pozisyonunu hesapla: 
        // X ve Z karakterle tamamen aynı, Y ise bizim belirlediğimiz yükseklikte sabit kalır.
        Vector3 newPosition = new Vector3(playerTransform.position.x, height, playerTransform.position.z);
        
        // Kamerayı yeni pozisyona eşitle
        transform.position = newPosition;
    }
}