using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        BossStateMachine bossHealth = other.GetComponent<BossStateMachine>();
        if (bossHealth) {
            bossHealth.health -= 10;
        }
    }
}
