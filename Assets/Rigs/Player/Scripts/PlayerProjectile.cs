using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    /// <summary>
    /// age of the projectile.
    /// </summary>
    float age = 0;
    /// <summary>
    /// the age when the projectile will be deleted.
    /// </summary>
    float life = 1.5f;

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
    public ParticleSystem explosion;

    void Start() {
        velocity = transform.forward * 40; // Setting the speed of the projectile.
    }

    // Update is called once per frame
    void Update() {
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
        Health bossHealthScript = other.GetComponentInParent<Health>(); 
        if (bossHealthScript) { // if the object that the projectile collided has the Health script on it.
            bossHealthScript.TakenDamage(10); // Damages the player.
            SoundEffectBoard.Impact(); // Sounds the impact sound.
        }

        Instantiate(explosion, transform.position, transform.rotation); // spawns the particle effect
        Destroy(gameObject); // destroys projetile after collision.
    }
}
