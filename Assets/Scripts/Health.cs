using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    public float health { get; private set; }
    public float healthAmt = 100;

    public Slider healthBarController;

    void Start() {
        health = healthAmt;
        HealthBarSetup();
    }


    public void TakenDamage(float damage) {
        health -= damage;
        CurrentHealth();
        // play sound effect
    }

    void HealthBarSetup() {
        healthBarController.maxValue = health;
        healthBarController.value = health;
    }

    void CurrentHealth() {
        healthBarController.value = health;
    }
}
