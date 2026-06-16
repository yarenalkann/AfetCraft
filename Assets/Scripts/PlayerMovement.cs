using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float walkSpeed = 5.0f;       // 🎯 İsim netliği için moveSpeed'i walkSpeed yaptık
    public float runSpeed = 9.0f;        // 🏃‍♂️ YENİ: Shift'e basınca çıkılacak koşu hızı
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float turnSpeed = 100.0f;

    [Header("Zemin Sensör Ayarları")]
    public float groundCheckRadius = 0.3f; 
    public Vector3 groundCheckOffset = new Vector3(0, -1f, 0); 
    public LayerMask groundMask; 

    [Header("Referanslar")]
    public Transform cameraPivot; 
    public Transform headPivot;   
    public GameObject backCamera;
    public GameObject frontCamera;
    public Animator anim;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float verticalRotation = 0f;
    private float horizontalHeadRotation = 0f;
    private float currentSpeed;          // 🎯 Anlık hızı hafızada tutacak gizli değişken

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = walkSpeed;        // Oyun başında yürüme hızıyla başlıyoruz
    }

    void Update()
    {
        bool raporAcik = ReportUIManager.Instance != null && ReportUIManager.Instance.reportPanel.activeInHierarchy;
        bool sonucSayfasiAcik = FeedbackPopupManager.Instance != null && FeedbackPopupManager.Instance.yanlisSayfasiPaneli.activeInHierarchy;

        if (raporAcik || sonucSayfasiAcik)
        {
            if (anim != null) anim.SetFloat("Speed", 0f);
            return; 
        }

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

        transform.Rotate(0, mouseX, 0); 

        if (cameraPivot != null || headPivot != null)
        {
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -30f, 30f);
        
            horizontalHeadRotation = Mathf.Lerp(horizontalHeadRotation, Input.GetAxis("Mouse X") * 20f, Time.deltaTime * 5f);
            horizontalHeadRotation = Mathf.Clamp(horizontalHeadRotation, -40f, 40f);

            if (cameraPivot != null) {
                cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            }

            if (headPivot != null) {
                headPivot.localRotation = Quaternion.Euler(0f, horizontalHeadRotation, verticalRotation);
            }
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 🏃‍♂️ SHIFT KONTROLÜ: Oyuncu sol shift'e basıyor mu ve ileri/geri/sağa/sola hareket ediyor mu?
        if (Input.GetKey(KeyCode.LeftShift) && (horizontal != 0 || vertical != 0))
        {
            currentSpeed = runSpeed; // Koşma hızına geç
        }
        else
        {
            currentSpeed = walkSpeed; // Normal yürüme hızına dön
        }

        Vector3 move = (transform.forward * vertical) + (transform.right * horizontal);
        
        // 🎯 Burayı currentSpeed yaptık ki shift durumuna göre dinamik değişsin
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- ANIMATÖR PARAMETRE AYARI ---
        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        
        if (anim != null) 
        {
            // Eğer koşuyorsak Animatördeki Speed değerini 2 ile çarpıp üst limite çekiyoruz (Koşma animasyonunu tetikler)
            if (currentSpeed == runSpeed)
            {
                anim.SetFloat("Speed", moveAmount * 2f); 
            }
            else
            {
                anim.SetFloat("Speed", moveAmount); 
            }
        }
    }

    void HandleGravityAndJump()
    {    
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundMask);

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

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}