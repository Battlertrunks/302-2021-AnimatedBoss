using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyFoot : MonoBehaviour {

    public Transform stepPosition;

    public AnimationCurve verticleStepMovement;

    public static float moveThreshold = 3;

    private Quaternion startingRot;

    private Vector3 previousPlantedPosition;
    private Quaternion previousPlantedRotation = Quaternion.identity;

    private Vector3 plantedPosition;
    private Quaternion plantedRotation = Quaternion.identity;

    private float timeLength = .25f;
    private float timeCurrent = 0;

    public float footHeight = 0;

    Transform kneePole;

    public bool isAnimating {
        get {
            return (timeCurrent < timeLength);
        }
    }

    public bool footHasMoved = false;

    void Start() {
        kneePole = transform.GetChild(0);
        startingRot = transform.localRotation;
        TryToMove(true);
    }


    void Update() {
       /* if (CheckIfCanStep()) {
            DoRayCast();
        }

        if (timeCurrent < timeLength) {*/

        if (isAnimating) {

            timeCurrent += Time.deltaTime; // move playhead forward

            float p = timeCurrent / timeLength;

            plantedPosition.y = Mathf.Clamp(plantedPosition.y, footHeight, 100);

            Vector3 finalPosition = AnimMath.Lerp(previousPlantedPosition, plantedPosition, p);
            //transform.rotation = AnimMath.Lerp(previousPlantedRotation, plantedRotation, p);

            finalPosition.y += verticleStepMovement.Evaluate(p);
            transform.position = finalPosition;

            Vector3 vFromCenter = transform.position - transform.parent.position;
            vFromCenter.y = 0;
            vFromCenter.Normalize();
            vFromCenter *= 3;
            vFromCenter.y += 2.5f;

            kneePole.position = vFromCenter + transform.position;


        } else { // animation is NOT playing:
            plantedPosition.y = Mathf.Clamp(plantedPosition.y, footHeight, 100);
            transform.position = plantedPosition;
            FindGroundBumps();
            //transform.rotation = plantedRotation;
        }
    }

    public bool TryToMove(bool forceRaycast = false) {
        // if animating, don't try to step:
        if (isAnimating && !forceRaycast) return false;

        if (footHasMoved && !forceRaycast) return false;

        Vector3 vBetween = transform.position - stepPosition.position;

        // if too close to previous target, don't try to step:
        if (vBetween.sqrMagnitude < moveThreshold * moveThreshold && !forceRaycast) return false;


        Ray ray = new Ray(stepPosition.position + new Vector3(0, 3, 0), Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 3);

        if (Physics.Raycast(ray, out RaycastHit hit, 7)) {

            // setup beginning of animation:
            previousPlantedPosition = transform.position;
            previousPlantedRotation = transform.rotation;

            // set starting rotation
            transform.localRotation = startingRot;

            // setup end of animation:
            plantedPosition = hit.point;
            plantedRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;


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

    void FindGroundBumps() {
        Ray bumpRay = new Ray(transform.position + new Vector3(0, 2f, 0), Vector3.down * 2);
        Debug.DrawRay(bumpRay.origin, bumpRay.direction);

        if (Physics.Raycast(bumpRay, out RaycastHit hit)) {
            plantedPosition.y = hit.point.y + footHeight;
        }
    }
}
