using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCapsule : MonoBehaviour
{
    [Header("플레이어 이동/회전")]
    public float speed = 5f;
    public float mouseSensitivity = 50f;

    public Transform playerCamera;

    private Rigidbody rb;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 회전 물리 막기
    }

    void FixedUpdate()
    {
        float h = 0f;
        float v = 0f;

        if (Keyboard.current.aKey.isPressed) h = -1;
        if (Keyboard.current.dKey.isPressed) h = 1;
        if (Keyboard.current.wKey.isPressed) v = 1;
        if (Keyboard.current.sKey.isPressed) v = -1;

        Vector3 move = (transform.right * h + transform.forward * v) * speed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        //if (Mouse.current.leftButton.isPressed && mousePos.x > Screen.width * 0.5f)
        if (Mouse.current.leftButton.isPressed)
        {
            float mouseX = Mouse.current.delta.x.ReadValue();
            float mouseY = Mouse.current.delta.y.ReadValue();

            // 플레이어 회전 (Y축)
            Quaternion deltaRotation = Quaternion.Euler(0f, mouseX * mouseSensitivity * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);

            // 카메라 회전 (X축)
            xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
