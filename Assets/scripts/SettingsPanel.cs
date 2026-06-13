using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsPanel : MonoBehaviour
{
    [Header("--- GRAFİK AYARLARI ---")]
    public TMP_Dropdown grafikDropdown;
    public TMP_Dropdown cozunurlukDropdown;
    public Toggle tamEkranToggle;
    public TMP_Dropdown fpsDropdown;

    [Header("--- SES AYARLARI ---")]
    public Slider anaSesSlider;
    public Slider muzikSlider;
    public Slider efektSlider;

    // Çözünürlükleri hafızada tutmak için liste
    private Resolution[] tumCozunurlukler;

    private void Start()
    {
        SetupGrafikVeEkran();
        SetupSesAyarlari();
    }

    #region GRAFİK VE EKRAN SİSTEMİ
    private void SetupGrafikVeEkran()
    {
        // 1. Grafik Kalitesi
        if (grafikDropdown != null)
        {
            grafikDropdown.value = QualitySettings.GetQualityLevel();
            grafikDropdown.onValueChanged.AddListener(GrafikKalitesiDegistir);
        }

        // 2. FPS Limiti Popüle Etme ve Ayarlama
        if (fpsDropdown != null)
        {
            fpsDropdown.onValueChanged.AddListener(FpsLimitiDegistir);
        }

        // 3. Tam Ekran Kontrolü
        if (tamEkranToggle != null)
        {
            tamEkranToggle.isOn = Screen.fullScreen;
            tamEkranToggle.onValueChanged.AddListener(TamEkranDegistir);
        }

        // 4. Bilgisayarın Desteklediği Çözünürlükleri Çekme
        if (cozunurlukDropdown != null)
        {
            tumCozunurlukler = Screen.resolutions;
            cozunurlukDropdown.ClearOptions();

            List<string> secenekler = new List<string>();
            int gecerliCozunurlukIndeksi = 0;

            for (int i = 0; i < tumCozunurlukler.Length; i++)
            {
                string secenek = tumCozunurlukler[i].width + "x" + tumCozunurlukler[i].height + " @" + tumCozunurlukler[i].refreshRateRatio.value.ToString("0") + "Hz";
                secenekler.Add(secenek);

                if (tumCozunurlukler[i].width == Screen.currentResolution.width &&
                    tumCozunurlukler[i].height == Screen.currentResolution.height)
                {
                    gecerliCozunurlukIndeksi = i;
                }
            }

            cozunurlukDropdown.AddOptions(secenekler);
            cozunurlukDropdown.value = gecerliCozunurlukIndeksi;
            cozunurlukDropdown.RefreshShownValue();
            cozunurlukDropdown.onValueChanged.AddListener(CozunurlukDegistir);
        }
    }

    public void GrafikKalitesiDegistir(int indeks)
    {
        QualitySettings.SetQualityLevel(indeks, true);
    }

    public void TamEkranDegistir(bool tamEkranMi)
    {
        Screen.fullScreen = tamEkranMi;
    }

    public void FpsLimitiDegistir(int indeks)
    {
        // Dropdown sırasına göre: 0 = 30 FPS, 1 = 60 FPS, 2 = 144 FPS, 3 = Sınırsız
        switch (indeks)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = 144; break;
            case 3: Application.targetFrameRate = -1; break; // Sınırsız
        }
    }

    public void CozunurlukDegistir(int indeks)
    {
        Resolution cozunurluk = tumCozunurlukler[indeks];
        Screen.SetResolution(cozunurluk.width, cozunurluk.height, Screen.fullScreen);
    }
    #endregion

    #region SES SİSTEMİ
    private void SetupSesAyarlari()
    {
        // Ses slider'ları oynatıldıkça çalışacak tetikleyiciler
        if (anaSesSlider != null) { anaSesSlider.value = PlayerPrefs.GetFloat("AnaSes", 0.8f); anaSesSlider.onValueChanged.AddListener(AnaSesAyarla); }
        if (muzikSlider != null) { muzikSlider.value = PlayerPrefs.GetFloat("MuzikSes", 0.6f); muzikSlider.onValueChanged.AddListener(MuzikAyarla); }
        if (efektSlider != null) { efektSlider.value = PlayerPrefs.GetFloat("EfektSes", 0.9f); efektSlider.onValueChanged.AddListener(EfektAyarla); }
    }

    public void AnaSesAyarla(float deger) { AudioListener.volume = deger; PlayerPrefs.SetFloat("AnaSes", deger); }
    public void MuzikAyarla(float deger) { /* İleride buraya müzik AudioSource kanalını bağlayacaksın */ PlayerPrefs.SetFloat("MuzikSes", deger); }
    public void EfektAyarla(float deger) { /* İleride buraya efekt AudioSource kanalını bağlayacaksın */ PlayerPrefs.SetFloat("EfektSes", deger); }
    #endregion

    // Şemandaki o harika KAYDET butonuna basılınca ayarları diske yazacak fonksiyon
    public void AyarlariKaydet()
    {
        PlayerPrefs.Save();
        Debug.Log("[SISTEM] Tüm değişiklikler başarıyla diske kaydedildi!");
        // İstersen kaydettikten sonra paneli otomatik kapattırabiliriz:
        // this.gameObject.SetActive(false);
    }
}