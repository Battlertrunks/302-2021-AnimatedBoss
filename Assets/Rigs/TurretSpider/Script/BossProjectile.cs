using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{

    float age = 0;
    float life = 4;

    private Vector3 velocity;

    public float damageAmount = 10;
    public ParticleSystem bulletExplosion;

    void Start() {
        velocity = transform.forward * 150;
    }

    // Update is called once per frame
    void Update()
    {
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
        Health playerHealth = other.GetComponent<Health>();
        if (playerHealth) {
            playerHealth.TakenDamage(10);
            SoundEffectBoard.Impact();
        }

        if (other.tag == "Boss") return;

        Instantiate(bulletExplosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
