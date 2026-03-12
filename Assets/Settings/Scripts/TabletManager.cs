using UnityEngine;

public class TabletManager : MonoBehaviour
{
    public bool isTabletOpen = false;
    public GameObject tabletPanel; 

    
    [Header("Sayfa Panelleri")]
    public GameObject basvurularPage;
    public GameObject gorevlerPage;
    public GameObject hasarKriterleriPage; 
    public GameObject insaKriterleriPage;  
    public GameObject magazaPage;

    void Start()
    {
        // Oyun başında tableti gizle
        if (tabletPanel != null)
        {
            tabletPanel.SetActive(false);
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTabletOpen = !isTabletOpen;

            if (tabletPanel != null)
            {
                tabletPanel.SetActive(isTabletOpen);
            }

            
            if (isTabletOpen)
            {
                OpenBasvurular();
            }
        }
    }

    
    

    public void OpenBasvurular()
    {
        CloseAllPages();
        basvurularPage.SetActive(true);
    }

    public void OpenGorevler()
    {
        CloseAllPages();
        gorevlerPage.SetActive(true);
    }

    public void OpenHasarKriterleri() // YENİ
    {
        CloseAllPages();
        hasarKriterleriPage.SetActive(true);
    }

    public void OpenInsaKriterleri() // YENİ
    {
        CloseAllPages();
        insaKriterleriPage.SetActive(true);
    }

    public void OpenMagaza()
    {
        CloseAllPages();
        magazaPage.SetActive(true);
    }

   
    private void CloseAllPages()
    {
        basvurularPage.SetActive(false);
        gorevlerPage.SetActive(false);
        hasarKriterleriPage.SetActive(false);
        insaKriterleriPage.SetActive(false);
        magazaPage.SetActive(false);
    }

    
    public void CloseTablet()
    {
        isTabletOpen = false;
        tabletPanel.SetActive(false);
    }

    
    public void GoreviOnayla()
    {
        Debug.Log("GÖREV ALINDI: #1001 numaralı ev için onarım süreci başlatılıyor!");
        CloseTablet(); 
    }
}