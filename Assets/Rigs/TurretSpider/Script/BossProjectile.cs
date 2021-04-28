using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    /// <summary>
    /// How old the projectile is.
    /// </summary>
    float age = 0;

    /// <summary>
    /// the age when the projectile will be deleted.
    /// </summary>
    float life = 4;

    /// <summary>
    /// How fast the bullet will be moving.
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// How much damage the bullet will cause to the target.
    /// </summary>
    public float damageAmount = 10;

    /// <summary>
    /// The particle effect the bullet will instantiate when hitting a object
    /// </summary>
    public ParticleSystem bulletExplosion;

    void Start() {
        velocity = transform.forward * 150; // Setting the speed of the projectile.
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime; // ageing the projectile.
        if (age > life) {
            DestroyProjectile(); // when the projectile age has reached its lifetime, the projectile will be destroyed
        }

        // Making the projectile move forward.
        transform.position += velocity * Time.deltaTime;
    }

    /// <summary>
    /// Destroys the projectile.
    /// </summary>
    void DestroyProjectile() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        Health playerHealth = other.GetComponent<Health>();
        if (playerHealth) { // if the object that the projectile collided has the Health script on it.
            playerHealth.TakenDamage(10); // Damages the player.
            SoundEffectBoard.Impact(); // Sounds the impact sound.
        }

        if (other.tag == "Boss") return; // if the projectile hits the boss by accident, it will stop eveything else below from running.

        Instantiate(bulletExplosion, transform.position, transform.rotation); // spawns the particle effect
        Destroy(gameObject); // destroys projetile after collision.
    }
}
