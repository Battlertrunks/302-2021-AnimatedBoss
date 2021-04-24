using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script animates the foot / legs by changing the local position of this object (IK target).
/// </summary>
public class PlayerFootAnimator : MonoBehaviour {

    /// <summary>
    /// The local-space starting position of this object
    /// </summary>
    private Vector3 startingPos;

    /// <summary>
    /// The local=space starting rotation of this object
    /// </summary>
    private Quaternion startingRot;

    /// <summary>
    /// An offset value to use for the timing Sin wave that controls the walk animation. Measured in radians.
    /// 
    /// A value of Math.PI would be half-a-period.
    /// </summary>
    public float stepOffset = 0;

    public Transform playerRot;

    PlayerStates player;

    CharacterController playerMovement;

    private Vector3 targetPos;
    private Quaternion targetRot;

    public Collider ground;

    bool goToStartLocation = true;

    void Start() {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        player = GetComponentInParent<PlayerStates>();
        playerMovement = GetComponentInParent<CharacterController>();
    }

    void Update() {

        if (!playerMovement.isGrounded) {
            AnimateJump();
            return;
        }

        //AnimateIdle();

        if (player.currentState == 0) AnimateIdle();

        if (player.currentState == 1) AnimateWalk();


        // ease position and rotation towards their targets:
        //transform.position = AnimMath.Slide(transform.position, targetPos, .01f);
        //transform.rotation = AnimMath.Slide(transform.rotation, targetRot, .01f);
    }

    void AnimateJump() {
        if (transform.localPosition.y <= 0) {
            transform.localPosition = AnimMath.Slide(transform.localPosition, transform.localPosition + (Vector3.up * 1f), .1f);
            goToStartLocation = true;
        }
    }

    void AnimateWalk() {

        Vector3 finalPos = startingPos;
        float time = (Time.time + stepOffset) * player.setStepSpeed;

        // math
        // Lateral movement (z + x)
        float frontToBack = Mathf.Sin(time);
        Vector3 localMove = transform.InverseTransformDirection(player.move);
        finalPos += localMove * frontToBack * player.walkScale.z / 2;

        // verticle movement (y)
        finalPos.y += Mathf.Cos(time) * player.walkScale.y / 1.5f;
        //finalPos.x *= goon.walkScale.x;

        bool isOnGround =  (finalPos.y < startingPos.y);

        if (isOnGround) finalPos.y = finalPos.y = startingPos.y;

        // convert from z (-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(frontToBack);

        float anklePitch = isOnGround ? 0 : -p * 20;

        transform.localPosition = finalPos;
        transform.localRotation = startingRot * Quaternion.Euler(0, 0, anklePitch);

        goToStartLocation = true;
        //FindGround();

        //targetPos = transform.TransformPoint(finalPos);
        //targetRot = transform.parent.rotation * Quaternion.Euler(0, 0, anklePitch);

    }

    void AnimateIdle() {
        if (goToStartLocation) {
            transform.localPosition = AnimMath.Slide(transform.localPosition, startingPos, .1f);
            if (transform.localPosition == startingPos)
                goToStartLocation = false;
        }
        transform.localRotation = startingRot;

        //targetPos = transform.TransformPoint(startingPos);
        //targetRot = transform.parent.rotation * startingRot;
        FindGround();
    }

    void FindGround() {

        Ray ray = new Ray(transform.position + new Vector3(0, 0.5f, 0), Vector3.down * 3f);

        Debug.DrawRay(ray.origin, ray.direction);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.tag == "Ground") {

            transform.position = AnimMath.Slide(transform.position, hit.point +  new Vector3(0, .16f, 0), .001f);
            //targetPos = hit.point;
            
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            //targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        } else {
        }

    }
}
