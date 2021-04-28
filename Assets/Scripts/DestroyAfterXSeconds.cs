using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys objects in certain seconds
/// </summary>
public class DestroyAfterXSeconds : MonoBehaviour {

    void Start() {
        Destroy(gameObject, 2); // destroys object
    }
}
