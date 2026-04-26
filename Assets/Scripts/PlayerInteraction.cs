using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referanslar")]
    // Adım 1'de kafa yüzüne koyduğumuz boş objeyi buraya sürükle
    public Transform laserOrigin; 
    
    // Adım 2'de yarattığımız küçük yeşil küreyi (Sphere) buraya sürükle
    public GameObject worldCursor; 
    
    // Yürüme animasyonunu kilitlediğimiz 'Neck' pivotunu buraya sürükle (Yön için)
    public Transform neckPivot; 

    [Header("Ayarlar")]
    public float interactionDistance = 10f; // TPS için biraz daha uzak menzil
    public bool showLaserBeam = true; // Sahnede lazer çizgisi görünsün mü?

    // Lazer çizgisini Game View'da da göstermek istersek LineRenderer ekleyebiliriz
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            // Eğer LineRenderer yoksa kod hata vermesin diye ekliyoruz
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.green;
        }
    }

    void Update()
    {
        if (laserOrigin == null || worldCursor == null || neckPivot == null) return;

        // --- 1. MÜHENDİSLİK MATEMATİĞİ: Işını Oluştur ---
        // Başlangıç: Göz hizası (laserOrigin)
        // Yön: Boyun/Kafa pivotunun baktığı yön (neckPivot.forward)
        Ray ray = new Ray(laserOrigin.position, -neckPivot.right);
        RaycastHit hit;

        Vector3 endPoint; // Lazerin bittiği nokta (Çarpma noktası veya max menzil)

        // --- 2. RAYCAST: Işını Ateşle ---
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            endPoint = hit.point; // Lazer objeye çarptı
            
            // Yeşil noktayı çarptığımız yere ışınla
            worldCursor.SetActive(true);
            worldCursor.transform.position = hit.point;

            // Eğer Tag 'Interactable' ise kontrolleri yap
            if (hit.collider.CompareTag("Interactable"))
            {
                Debug.Log("Odaklandın: " + hit.collider.gameObject.name);
                
                // Yeşil noktayı büyütüp belli edebilirsin
                worldCursor.transform.localScale = Vector3.one * 0.2f;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    DoInteraction(hit.collider.gameObject);
                }
            }
            else
            {
                // Normal bir şeye çarpıyorsan nokta küçük kalsın
                worldCursor.transform.localScale = Vector3.one * 0.08f;
            }
        }
        else
        {
            // Işın havada kaldı, hiçbir şeye çarpmadı
            endPoint = ray.origin + (ray.direction * interactionDistance);
            
            // Yeşil noktayı max menzilde göster veya gizle
            worldCursor.SetActive(true); 
            worldCursor.transform.position = endPoint;
            worldCursor.transform.localScale = Vector3.one * 0.08f;
        }

        // --- 3. GÖRSEL İLLÜZYON: Lazer Çizgisini Çiz (Game View'da görünür) ---
        if (showLaserBeam && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, laserOrigin.position); // Lazer gözden başlar
            lineRenderer.SetPosition(1, endPoint); // Lazer çarptığı yerde biter
        }

        // Scene Debug (Sadece geliştirici için sahne ekranında kırmızı çizgi)
        Debug.DrawRay(laserOrigin.position, neckPivot.forward * interactionDistance, Color.red);
    }

    void DoInteraction(GameObject obj)
    {
        Debug.Log(obj.name + " ile etkileşime geçildi!");
        // Örnek: Destroy(obj); // Kutuyu yok et
    }
}