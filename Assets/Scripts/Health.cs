using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health manager for the boss and player
/// </summary>
public class Health : MonoBehaviour {

    /// <summary>
    /// health of the object
    /// </summary>
    public float health { get; private set; }

    /// <summary>
    /// max health of the object to set
    /// </summary>
    public float healthAmt = 100;

    /// <summary>
    /// Health bar of the object
    /// </summary>
    public Slider healthBarController;

    void Start() {
        health = healthAmt; // sets up health
        HealthBarSetup(); // sets up health bar
    }

    /// <summary>
    /// When the object takes damage
    /// </summary>
    /// <param name="damage"></param>
    public void TakenDamage(float damage) {
        health -= damage; // takes health away from damage
        CurrentHealth(); // updates health bar
        // play sound effect
    }

    /// <summary>
    /// Sets the health bar up on the screen
    /// </summary>
    void HealthBarSetup() {
        healthBarController.maxValue = health;
        healthBarController.value = health;
    }

    /// <summary>
    /// Updates the health bar 
    /// </summary>
    void CurrentHealth() {
        healthBarController.value = health;
    }
}
