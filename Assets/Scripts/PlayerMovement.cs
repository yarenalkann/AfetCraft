using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5.0f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float turnSpeed = 200.0f;

    [Header("Zemin Sensör Ayarları")]
    public float groundCheckRadius = 0.3f; // Sensörün genişliği
    public Vector3 groundCheckOffset = new Vector3(0, -1f, 0); // Ayaklara indirme mesafesi
    public LayerMask groundMask; // Buradan "Default" katmanını seçeceksin

    [Header("Referanslar")]
    public GameObject backCamera;
    public GameObject frontCamera;
    public Transform headPivot;
    public Animator anim;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float verticalRotation = 0f;
    private float horizontalHeadRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
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

        transform.Rotate(0, mouseX, 0);

        if (headPivot != null)
        {
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -30f, 30f);
            horizontalHeadRotation = Mathf.Lerp(horizontalHeadRotation, Input.GetAxis("Mouse X") * 20f, Time.deltaTime * 5f);
            horizontalHeadRotation = Mathf.Clamp(horizontalHeadRotation, -40f, 40f);

            headPivot.localRotation = Quaternion.Euler(0f, horizontalHeadRotation, verticalRotation);
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = (transform.forward * vertical) + (transform.right * horizontal);
        controller.Move(move * moveSpeed * Time.deltaTime);

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        if (anim != null) anim.SetFloat("Speed", moveAmount);
    }

    void HandleGravityAndJump()
    {    
        
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundMask);

        // Hız sıfırlama (Eksi sonsuza gitmeyi engeller)
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Zıplama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Yerçekimi uygulaması
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Sensörü Scene ekranında görmek için (Hata payını sıfırlar)
    private void OnDrawGizmos()
    {
        // Küre yere değmiyorsa KIRMIZI, değiyorsa YEŞİL olsun
        Gizmos.color = isGrounded ? Color.green : Color.red;
    
        // Küreyi çiziyoruz
        Gizmos.DrawWireSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}