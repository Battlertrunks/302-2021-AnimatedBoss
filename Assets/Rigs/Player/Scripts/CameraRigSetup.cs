using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigSetup : MonoBehaviour {

    Camera cam;

    float yaw = 0;
    float pitch = 0;

    public float camSensitivityX = 15;
    public float camSensitivityY = 15;

    public float camControllerSensitivityY = 10;
    public float camControllerSensitivityX = 10;

    public PlayerStates playerPos;
    Health playerHealth;

    public Transform gun;

    public Transform mapPoint;
    public Transform moveCameratoWorld;

    private bool isUsingMouse = true;

    void Start() {
        cam = GetComponentInChildren<Camera>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        playerHealth = playerPos.GetComponent<Health>();
    }

    void LateUpdate() {
        if (playerHealth.health <= 0) {
            ViewMap();
            return;
        }

        if (playerPos)
            transform.position = playerPos.transform.position;

        DetectInput();

        if (isUsingMouse)
            MouseAimming();
        else
            ControllerAimming();
    }

    void DetectInput() {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0) isUsingMouse = true;
        if (Input.GetAxisRaw("Aim Horizontal X") != 0 || Input.GetAxisRaw("Aim Vertical Y") != 0) isUsingMouse = false;
    }

    void ControllerAimming() {

        float h = Input.GetAxis("Aim Horizontal X");
        float v = Input.GetAxis("Aim Vertical Y");

        yaw += h * camControllerSensitivityX;
        pitch += v * camControllerSensitivityY;

        pitch = Mathf.Clamp(pitch, -20, 50);


        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
        float gunPitch = pitch;
        gun.localRotation = AnimMath.Slide(gun.localRotation, Quaternion.Euler(0, -90, -gunPitch), .01f);

        // Set transform.eulerAngles (0, y, 0);
    }

    void MouseAimming() {
        float cameraX = Input.GetAxis("Mouse X");
        float cameraY = Input.GetAxis("Mouse Y");

        yaw += cameraX * camSensitivityX;
        pitch += cameraY * camSensitivityY;

        pitch = Mathf.Clamp(pitch, -20, 50);

        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);

        float gunPitch = pitch;
        gun.localRotation = AnimMath.Slide(gun.localRotation, Quaternion.Euler(0, -90, -gunPitch), .01f);
    }

    void ViewMap() {
        transform.position = AnimMath.Slide(transform.position, mapPoint.position, .01f);

        // Moves camera itself farther away from the camer rig
        cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, moveCameratoWorld.localPosition, .001f);
        cam.transform.localRotation = AnimMath.Slide(cam.transform.localRotation, moveCameratoWorld.localRotation, .001f);

        // rotates camera rig when the rig is 'leveled'
        //if (transform.eulerAngles.x <= 0.5f && transform.eulerAngles.x >= -0.5f && transform.eulerAngles.z <= 0.5f && transform.eulerAngles.z >= -0.5f) {
        //    //transform.rotation = AnimMath.Slide(transform.rotation, transform.rotation * Quaternion.Euler(0, 5, 0), 0.01f);
        //    print("true");
        //    transform.Rotate(0, spinYaw, 0);
        //    return;
        //}

        // Levels the rig
        yaw += 10 * Time.deltaTime;
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, yaw, 0), 0.01f);
    }
}
