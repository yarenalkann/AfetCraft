using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform laserOrigin;   
    public Transform neckPivot;     
    public GameObject interactionUI; 

    [Header("Lazer Ayarları")]
    public bool showLaser = true;   
    public float interactionDistance = 3f; 
    public Color laserColor = Color.green; 
    public LayerMask interactableLayer; 

    private LineRenderer lineRenderer;
    private IInteractable lastHighlighted; 

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = laserColor;
    }

    void Update()
    {
        if (laserOrigin == null || neckPivot == null || interactionUI == null) return;

        // ====================================================================
        // 🎯 KRİTİK GÜVENLİK KORUMASI:
        // Eğer envanter tableti VEYA Rapor ekranı açıksa dünyadaki her şeyi kilitle!
        // Oyuncu rapordaki butonlara basarken arkada kazara çekiç vurmasın.
        // ====================================================================
        if ((PlayerInventory.Instance != null && PlayerInventory.Instance.anaTabletUIObjesi != null && PlayerInventory.Instance.anaTabletUIObjesi.activeSelf) ||
            (ReportUIManager.Instance != null && ReportUIManager.Instance.reportPanel != null && ReportUIManager.Instance.reportPanel.activeSelf))
        {
            ClearHighlight();
            return;
        }

        Ray ray = new Ray(laserOrigin.position, -neckPivot.right);
        RaycastHit hit;
        Vector3 endPoint = laserOrigin.position + (-neckPivot.right * interactionDistance);

        // --- HIGHLIGHT VE RAYCAST MANTIĞI ---
        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            endPoint = hit.point;

            // 🔀 1. ETKİLEŞİM KATMANI: IInteractable Arayüzü (E Tuşu & Raporlar için)
            if (hit.collider.TryGetComponent(out IInteractable currentInteractable))
            {
                // 🎯 SİHİRLİ DOKUNUŞ: Eğer bu ev için rapor zaten teslim edildiyse ekrana asla "E tuşu" yazısını BASMA!
                HouseInspector hedefEv = hit.collider.GetComponentInParent<HouseInspector>();
                if (hedefEv != null && hedefEv.raporGonderildiMi)
                {
                    interactionUI.SetActive(false); // Yazıyı gizle
                }
                else
                {
                    interactionUI.SetActive(true); // Rapor verilmediyse yazıyı göster
                }

                // Eğer yeni bir objeye bakıyorsak parlatalım
                if (currentInteractable != lastHighlighted)
                {
                    if (lastHighlighted != null) lastHighlighted.ToggleHighlight(false);
                    currentInteractable.ToggleHighlight(true);
                    lastHighlighted = currentInteractable;
                }

                // ETKİLEŞİM (E Tuşu - Sadece rapor gönderilmediyse tetiklenebilir)
                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentInteractable.Interact();
                }
            }
            else
            {
                ClearHighlight();
            }

            // ====================================================================
            // 🔨 2. YENİ ETKİLEŞİM KATMANI: ÇEKİÇLEME MOTORU (Sol Tık)
            // ====================================================================
            if (Input.GetMouseButtonDown(0))
            {
                HouseInspector hedefEv = hit.collider.GetComponentInParent<HouseInspector>();

                // 🎯 EKSTRA GÜVENLİK: Sadece raporu DOĞRU bilinen evler sol tık darbesi yiyebilir!
                if (hedefEv != null && hedefEv.raporOnaylandiMi)
                {
                    hedefEv.CekicVuruldu(); // HouseInspector'daki çekiç vuruş motorunu tetikle!
                }
            }
        }
        else
        {
            ClearHighlight();
        }

        // --- LAZERİ GÖSTERME ---
        if (showLaser)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, laserOrigin.position);
            lineRenderer.SetPosition(1, endPoint);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void ClearHighlight()
    {
        interactionUI.SetActive(false);
        if (lastHighlighted != null)
        {
            lastHighlighted.ToggleHighlight(false);
            lastHighlighted = null;
        }
    }
}