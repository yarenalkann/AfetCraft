using UnityEngine;
using UnityEngine.SceneManagement;

public class GeriDonusManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Ana sahneye dönmeden önce fareyi görünür yapalım
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Buraya ana sahnenin Unity'deki tam adını yaz (Örn: "MainScene")
            SceneManager.LoadScene("Character"); 
        }
    }
}