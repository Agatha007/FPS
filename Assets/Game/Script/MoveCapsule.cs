using System.Collections;
using System.Threading;
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

    float _rotationX; // 수직 회전 (카메라)
    float _rotationY; // 수평 회전 (플레이어 몸체)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 회전 물리 막기

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        Vector3 newPos = rb.position + move * Time.fixedDeltaTime;

        rb.MovePosition(newPos);

        Quaternion targetRotation = Quaternion.Euler(0f, _rotationY, 0f);
        rb.MoveRotation(targetRotation);
    }

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        _rotationY += mouseDelta.x * mouseSensitivity * 0.1f;
        _rotationX -= mouseDelta.y * mouseSensitivity * 0.1f;
        _rotationX = Mathf.Clamp(_rotationX, -80f, 80f);

        playerCamera.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
    }
}
