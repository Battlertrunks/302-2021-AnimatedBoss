using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the boss's feet movement
/// </summary>
public class StickyFoot : MonoBehaviour {

    /// <summary>
    /// Makes boss step in this transform for foot
    /// </summary>
    public Transform stepPosition;

    /// <summary>
    /// Makes boss raise foot
    /// </summary>
    public AnimationCurve verticleStepMovement;

    /// <summary>
    /// when the boss should mvoe the foot
    /// </summary>
    public static float moveThreshold = 2;

    /// <summary>
    /// Rotation of the foot
    /// </summary>
    private Quaternion startingRot;

    /// <summary>
    /// the previous planted position of the foot
    /// </summary>
    private Vector3 previousPlantedPosition;
    /// <summary>
    /// Previous planted rotation of the foot
    /// </summary>
    private Quaternion previousPlantedRotation = Quaternion.identity;

    /// <summary>
    /// Planted position of the foot
    /// </summary>
    private Vector3 plantedPosition;
    /// <summary>
    /// Planted position of the foot
    /// </summary>
    private Quaternion plantedRotation = Quaternion.identity;

    /// <summary>
    /// Animation length
    /// </summary>
    private float timeLength = .25f;

    /// <summary>
    /// Animation's current time
    /// </summary>
    private float timeCurrent = 0;

    /// <summary>
    /// How tall is the foot
    /// </summary>
    public float footHeight = 0;

    /// <summary>
    /// The knee of the feet 
    /// </summary>
    Transform kneePole;

    /// <summary>
    /// Is animating or not
    /// </summary>
    public bool isAnimating {
        get {
            return (timeCurrent < timeLength);
        }
    }

    /// <summary>
    /// If foot has moved or not
    /// </summary>
    public bool footHasMoved = false;

    void Start() {
        kneePole = transform.GetChild(0); // gets the knee pole from child
        startingRot = transform.localRotation; // gets starting rotation
        TryToMove(true);
    }


    void Update() {
       /* if (CheckIfCanStep()) {
            DoRayCast();
        }

        if (timeCurrent < timeLength) {*/

        if (isAnimating) { // if animating 

            timeCurrent += Time.deltaTime; // move playhead forward

            float p = timeCurrent / timeLength;

            plantedPosition.y = Mathf.Clamp(plantedPosition.y, footHeight, 100); // clamps the y position

            Vector3 finalPosition = AnimMath.Lerp(previousPlantedPosition, plantedPosition, p);
            //transform.rotation = AnimMath.Lerp(previousPlantedRotation, plantedRotation, p);

            finalPosition.y += verticleStepMovement.Evaluate(p);
            transform.position = finalPosition;

            // caculate to move to position
            Vector3 vFromCenter = transform.position - transform.parent.position;
            vFromCenter.y = 0;
            vFromCenter.Normalize();
            vFromCenter *= 3;
            vFromCenter.y += 2.5f;

            kneePole.position = vFromCenter + transform.position;


        } else { // animation is NOT playing:
            plantedPosition.y = Mathf.Clamp(plantedPosition.y, footHeight, 100);
            transform.position = plantedPosition;
            FindGroundBumps(); // finds bumps in ground
            //transform.rotation = plantedRotation;
        }
    }

    /// <summary>
    /// Sets the new position to go to
    /// </summary>
    /// <param name="forceRaycast"></param>
    /// <returns></returns>
    public bool TryToMove(bool forceRaycast = false) {
        // if animating, don't try to step:
        if (isAnimating && !forceRaycast) return false;

        if (footHasMoved && !forceRaycast) return false;

        Vector3 vBetween = transform.position - stepPosition.position;

        // if too close to previous target, don't try to step:
        if (vBetween.sqrMagnitude < moveThreshold * moveThreshold && !forceRaycast) return false;


        Ray ray = new Ray(stepPosition.position + new Vector3(0, 3, 0), Vector3.down); // sets raycast up

        Debug.DrawRay(ray.origin, ray.direction * 3);

        if (Physics.Raycast(ray, out RaycastHit hit, 7)) {

            // setup beginning of animation:
            previousPlantedPosition = transform.position;
            previousPlantedRotation = transform.rotation;

            // set starting rotation
            transform.localRotation = startingRot;

            // setup end of animation:
            plantedPosition = hit.point; // moves foot to raycast position that was hit
            plantedRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation; // rotates foot based on rotation


            // Begin animation:
            timeCurrent = 0;

            footHasMoved = true;

            return true;
        }
        return false;
    }

    /*
    bool CheckIfCanStep() {

        Vector3 vBetween = transform.position - stepPosition.position;
        float threshold = Random.Range(4.0f, 7.0f);
        
        return (vBetween.sqrMagnitude > threshold * threshold);
    }

    void DoRayCast() {

        Ray ray = new Ray(stepPosition.position + new Vector3(0, 3, 0), Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 3);

        if (Physics.Raycast(ray, out RaycastHit hit, 7)) {

            // setup beginning of animation:
            previousPlantedPosition = transform.position;
            previousPlantedRotation = transform.rotation;

            // setup end of animation:
            plantedPosition = hit.point;
            plantedRotation = Quaternion.FromToRotation(transform.up, hit.normal);

            // Begin animation:
            timeCurrent = 0;
        }

    }

    */

    /// <summary>
    /// Finds bumps on the ground below the foot
    /// </summary>
    void FindGroundBumps() {
        Ray bumpRay = new Ray(transform.position + new Vector3(0, 2f, 0), Vector3.down * 2); // creates ray
        Debug.DrawRay(bumpRay.origin, bumpRay.direction);

        if (Physics.Raycast(bumpRay, out RaycastHit hit)) {
            plantedPosition.y = hit.point.y + footHeight; // moves foot to ray hit position
        }
    }
}
