using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5.0f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float turnSpeed = 100.0f;

    [Header("Zemin Sensör Ayarları")]
    public float groundCheckRadius = 0.3f; 
    public Vector3 groundCheckOffset = new Vector3(0, -1f, 0); 
    public LayerMask groundMask; 

    [Header("Referanslar")]
    public Transform cameraPivot; // YENİ: Kamerayı taşıyan boş obje
    public Transform headPivot;   // Mevcut kafa pivotun
    public GameObject backCamera;
    public GameObject frontCamera;
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

        transform.Rotate(0, mouseX, 0); // Vücut döner

        if (cameraPivot != null || headPivot != null)
        {
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -30f, 30f);
        
            horizontalHeadRotation = Mathf.Lerp(horizontalHeadRotation, Input.GetAxis("Mouse X") * 20f, Time.deltaTime * 5f);
            horizontalHeadRotation = Mathf.Clamp(horizontalHeadRotation, -40f, 40f);

            // KAMERA: Sadece yukarı-aşağı baksın (X ekseninde)
            if (cameraPivot != null) {
                cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            }

            // KAFA: Senin karakter sağa baktığı için dikey hareket (verticalRotation) 
            // muhtemelen Z ekseninde olmalı. Deneyerek bulalım:
            if (headPivot != null) {
                headPivot.localRotation = Quaternion.Euler(0f, horizontalHeadRotation, verticalRotation);
            }
        }
    }
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Mevcut çalışan yön mantığın (Dokunulmadı)
        Vector3 move = (transform.forward * vertical) + (transform.right * horizontal);
        controller.Move(move * moveSpeed * Time.deltaTime);

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        if (anim != null) anim.SetFloat("Speed", moveAmount);
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