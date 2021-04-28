using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// The states the player are in while the player is playing
/// </summary>
public class PlayerStates : MonoBehaviour {

    /// <summary>
    /// Sets up the State Machine.
    /// </summary>
    static class States {
        public class State {

            /// <summary>
            /// The way to accesss everything outside the state machine.
            /// </summary>
            protected PlayerStates playerState;

            virtual public State Update() {
                return null;
            }

            /// <summary>
            /// When the state starts
            /// </summary>
            /// <param name="playerState"></param>
            virtual public void OnStart(PlayerStates playerState) {
                this.playerState = playerState;
            }

            virtual public void OnEnd() {

            }
        }

        //// Child Classes:

        
        /// <summary>
        /// The idle state of the player
        /// </summary>
        public class Idle : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk(10); // for when the player wants to jump
                playerState.IdleAnimation(); // plays the idle animation
                playerState.currentState = 0; // cureent state for he PlayerFootAnimator

                // transition:

                // When the player presses movement inputs
                if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d") || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                    return new States.Walking();

                // When the player presses the fire button
                if (Input.GetButton("Fire1") && playerState.rateOfFire <= 0)
                    return new States.ShootingAttack();

                // When the player dies
                if (playerState.playerHealth.health <= 0) {
                    SoundEffectBoard.PlayerDeath(); // plays death sound
                    return new States.DeathAnim();
                }

