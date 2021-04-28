using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes player hands IK follow a target
/// </summary>
public class PlayerHandsTarget : MonoBehaviour {

    /// <summary>
    /// Target for the hands to go to
    /// </summary>
    public Transform target;


    void Update() {
        transform.position = AnimMath.Slide(transform.position, target.position, .0001f); // moves hands to target
    }
}
