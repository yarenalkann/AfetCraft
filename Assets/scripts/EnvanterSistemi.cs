using UnityEngine;

public class EnvanterSistemi : MonoBehaviour
{
    [Header("Envanter Paneli")]
    public GameObject envanterSayfasi;

    void Update()
    {
        // Klavyeden 'I' tuşuna basıldığını algıla
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Panelin şu anki durumunu kontrol et (Açıksa kapat, kapalıysa aç)
            bool suAnAcikMi = envanterSayfasi.activeSelf;
            envanterSayfasi.SetActive(!suAnAcikMi);
        }
    }
}