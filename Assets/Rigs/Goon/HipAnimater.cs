using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipAnimater : MonoBehaviour {

    float rollAmount = 5;

    Quaternion startingRot;
    GoonController goon;

    void Start() {
        startingRot = transform.localRotation;
        goon = GetComponentInParent<GoonController>();
    }


    void Update() {
        switch (goon.state) {
            case GoonController.States.Idle:
                AnimateIdle();
                break;

            case GoonController.States.Walk:
                AnimateWalk();
                break;
        }
    }

    void AnimateIdle() {
        transform.localRotation = startingRot;
    }

    void AnimateWalk() {

        float time = Time.time * goon.stepSpeed;
        float roll = Mathf.Cos(time) * rollAmount;

        transform.localRotation = Quaternion.Euler(0, 0, roll);
    }
}
