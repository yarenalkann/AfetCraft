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
    public float interactionDistance = 15f; // Yüksekten de zemini pürüzsüz yakalasın diye
    public Color laserColor = Color.green; 

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

        // Panel Açıkken Dünyayı ve Etkileşimleri Kilitleme
        if ((PlayerInventory.Instance != null && PlayerInventory.Instance.anaTabletUIObjesi != null && PlayerInventory.Instance.anaTabletUIObjesi.activeSelf) ||
            (ReportUIManager.Instance != null && ReportUIManager.Instance.reportPanel != null && ReportUIManager.Instance.reportPanel.activeSelf))
        {
            ClearHighlight();
            return;
        }

        Ray ray = new Ray(laserOrigin.position, -neckPivot.right);
        RaycastHit hit;
        Vector3 endPoint = laserOrigin.position + (-neckPivot.right * interactionDistance);

        // Katman maskelerini devreden çıkarıp her şeye (Zemin ve Evler) çarpan saf raycast
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            endPoint = hit.point;

            // 🔀 1. ETKİLEŞİM KATMANI: IInteractable (E Tuşu ile Rapor Açma)
            if (hit.collider.TryGetComponent(out IInteractable currentInteractable))
            {
                HouseInspector hedefEv = hit.collider.GetComponentInParent<HouseInspector>();
                if (hedefEv != null && hedefEv.raporGonderildiMi)
                {
                    interactionUI.SetActive(false); 
                }
                else
                {
                    interactionUI.SetActive(true); 
                }

                if (currentInteractable != lastHighlighted)
                {
                    if (lastHighlighted != null) lastHighlighted.ToggleHighlight(false);
                    currentInteractable.ToggleHighlight(true);
                    lastHighlighted = currentInteractable;
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentInteractable.Interact();
                }
            }
            else
            {
                if (hit.collider.GetComponent<BuildableCube>() != null)
                {
                    interactionUI.SetActive(false);
                }
                else
                {
                    ClearHighlight();
                }
            }

            // ====================================================================
            // 🔨 2. ETKİLEŞİM KATMANI: SOL TIK İNŞAAT VE YIKIM MOTORU
            // ====================================================================
            if (Input.GetMouseButtonDown(0))
            {
                if (CharacterHandManager.Instance != null && CharacterHandManager.Instance.suAnElindekiEsyaData != null)
                {
                    ItemData eldekiEsyaData = CharacterHandManager.Instance.suAnElindekiEsyaData;
                    string eldekiEsyaAdi = eldekiEsyaData.esyaAdi;

                    // 🧱 SENARYO 1: KÜP İNŞA ETME
                    if (eldekiEsyaAdi == "Beton Blok")
                    {
                        // Canlı envanter miktar kontrolü (Sözlükten adet çekiyoruz)
                        if (PlayerInventory.Instance != null)
                        {
                            int gercekKalanAdet = PlayerInventory.Instance.EsyaAdetiniGetir(eldekiEsyaData);
                            if (gercekKalanAdet <= 0)
                            {
                                Debug.LogWarning("[İnşaat] Envanterde Beton Blok kalmadı!");
                                return; 
                            }
                        }

                        // Kalın/Şişmiş zeminler için milimetrik pivot düzeltme motoru
                        Vector3 hamPozisyon = hit.point + (hit.normal * 0.5f);
                        Vector3 insaatPozisyonu;

                        // Dümdüz zemine yukarıdan bakıldığında tavan hizasına çivileme hesabı
                        if (Mathf.Abs(hit.normal.y) > 0.9f)
                        {
                            insaatPozisyonu = new Vector3(
                                Mathf.Round(hamPozisyon.x),
                                hit.point.y + 0.5f, 
                                Mathf.Round(hamPozisyon.z)
                            );
                        }
                        else // Yan yüzeyler veya duvarlar için standart yuvarlama
                        {
                            insaatPozisyonu = new Vector3(
                                Mathf.Round(hamPozisyon.x),
                                Mathf.Round(hamPozisyon.y),
                                Mathf.Round(hamPozisyon.z)
                            );
                        }

                        // Küp fiziksel üretimi
                        GameObject yeniKup = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        yeniKup.transform.position = insaatPozisyonu;
                        yeniKup.name = "Beton_Blok";

                        Texture2D yüklenenDokusu = Resources.Load<Texture2D>("BetonTexture");
                        if (yüklenenDokusu != null)
                        {
                            yeniKup.GetComponent<Renderer>().material.mainTexture = yüklenenDokusu;
                        }
                        else
                        {
                            yeniKup.GetComponent<Renderer>().material.color = Color.gray;
                        }

                        yeniKup.layer = LayerMask.NameToLayer("Interactable"); 
                        yeniKup.AddComponent<BuildableCube>();

                        // Blok adedini 1 düşürüyoruz
                        if (PlayerInventory.Instance != null)
                        {
                            PlayerInventory.Instance.EsyaKullan(eldekiEsyaData, 1f);
                            
                            // 🎯 Arayüzü tazele ki hotbar'daki sayı anında azalsın
                            if (InventoryUIManager.Instance != null)
                            {
                                InventoryUIManager.Instance.EnvanterArayuzunuYenile();
                            }
                        }

                        Debug.Log($"[İnşaat] Blok yerleştirildi. Konum: {insaatPozisyonu}");
                    }
                    
                    // 🔨 SENARYO 2: BALYOZLA KIRMA
                    else if (eldekiEsyaAdi == "Balyoz")
                    {
                        BuildableCube vurulanKup = hit.collider.GetComponent<BuildableCube>();
                        
                        if (vurulanKup != null)
                        {
                            // Küpün canını azalt/kır
                            vurulanKup.DarbeAl(); 

                            if (PlayerInventory.Instance != null)
                            {
                                // Balyozun canını %10 düşür
                                PlayerInventory.Instance.EsyaKullan(eldekiEsyaData, 10f);
                                Debug.Log($"[Envanter] Balyoz dayanıklılığı %10 azaldı!");

                                // 🎯 SİHİRLİ DOKUNUŞ: Balyoz hasar yediği an Hot Slot/Hotbar barını canlı yenile!
                                if (InventoryUIManager.Instance != null)
                                {
                                    InventoryUIManager.Instance.EnvanterArayuzunuYenile();
                                }
                            }
                        }
                    }

                    // 🪚 SENARYO 3: ÇEKİÇLE EV ONARMA
                    else if (eldekiEsyaAdi == "Çekiç")
                    {
                        HouseInspector hedefEv = hit.collider.GetComponentInParent<HouseInspector>();
                        if (hedefEv != null && hedefEv.raporOnaylandiMi)
                        {
                            hedefEv.CekicVuruldu(); 

                            if (PlayerInventory.Instance != null)
                            {
                                // Çekiç her ev onarma darbesinde %5 hasar yesin
                                PlayerInventory.Instance.EsyaKullan(eldekiEsyaData, 5f);
                                
                                // 🎯 Çekiç hasar yediği an Hot Slot/Hotbar barını canlı yenile!
                                if (InventoryUIManager.Instance != null)
                                {
                                    InventoryUIManager.Instance.EnvanterArayuzunuYenile();
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            ClearHighlight();
        }

        // Lazer çizimi
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