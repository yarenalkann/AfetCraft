using UnityEngine;

public class TabletManager : MonoBehaviour
{
    public bool isTabletOpen = false;
    public GameObject tabletPanel; 

    public GameObject basvurularPage;
    public GameObject gorevlerPage;
    public GameObject magazaPage;

    void Start()
    {
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
        basvurularPage.SetActive(true);
        gorevlerPage.SetActive(false);
        magazaPage.SetActive(false);
    }

    public void OpenGorevler()
    {
        basvurularPage.SetActive(false);
        gorevlerPage.SetActive(true);
        magazaPage.SetActive(false);
    }

    public void OpenMagaza()
    {
        basvurularPage.SetActive(false);
        gorevlerPage.SetActive(false);
        magazaPage.SetActive(true);
    }

    // --- YENİ EKLENEN KISIM: GÖREV ONAYLAMA SİSTEMİ ---
    public void GoreviOnayla()
    {
        // 1. Sisteme mesaj gönder (Test için Unity konsoluna yazdırıyoruz)
        Debug.Log("GÖREV ALINDI: #1001 numaralı hasarlı ev için onarım süreci başlatılıyor!");

        // 2. Tableti otomatik kapat ve oyuncuyu oyuna döndür
        isTabletOpen = false;
        tabletPanel.SetActive(false);
    }
}