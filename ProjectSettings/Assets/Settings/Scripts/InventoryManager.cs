using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public bool isInventoryOpen = false;
    public GameObject inventoryPanel; 

    void Start()
    {
        // Oyun başlarken paneli kapalı tut
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isInventoryOpen);
            }
        }
    }
}