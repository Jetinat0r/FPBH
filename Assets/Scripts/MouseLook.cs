using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity = 100f;
    [SerializeField]
    private float lookRestraint = 85f;

    [SerializeField]
    private Transform playerBody;

    private float xRotation = 0f;
    private bool isCursorLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
            float _mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
            float _mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= _mouseY;
            xRotation = Mathf.Clamp(xRotation, -lookRestraint, lookRestraint);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * _mouseX);
    }
}
