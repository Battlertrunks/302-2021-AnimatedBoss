using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandsTarget : MonoBehaviour {

    Vector3 startingPos;
    public Transform target;

    void Start() {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update() {

        transform.position = AnimMath.Slide(transform.position, target.position, .0001f);
    }
}
