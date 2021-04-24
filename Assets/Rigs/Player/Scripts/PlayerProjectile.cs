using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    float age = 0;
    float life = 4;

    private Vector3 velocity;

    public float damageAmount = 10;

    void Start() {
        velocity = transform.forward * 40;
    }

    // Update is called once per frame
    void Update() {
        age += Time.deltaTime;
        if (age > life) {
            DestroyProjectile();
        }

        transform.position += velocity * Time.deltaTime;
    }

    void DestroyProjectile() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        Health bossHealthScript = other.GetComponentInParent<Health>();
        if (bossHealthScript) {
            bossHealthScript.TakenDamage(10);
        }

        Destroy(gameObject);
    }
}
