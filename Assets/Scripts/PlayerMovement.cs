using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float turnSpeed = 200.0f; 

    // YENİ: Kameralarımız için boşluklar
    public GameObject backCamera;
    public GameObject frontCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        // --- 1. KAMERA DEĞİŞTİRME ('V' TUŞU) ---
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Arka kamera açıksa önü aç, ön açıksa arkayı aç
            bool isBackActive = backCamera.activeSelf;
            backCamera.SetActive(!isBackActive);
            frontCamera.SetActive(isBackActive);
        }

        // --- 2. FARE İLE SAĞA SOLA DÖNME ---
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
        transform.Rotate(0, mouseX, 0); 

        // --- 3. ZEMİN KONTROLÜ ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // --- 4. HAREKET ---
        float horizontal = Input.GetAxis("Horizontal"); 
        float vertical = Input.GetAxis("Vertical");     
        Vector3 move = (transform.forward * vertical) + (transform.right * horizontal);
        controller.Move(move * moveSpeed * Time.deltaTime);

        // --- 5. ZIPLAMA ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- 6. YERÇEKİMİ ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}