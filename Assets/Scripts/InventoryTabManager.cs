using UnityEngine;
using TMPro; // Yazı renklerini değiştirmek için bu kütüphane şart

public class InventoryTabManager : MonoBehaviour
{
    [Header("Sayfa İçerik Grupları")]
    public GameObject envanterSayfaGrubu; // Envanter_Grubu objesi
    public GameObject craftSayfaGrubu;    // Uretim_Grubu objesi

    [Header("Sekme Yazı Objeleri (TextMeshPro)")]
    public TextMeshProUGUI envanterButonYazisi; // ENVANTER butonunun içindeki Text objesi
    public TextMeshProUGUI craftButonYazisi;    // ÜRETİM butonunun içindeki Text objesi

    [Header("Yazı Renk Ayarları")]
    public Color aktifYaziRengi = new Color(0.8705882f, 0.5411765f, 0.09803922f); // Seçili olan sekme turuncu/aktif renk olsun
    public Color pasifYaziRengi = new Color(0.8274511f, 0.8274511f, 0.8274511f);            // Seçili olmayan sekme beyaz/sabit kalsın

    private void Start()
    {
        // Oyun ilk açıldığında Envanter sayfası açık gelsin
        EnvanterSekmesiniAc();
    }

    public void EnvanterSekmesiniAc()
    {
        // Panelleri ayarla
        envanterSayfaGrubu.SetActive(true);
        craftSayfaGrubu.SetActive(false);

        // Sadece yazı renklerini değiştiriyoruz
        if (envanterButonYazisi != null) envanterButonYazisi.color = aktifYaziRengi;
        if (craftButonYazisi != null) craftButonYazisi.color = pasifYaziRengi;
    }

    public void CraftSekmesiniAc()
    {
        // Panelleri ayarla
        envanterSayfaGrubu.SetActive(false);
        craftSayfaGrubu.SetActive(true);

        // Sadece yazı renklerini değiştiriyoruz
        if (envanterButonYazisi != null) envanterButonYazisi.color = pasifYaziRengi;
        if (craftButonYazisi != null) craftButonYazisi.color = aktifYaziRengi;
    }
}