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
    public LayerMask interactableLayer; // Sadece etkileşimli objeleri taramak için

    private LineRenderer lineRenderer;
    private IInteractable lastHighlighted; // Hafızadaki son parlatılan obje

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

        // Senin karakter yönüne göre lazer: -neckPivot.right
        Ray ray = new Ray(laserOrigin.position, -neckPivot.right);
        RaycastHit hit;
        Vector3 endPoint = laserOrigin.position + (-neckPivot.right * interactionDistance);

        // --- HIGHLIGHT VE RAYCAST MANTIĞI ---
        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            endPoint = hit.point;

            // Çarptığımız şey IInteractable arayüzüne sahip mi?
            if (hit.collider.TryGetComponent(out IInteractable currentInteractable))
            {
                interactionUI.SetActive(true);

                // Eğer yeni bir objeye bakıyorsak parlatalım
                if (currentInteractable != lastHighlighted)
                {
                    if (lastHighlighted != null) lastHighlighted.ToggleHighlight(false);
                    currentInteractable.ToggleHighlight(true);
                    lastHighlighted = currentInteractable;
                }

                // ETKİLEŞİM (E Tuşu)
                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentInteractable.Interact();
                }
            }
            else
            {
                ClearHighlight();
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

    // Parlamayı söndüren ve UI'ı kapatan yardımcı fonksiyon
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