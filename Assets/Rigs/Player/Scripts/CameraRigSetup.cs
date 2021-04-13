using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigSetup : MonoBehaviour {

    Camera cam;

    float yaw = 0;
    float pitch = 0;

    public float camSensitivityX = 15;
    public float camSensitivityY = 15;

    public PlayerStates playerPos;

    void Start() {
        cam = GetComponent<Camera>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void LateUpdate() {

        if (playerPos)
            transform.position = playerPos.transform.position;
        OrbitPlayer();
    }

    void OrbitPlayer() {
        float cameraX = Input.GetAxis("Mouse X");
        float cameraY = Input.GetAxis("Mouse Y");

        yaw += cameraX * camSensitivityX;
        pitch += cameraY * camSensitivityY;

        pitch = Mathf.Clamp(pitch, -20, 50);

        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }
}
