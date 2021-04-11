using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyFoot : MonoBehaviour {

    public Transform stepPosition;

    public AnimationCurve verticleStepMovement;

    private Vector3 previousPlantedPosition;
    private Quaternion previousPlantedRotation = Quaternion.identity;

    private Vector3 plantedPosition;
    private Quaternion plantedRotation = Quaternion.identity;

    private float timeLength = .25f;
    private float timeCurrent = 0;

    void Start() {
        
    }


    void Update() {
        if (CheckIfCanStep()) {
            DoRayCast();
        }

        if (timeCurrent < timeLength) { // animation is playing...

            timeCurrent += Time.deltaTime; // move playhead forward

            float p = timeCurrent / timeLength;

            plantedPosition.y = Mathf.Clamp(plantedPosition.y, 2, 100);

            Vector3 finalPosition = AnimMath.Lerp(previousPlantedPosition, plantedPosition, p);
            //transform.rotation = AnimMath.Lerp(previousPlantedRotation, plantedRotation, p);

            finalPosition.y += verticleStepMovement.Evaluate(p);

            transform.position = finalPosition;

        } else { // animation is NOT playing:
            plantedPosition.y = Mathf.Clamp(plantedPosition.y, 2, 100);
            transform.position = plantedPosition;
            FindGroundBumps();
            //transform.rotation = plantedRotation;
        }


    }

    bool CheckIfCanStep() {

        Vector3 vBetween = transform.position - stepPosition.position;
        float threshold = Random.Range(4.0f, 7.0f);

        return (vBetween.sqrMagnitude > threshold * threshold);
    }

    void DoRayCast() {

        Ray ray = new Ray(stepPosition.position + Vector3.up, Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 3);

        if (Physics.Raycast(ray, out RaycastHit hit, 3)) {

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

    void FindGroundBumps() {
        Ray bumpRay = new Ray(transform.position + new Vector3(0, 2f, 0), Vector3.down * 2);
        Debug.DrawRay(bumpRay.origin, bumpRay.direction);

        if (Physics.Raycast(bumpRay, out RaycastHit hit)) {
            plantedPosition.y = hit.point.y + 2;
        }
    }
}
