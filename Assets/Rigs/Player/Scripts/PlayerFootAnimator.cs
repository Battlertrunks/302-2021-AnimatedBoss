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

    private Vector3 targetPos;
    private Quaternion targetRot;

    public Collider ground;

    void Start() {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        player = GetComponentInParent<PlayerStates>();
    }

    void Update() {


        //AnimateIdle();



        AnimateWalk();


        // ease position and rotation towards their targets:
        //transform.position = AnimMath.Slide(transform.position, targetPos, .01f);
        //transform.rotation = AnimMath.Slide(transform.rotation, targetRot, .01f);
    }

    void AnimateWalk() {

        Vector3 finalPos = startingPos;
        float time = (Time.time + stepOffset) * player.steppingSpeed;

        // math
        // Lateral movement (z + x)
        float frontToBack = Mathf.Sin(time);
        Vector3 localMove = transform.InverseTransformDirection(player.move);
        finalPos += localMove * frontToBack * player.walkScale.z;

        // verticle movement (y)
        finalPos.y += Mathf.Cos(time) * player.walkScale.y;
        //finalPos.x *= goon.walkScale.x;

        bool isOnGround =  (finalPos.y < startingPos.y);

        if (isOnGround) finalPos.y = finalPos.y = startingPos.y;

        // convert from z (-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(frontToBack);

        float anklePitch = isOnGround ? 0 : -p * 20;

        transform.localPosition = finalPos;
        transform.localRotation = startingRot * Quaternion.Euler(0, 0, anklePitch);

        FindGround();

        //targetPos = transform.TransformPoint(finalPos);
        //targetRot = transform.parent.rotation * Quaternion.Euler(0, 0, anklePitch);

    }

    void AnimateIdle() {
        transform.localPosition = startingPos;
        transform.localRotation = startingRot;

        //targetPos = transform.TransformPoint(startingPos);
        //targetRot = transform.parent.rotation * startingRot;
        FindGround();
    }

    void FindGround() {

        Ray ray = new Ray(transform.position + new Vector3(0, 3f, 0), Vector3.down);
        Debug.DrawRay(transform.position + new Vector3(0, 1.5f, 0), Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.tag == "Ground") {

            transform.position = hit.point;
            //targetPos = hit.point;
            
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            //targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        } else {

        }

    }
}
