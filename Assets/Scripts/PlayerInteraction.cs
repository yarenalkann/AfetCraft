using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform laserOrigin;   // Kafa yüzündeki boş obje
    public Transform neckPivot;     // Boyun/Kafa pivotu (Yön için)
    public GameObject interactionUI; // Ekranda çıkan "E'ye Bas" yazısı

    [Header("Lazer Ayarları")]
    public bool showLaser = true;   // Lazer görünsün mü? (Buradan açıp kapatabilirsin)
    public float interactionDistance = 3f; // Etkileşim mesafesi
    public Color laserColor = Color.green; // Lazerin rengi

    private LineRenderer lineRenderer;

    void Start()
    {
        // LineRenderer bileşenini kontrol et, yoksa ekle
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Lazerin görsel ayarları
        lineRenderer.startWidth = 0.01f; // Çizgi kalınlığı başı
        lineRenderer.endWidth = 0.01f;   // Çizgi kalınlığı sonu
        lineRenderer.positionCount = 2;
        
        // Pembe görünmemesi için basit bir materyal atayalım
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = laserColor;
    }

    void Update()
    {
        if (laserOrigin == null || neckPivot == null || interactionUI == null) return;

        // Görünmez Raycast ışınını oluştur
        Ray ray = new Ray(laserOrigin.position, -neckPivot.right);
        RaycastHit hit;

        
        Vector3 endPoint = laserOrigin.position + (-neckPivot.right * interactionDistance);

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            endPoint = hit.point; // Lazer bir şeye çarptıysa orada bitsin

            if (hit.collider.CompareTag("Interactable"))
            {
                interactionUI.SetActive(true);
                

                if (Input.GetKeyDown(KeyCode.E))
                {
                    DoInteraction(hit.collider.gameObject);
                }
            }
            else
            {
                interactionUI.SetActive(false);
            }
        }
        else
        {
            interactionUI.SetActive(false);
        }

        // --- LAZERİ GÖSTERME KISMI ---
        if (showLaser)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, laserOrigin.position); // Başlangıç: Göz hizası
            lineRenderer.SetPosition(1, endPoint);             // Bitiş: Çarptığı yer veya max menzil
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void DoInteraction(GameObject obj)
    {
        Debug.Log(obj.name + " ile etkileşime geçildi!");
    }
}