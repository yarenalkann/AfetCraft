using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletButtonColorizer : MonoBehaviour
{
    [System.Serializable]
    public class TabButtonData
    {
        public Button button;              // Tıklanacak buton
        public TextMeshProUGUI buttonText; // Butonun içindeki yazı
    }

    [Header("Tüm Sekme Butonları")]
    public TabButtonData[] tabs;

    [Header("Renk Ayarları")]
    public Color normalColor = new Color(0.88f, 0.85f, 0.79f); // Kemik Rengi (#E1D5C9)
    public Color activeColor = new Color(1f, 0.53f, 0f);       // Canlı Turuncu (#FF8800)

    // Bu fonksiyonu butonlar tıklandığında çağıracağız
    public void ColorizeTab(Button clickedButton)
    {
        foreach (var tab in tabs)
        {
            if (tab.button == clickedButton)
            {
                // Tıklanan butonun yazısını turuncu yap
                tab.buttonText.color = activeColor;
            }
            else
            {
                // Diğer tüm butonların yazısını kemik rengine döndür
                tab.buttonText.color = normalColor;
            }
        }
    }
}