using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the camera movement
/// </summary>
public class CameraRigSetup : MonoBehaviour {

    /// <summary>
    /// Gets the main camera
    /// </summary>
    Camera cam;

    /// <summary>
    /// float to move the yaw of the camera
    /// </summary>
    float yaw = 0;
    /// <summary>
    /// float to move the pitch of the camera
    /// </summary>
    float pitch = 0;

    /// <summary>
    /// Sensitvity for the X-axis of the mouse control
    /// </summary>
    public float camSensitivityX = 15;
    /// <summary>
    /// Sensitivity for the Y-axis of the mouse control
    /// </summary>
    public float camSensitivityY = 15;

    /// <summary>
    /// Sensitivity for the X-axis of the controller control
    /// </summary>
    public float camControllerSensitivityY = 10;
    /// <summary>
    /// Sensitivity for the Y-axis of the controller control
    /// </summary>
    public float camControllerSensitivityX = 10;

    /// <summary>
    /// Gets the player's position
    /// </summary>
    public PlayerStates playerPos;
    /// <summary>
    /// Gets the player's health
    /// </summary>
    Health playerHealth;

    /// <summary>
    /// Gets the transform of the player's gun
    /// </summary>
    public Transform gun;

    /// <summary>
    /// Moves camera rig to this point when the player dies
    /// </summary>
    public Transform mapPoint;
    /// <summary>
    /// Moves camera to the point
    /// </summary>
    public Transform moveCameratoWorld;

    /// <summary>
    /// If the player is using keyboard and mouse or a game pad controller
    /// </summary>
    private bool isUsingMouse = true;

    void Start() {
        cam = GetComponentInChildren<Camera>(); // Gets camera
        Cursor.visible = false; // makes cursor invisible
        Cursor.lockState = CursorLockMode.Confined; // confines cursor in the game screen

        playerHealth = playerPos.GetComponent<Health>(); // gets player health
    }

    void LateUpdate() {
        if (playerHealth.health <= 0) { // checks the player's health
            ViewMap(); // views the level when the player is dead
            return; // stops the code going below from here
        }

        if (playerPos) // Keeps the position of the camera rig on the player
            transform.position = playerPos.transform.position;

        DetectInput(); // checks inputs

        if (isUsingMouse)
            MouseAimming(); // goes to mouse controls if player is using mouse
        else
            ControllerAimming(); // goes to game pad if player is using game controller
    }

    /// <summary>
    /// Checks to see if the player is using a keyboard and mouse or a game pad
    /// </summary>
    void DetectInput() {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0) isUsingMouse = true; // checks mouse movement
        if (Input.GetAxisRaw("Aim Horizontal X") != 0 || Input.GetAxisRaw("Aim Vertical Y") != 0) isUsingMouse = false; // checks game controller input
    }

    /// <summary>
    /// Makes the camera move around the player if the controller is in use
    /// </summary>
    void ControllerAimming() {

        // Gets controller inputs as a value of float for the X and Y axis
        float h = Input.GetAxis("Aim Horizontal X");
        float v = Input.GetAxis("Aim Vertical Y");

        // sets the yaw and pitch with the input and sensitvity
        yaw += h * camControllerSensitivityX;
        pitch += v * camControllerSensitivityY;

        // clamps the pitch so the camera does not look up or down too much
        pitch = Mathf.Clamp(pitch, -20, 50);

        // rotates the camera rig to look around
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
        float gunPitch = pitch;
        // moves gun up and down when the player looks up and down
        gun.localRotation = AnimMath.Slide(gun.localRotation, Quaternion.Euler(0, -90, -gunPitch), .01f);

        // Set transform.eulerAngles (0, y, 0);
    }

    /// <summary>
    /// Makes the camera move around the player if the mouse is in use
    /// </summary>
    void MouseAimming() {
        // Gets mouse inputs as a value of float for the X and Y axis
        float cameraX = Input.GetAxis("Mouse X");
        float cameraY = Input.GetAxis("Mouse Y");
        
        // sets the yaw and pitch with the input and sensitvity
        yaw += cameraX * camSensitivityX;
        pitch += cameraY * camSensitivityY;

        // clamps the pitch so the camera does not look up or down too much
        pitch = Mathf.Clamp(pitch, -20, 50);

        // rotates the camera rig to look around
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);

        float gunPitch = pitch;
        // moves gun up and down when the player looks up and down
        gun.localRotation = AnimMath.Slide(gun.localRotation, Quaternion.Euler(0, -90, -gunPitch), .01f);
    }

    /// <summary>
    /// When the player dies, focuses on the map and spins slowly in a circle
    /// </summary>
    void ViewMap() {
        transform.position = AnimMath.Slide(transform.position, mapPoint.position, .01f); // slides camera rig to map position

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
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, yaw, 0), 0.01f); // spins camera slowly in circle
    }
}