                return null;
            }
        }

        /// <summary>
        /// State when the player is walking 
        /// </summary>
        public class Walking : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk(playerState.playerWalkSpeed); // goes to the function and sends the walking value
                playerState.PlayerWalkWaddleAnim(); // plays the walking animation
                playerState.currentState = 1; // sends current state for the PlayerFootAnimator
                playerState.hipRing.localPosition = Vector3.zero; // sets the hip ring position to 0

                // transition:
                if (Input.GetButton("Fire3")) {
                    playerState.setStepSpeed = playerState.runningStepingSpeed; // Makes player run
                    return new States.Running();
                }

                // When player is not pressing movement input
                if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d") && (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0))
                    return new States.Idle();

                // when player fires their gun
                if (Input.GetButton("Fire1") && playerState.rateOfFire <= 0)
                    return new States.ShootingAttack();

                // When the player dies
                if (playerState.playerHealth.health <= 0) {
                    SoundEffectBoard.PlayerDeath();
                    return new States.DeathAnim();
                }

                return null;
            }
        }

        /// <summary>
        /// State when the player is running
        /// </summary>
        public class Running : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk(playerState.playerWalkSpeed + 7); // function to make the player run
                playerState.PlayerRunningWaddleAnim(); // running animation
                playerState.hipRing.localPosition = Vector3.zero; // zeros out the hip ring position
                // transitions:

                // when player is not pressing any movement input keys
                if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d") && (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)) {
                    playerState.setStepSpeed = playerState.steppingSpeed;
                    return new States.Idle();
                }

                // When player is not holding the running button
                if (!Input.GetButton("Fire3")) {
                    playerState.setStepSpeed = playerState.steppingSpeed;
                    return new States.Walking();
                }

                // When player dies
                if (playerState.playerHealth.health <= 0) {
                    SoundEffectBoard.PlayerDeath();
                    return new States.DeathAnim();
                }

                return null;
            }
        }

        /// <summary>
        /// State the player is shooting their gun
        /// </summary>
        public class ShootingAttack : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerGun(); // function the gun is shooting

                // transition:
                
                // if the player is pressing the movement inputs
                if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d") || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                    return new States.Walking();
                }

                return new States.Idle(); // returns to the idle state
            }
        }

        /// <summary>
        /// State the player is dead and play animation
        /// </summary>
        public class DeathAnim : State {
            public override State Update() {
                // behaviour
                playerState.DeathAnimation(); // plays the death animation
                playerState.currentState = 2;

                return null;
            }
        }

    }

    /// <summary>
    /// Able to change the state the player is in
    /// </summary>
    private States.State state;

    /// <summary>
    /// Gets player controller to move and jump
    /// </summary>
    CharacterController player;

    /// <summary>
    /// Gets camera
    /// </summary>
    private Camera cam;

    [Header("Player movement variables:")]
    [HideInInspector] public float verticalVelocity = 0;

    /// <summary>
    /// Setting the gravity
    /// </summary>
    public float gravityMultiplier = 10;

    /// <summary>
    /// How fast the player can walk
    /// </summary>
    public float playerWalkSpeed = 10;

    /// <summary>
    /// When the player can jump again
    /// </summary>
    private float timeLeftOnGround = 0;

    /// <summary>
    /// The jump force of the player
    /// </summary>
    public float jumpImpulse = 10;

    /// <summary>
    /// Step speed of the animation
    /// </summary>
    public float steppingSpeed = 5;

    /// <summary>
    /// Running step speed of the animation
    /// </summary>
    public float runningStepingSpeed = 17;

    /// <summary>
    /// The variable to set the step speed
    /// </summary>
    [HideInInspector] public float setStepSpeed;

    /// <summary>
    /// How big the walk scale is
    /// </summary>
    public Vector3 walkScale = Vector3.one;

    /// <summary>
    /// Player's movement
    /// </summary>
    public Vector3 move { get; private set; }

    [Header("Player weapon variables:")]
    /// <summary>
    /// The muzzle of the gun
    /// </summary>
    public Transform muzzle;

    /// <summary>
    /// The gun itself
    /// </summary>
    public Transform gunModel;

    /// <summary>
    /// The guns location
    /// </summary>
    private Vector3 gunStartingLocation;

    /// <summary>
    /// Rounds fired per second
    /// </summary>
    public float roundsPerSecond = 10f;

    /// <summary>
    /// Rate of fire of the gun
    /// </summary>
    private float rateOfFire = 0;

    /// <summary>
    /// The projectile of the bullet to spawn
    /// </summary>
    public PlayerProjectile bullet;

    /// <summary>
    /// The player's rig
    /// </summary>
    public Transform playerRig;

    /// <summary>
    /// Getting all rigidbodies of the player to make a ragdoll effect
    /// </summary>
    Rigidbody[] ragDoll;

    /// <summary>
    /// makes the walking animation when the PlayerFootAnimator
    /// </summary>
    public GameObject walkingAnim;

    /// <summary>
    /// Current state of the player
    /// </summary>
    public int currentState = 0;

    /// <summary>
    /// Hip ring to make animations on the player
    /// </summary>
    public Transform hipRing;

    /// <summary>
    /// Turns off rigbuilder on rig to make ragdoll effect
    /// </summary>
    RigBuilder letBonesGo;

    /// <summary>
    /// turns on all colliders on the player when the player dies
    /// </summary>
    Collider[] turningOnRagdollColliders;

    /// <summary>
    /// Gets the health of the player
    /// </summary>
    private Health playerHealth;

    /// <summary>
    /// Checks if player is grounded their time on the ground
    /// </summary>
    public bool isGrounded {
        get {
            return player.isGrounded || timeLeftOnGround > 0;
        }
    }

    void Start() {
        player = GetComponent<CharacterController>();
        cam = Camera.main;
        ragDoll = GetComponentsInChildren<Rigidbody>();
        setStepSpeed = steppingSpeed;
        gunStartingLocation = gunModel.localPosition;

        letBonesGo = GetComponentInChildren<RigBuilder>();
        turningOnRagdollColliders = GetComponentsInChildren<Collider>();
        playerHealth = GetComponent<Health>();
    }


    void Update() {
        if (state == null) StateSwitcher(new States.Idle()); // sets idle if the state is null
        if (state != null) StateSwitcher(state.Update()); // if is not null runs the state update

        if (timeLeftOnGround > 0) timeLeftOnGround -= Time.deltaTime; // counts down 
        if (rateOfFire > 0) rateOfFire -= Time.deltaTime; // counts down

        AttackAnimation(); // is a recoil animation that puts the gun back to its starting rotation when the player shoots
    }

    /// <summary>
    /// Sets the state for the player
    /// </summary>
    /// <param name="switchState"></param>
    void StateSwitcher(States.State switchState) {
        if (switchState == null) return;
        if (state != null) state.OnEnd();

        state = switchState;
        state.OnStart(this);
        
    }

    /// <summary>
    /// When the player is not moving plays the idle animation
    /// </summary>
    void IdleAnimation() {
        hipRing.localPosition = Vector3.down * 0.005f * Mathf.Sin(Time.time); // moves the hip ring up and down
        hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.identity, .001f); // rotates the hip rotation
    }

    /// <summary>
    /// Waddle animation when the player walks
    /// </summary>
    void PlayerWalkWaddleAnim() {
        if (player.isGrounded) // rotates the hip around to give a walking feel
            hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.Euler(0, 20f * Mathf.Sin(Time.time * 3), 90f * Mathf.Cos(Time.time * 4)), .001f);
        if (!player.isGrounded) // rotates back to original rotation when not grounded
            hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.identity, .001f);
    }

    /// <summary>
    /// Runnning waddle animation when the player is running
    /// </summary>
    void PlayerRunningWaddleAnim() {
        if (player.isGrounded) // rotates the hip when running
            hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.Euler(165, 1f * Mathf.Sin(Time.time * 3), 75f * Mathf.Cos(Time.time * 6)), .001f);
        if (!player.isGrounded) // rotates back to original rotation when not grounded
            hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.identity, .001f);
    }

    /// <summary>
    /// Moves the player 
    /// </summary>
    /// <param name="walkSpeed"></param>
    void PlayerWalk(float walkSpeed = 10) {
        // Gets player input and stores in floats
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool heldJump = Input.GetButton("Jump"); // gets jump input

        // moves the player based on the yaw of the camera
        float cameraYaw = cam.transform.eulerAngles.y;
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, cameraYaw, 0), .01f);

        // sets the direction of movement for the player
        move = transform.right * h + v * transform.forward;
        if (move.sqrMagnitude > 1) move.Normalize();

        // sets the gravity
        verticalVelocity += gravityMultiplier * Time.deltaTime;

        // puts it all together
        Vector3 moveDeltaWithGravity = move * walkSpeed + verticalVelocity * Vector3.down;

        
        player.Move(moveDeltaWithGravity * Time.deltaTime); // move the character controller

        if (player.isGrounded) { // if player is touching the ground
            verticalVelocity = 0f; // sets gravity
            timeLeftOnGround = .2f;
        }

        if (isGrounded && heldJump) { // if grounded and pressed jump
            verticalVelocity -= 10; // makes player jump
            timeLeftOnGround = 0;
            SoundEffectBoard.JumpSound(); // jump sound
        }
    }

    /// <summary>
    /// When player is shooting the gun
    /// </summary>
    void PlayerGun() {
        // Spawns the bullet
        Instantiate(bullet, muzzle.position, muzzle.rotation);

        // caculates rate of fire
        rateOfFire = 1 / roundsPerSecond;
        SoundEffectBoard.PlayerShooting(); // plays the gun sound

        // makes the recoil effect
        gunModel.localPosition = AnimMath.Slide(gunModel.localPosition, gunModel.localPosition - (Vector3.forward * 6), .0001f);
    }

    /// <summary>
    /// Does the recoil animation to place the gun back to its starting position
    /// </summary>
    void AttackAnimation() {
        gunModel.localPosition = AnimMath.Slide(gunModel.localPosition, gunStartingLocation, .001f);
    }

    /// <summary>
    /// Player's death animation
    /// </summary>
    void DeathAnimation() {
        playerRig.parent = null; // taking off the rig from parent
        gunModel.gameObject.SetActive(false); // turns gun off
        player.enabled = false; // turn character controller off
        walkingAnim.SetActive(false); // walkingAnim is off

        foreach (Rigidbody ragdollAnim in ragDoll) { // sets gravity on and kinematic off
            ragdollAnim.useGravity = true;
            ragdollAnim.isKinematic = false;
        }
        foreach (Collider collider in turningOnRagdollColliders) {
            collider.isTrigger = false; // makes colliders not triggers anymore
        }

        letBonesGo.enabled = false; // stops the deathAnimation from looping
    }
}
