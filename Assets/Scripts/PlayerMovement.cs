using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5.0f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float turnSpeed = 200.0f; 

    [Header("Referanslar")]
    public GameObject backCamera;
    public GameObject frontCamera;
    public Transform headPivot; // Buraya 'Neck' objesini sürükle
    public Animator anim;       // Buraya 'body' objesini sürükle

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // Bakış değişkenleri
    private float verticalRotation = 0f; 
    private float horizontalHeadRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Fareyi oyun ekranına hapseder
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        HandleCameraSwitch();
        HandleRotation();
        HandleMovement();
        HandleGravityAndJump();
    }

    void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            bool isBackActive = backCamera.activeSelf;
            backCamera.SetActive(!isBackActive);
            frontCamera.SetActive(isBackActive);
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime;

        // 1. TÜM VÜCUT: Sağa-sola dönüş (Root objeyi döndürür)
        transform.Rotate(0, mouseX, 0); 

        // 2. KAFA KONTROLÜ:
        if (headPivot != null)
        {
            // Yukarı - Aşağı Bakış (Senin modelinde Kırmızı Halkaya rağmen Z değişiyordu)
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -30f, 30f); // Boyun kırılmasın

            // Sağa - Sola Kafa Esnemesi (Vücut dönerken kafanın biraz daha fazla dönmesi için)
            // Fare hareketine göre kafayı kendi içinde de biraz döndürüyoruz
            horizontalHeadRotation = Mathf.Lerp(horizontalHeadRotation, Input.GetAxis("Mouse X") * 20f, Time.deltaTime * 5f);
            horizontalHeadRotation = Mathf.Clamp(horizontalHeadRotation, -40f, 40f);

            /* KRİTİK NOKTA: Senin testlerine göre;
               Yukarı-Aşağı = Z ekseni (3. parametre)
               Sağa-Sola = Y ekseni (2. parametre)
            */
            headPivot.localRotation = Quaternion.Euler(0f, horizontalHeadRotation, verticalRotation);
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); 
        float vertical = Input.GetAxis("Vertical");     

        Vector3 move = (transform.forward * vertical) + (transform.right * horizontal);
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Animasyon parametresini güncelle (Speed)
        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        if (anim != null)
        {
            anim.SetFloat("Speed", moveAmount);
        }
    }

    void HandleGravityAndJump()
    {
        isGrounded = controller.isGrounded;
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}