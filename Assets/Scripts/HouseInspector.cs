using UnityEngine;

public class HouseInspector : MonoBehaviour, IInteractable
{
    [Header("Ev Veri Kartı")]
    public EvVerisi data; 

    [Header("Model Ayarları")]
    public GameObject hasarliModel;
    public GameObject onarilmisModel;

    [Header("Durum Takibi")]
    // 🎯 YENİ KİLİT: Oyuncu doğru veya yanlış, raporu bir kez gönderdi mi?
    [HideInInspector] public bool raporGonderildiMi = false; 
    [HideInInspector] public bool raporOnaylandiMi = false; // Doğru bildiyse true olacak
    [HideInInspector] public bool onarimBittiMi = false;

    private int vurusSayaci = 0;
    private const int MAKS_VURUS = 3;

    void Start()
    {
        if (hasarliModel != null) hasarliModel.SetActive(true);
        if (onarilmisModel != null) onarilmisModel.SetActive(false);
    }

    // ====================================================================
    // 📑 1. SÜREÇ: E TUŞUNA BASINCA RAPOR PANELİNİ AÇAN YER
    // ====================================================================
    public void Interact()
    {
        // 🚨 KİLİT MEKANİZMASI: Eğer bu ev için rapor zaten gönderildiyse E tuşunu tamamen SAĞIR ET!
        if (raporGonderildiMi)
        {
            Debug.Log($"<color=orange>[Sistem]</color> {gameObject.name} için rapor zaten teslim edilmiş. Tekrar E'ye basamazsın!");
            return; 
        }

        if (data != null && ReportUIManager.Instance != null)
        {
            ReportUIManager.Instance.OpenReport(this); 
        }
    }

    // 📝 2. SÜREÇ: Rapor doğru onaylandığında çekiç vurma iznini açar
    public void OnarimIzniniAktifEt()
    {
        raporOnaylandiMi = true;
        Debug.Log($"[Sistem] {gameObject.name} onarım izni açıldı. Çekiç vurulabilir!");
    }

    // ====================================================================
    // 🔨 3. SÜREÇ: SOL TIKLA ÇEKİÇ VURULAN YER
    // ====================================================================
    public void CekicVuruldu()
    {
        if (onarimBittiMi) return;

        // Rapor onaylanmadıysa (veya yanlış bildiyse) sol tık da işlemesin
        if (!raporOnaylandiMi)
        {
            Debug.LogWarning("[Sistem] Bu binanın onarım izni yok! (Rapor hatalı veya doldurulmadı)");
            return;
        }

        if (CharacterHandManager.Instance == null || CharacterHandManager.Instance.suAnElindekiEsyaData == null)
        {
            Debug.LogWarning("[Sistem] Elin boş! Çekiç kuşanmalısın.");
            return;
        }

        if (CharacterHandManager.Instance.suAnElindekiEsyaData.esyaAdi != "Çekiç")
        {
            Debug.LogWarning("[Sistem] Bu binayı onarmak için Çekiç kullanmalısın!");
            return;
        }

        vurusSayaci++;
        Debug.Log($"[İnşaat] Çekiç Darbesi: {vurusSayaci}/{MAKS_VURUS}");

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.EsyaKullan(CharacterHandManager.Instance.suAnElindekiEsyaData, 10f);
        }

        if (vurusSayaci >= MAKS_VURUS)
        {
            EviYenile();
        }
    }

    private void EviYenile()
    {
        onarimBittiMi = true;

        if (onarilmisModel != null) 
        {
            // 🎯 ADIM 1: Yeni evi tam olarak hasarlı evin olduğu konuma ve açıya klonla
            GameObject yeniEv = Instantiate(onarilmisModel, transform.position, transform.rotation);
            
            // 🎯 ADIM 2: Eğer prefab'ın kendisi kapalıysa zorla AÇ (Gözükmeme sorununu çözer!)
            yeniEv.SetActive(true);

            // 🎯 ADIM 3: Boyutunu (Scale) hasarlı evle (2.43) milimetrik olarak eşitle
            yeniEv.transform.localScale = transform.localScale;

            // 🎯 ADIM 4: Katmanını ayarla ki bir daha çekiç vurulmasın
            yeniEv.layer = LayerMask.NameToLayer("Default");

            Debug.Log($"[İnşaat] {onarilmisModel.name} başarıyla dünyaya klonlandı ve aktif edildi.");
        }
        else
        {
            Debug.LogError("[Hata] Inspector'da Onarılmış Model (orijinalEv Prefabı) ATANMAMIŞ!");
            return; // Eğer model yoksa aşağıya geçip eski evi silme ki oyun patlamasın!
        }

        Debug.Log($"<color=green>[BAŞARI]</color> {gameObject.name} dönüşümü tamamlandı! Eski ev kaldırılıyor.");

        // 🎯 ADIM 5: Yeni ev saniyeler içinde doğduğuna göre artık eski hasarlı objeyi silebiliriz
        Destroy(gameObject); 
    }

    public void ToggleHighlight(bool isOn)
    {
        // Rapor bittiyse veya onarıldıysa yeşil lazer parlamasını (Highlight) kapatmak istersen:
        if (raporGonderildiMi && !raporOnaylandiMi) isOn = false; // Yanlış bildiyse parlama sönsün
        if (onarimBittiMi) isOn = false; // Onarım bittiyse parlama sönsün

        if (GetComponentInChildren<Outline>() != null)
            GetComponentInChildren<Outline>().enabled = isOn;
    }
}