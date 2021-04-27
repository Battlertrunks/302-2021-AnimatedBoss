using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    float age = 0;
    float life = 1.5f;

    private Vector3 velocity;

    public float damageAmount = 10;
    public ParticleSystem explosion;

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
            SoundEffectBoard.Impact();
        }

        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
