using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterXSeconds : MonoBehaviour {

    void Start() {
        Destroy(gameObject, 2);
    }
}
