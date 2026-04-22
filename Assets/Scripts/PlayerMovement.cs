using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpHeight = 1.5f; // Zıplama gücü
    public float gravity = -9.81f; // Yerçekimi

    private CharacterController controller;
    private Vector3 velocity; // Aşağı düşme hızımız
    private bool isGrounded; // Yerde miyiz?

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. ZEMİN KONTROLÜ (Yerdeysek düşme hızını sıfırla)
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. HAREKET (Sadece karakterin kendi yönüne göre)
        float horizontal = Input.GetAxis("Horizontal"); // A ve D
        float vertical = Input.GetAxis("Vertical");     // W ve S

        // transform.forward = Karakterin mavi oku (İleri)
        // transform.right = Karakterin kırmızı oku (Sağ)
        Vector3 move = (transform.forward * vertical) + (transform.right * horizontal);
        
        // Hareketi uygula
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 3. ZIPLAMA
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 4. YERÇEKİMİ UYGULAMA (Havadaysak aşağı çek)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}