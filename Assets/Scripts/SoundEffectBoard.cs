using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    
/// <summary>
/// This class if for when the player, minion, and boss does sound effect actions
/// </summary>
public class SoundEffectBoard : MonoBehaviour {

    /// <summary>
    /// This is a Singleton!
    /// </summary>
    public static SoundEffectBoard main;

    /// <summary>
    /// When the player shoots
    /// </summary>
    public AudioClip shooting;

    /// <summary>
    /// Sound when Boss shoots
    /// </summary>
    public AudioClip bossShooting;

    /// <summary>
    /// Sound when the player dies
    /// </summary>
    public AudioClip deathSound;

    /// <summary>
    /// Sound when the player jumps
    /// </summary>
    public AudioClip soundJump;

    /// <summary>
    /// Sound when the boss dies
    /// </summary>
    public AudioClip bossDeath;

    /// <summary>
    /// Sound when the boss is shooting
    /// </summary>
    public AudioClip soundBeingHit;

    /// <summary>
    /// Creating audio source to play sounds on
    /// </summary>
    private AudioSource player;

    // Start is called before the first frame update
    void Start() {

        if (main == null) {
            main = this;
            player = GetComponent<AudioSource>();
        } else {
            Destroy(this.gameObject);
        }

    }

    /// <summary>
    /// Plays when the player shoots
    /// </summary>
    public static void PlayerShooting() {
        main.player.PlayOneShot(main.shooting);
    }

    /// <summary>
    /// Sound when the boss shoots
    /// </summary>
    public static void BossShooting() {
        main.player.PlayOneShot(main.bossShooting);
    }

    /// <summary>
    /// Plays the sound when the player dies
    /// </summary>
    public static void PlayerDeath() {
        main.player.PlayOneShot(main.deathSound);
    }

    /// <summary>
    /// Plays when the player is jumping
    /// </summary>
    public static void JumpSound() {
        main.player.PlayOneShot(main.soundJump);
    }

    /// <summary>
    /// Plays when the boss dies
    /// </summary>
    public static void BossDeathSound() {
        main.player.PlayOneShot(main.bossDeath);
    }

    /// <summary>
    /// Sound when projectiles hit a target
    /// </summary>
    public static void Impact() {
        main.player.PlayOneShot(main.soundBeingHit);
    }
}
