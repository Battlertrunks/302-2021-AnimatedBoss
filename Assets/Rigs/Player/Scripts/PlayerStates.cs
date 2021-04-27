using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerStates : MonoBehaviour {

    static class States {
        public class State {

            protected PlayerStates playerState;

            virtual public State Update() {
                return null;
            }

            virtual public void OnStart(PlayerStates playerState) {
                this.playerState = playerState;
            }

            virtual public void OnEnd() {

            }
        }

        /// Child Classes:
        /// 

        public class Idle : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk(10);
                playerState.IdleAnimation();
                playerState.currentState = 0;

                // transition:
                if (playerState.playerHealthAmt <= 0) return new States.DeathAnim();

                if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d") || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                    return new States.Walking();

                if (Input.GetButton("Fire1") && playerState.rateOfFire <= 0)
                    return new States.ShootingAttack();

                if (playerState.playerHealth.health <= 0) {
                    SoundEffectBoard.PlayerDeath();
                    return new States.DeathAnim();
                }

                return null;
            }
        }

        public class Walking : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk(playerState.playerWalkSpeed);
                playerState.PlayerWalkWaddleAnim();
                playerState.currentState = 1;
                playerState.hipRing.localPosition = Vector3.zero;

                // transition:
                if (Input.GetButton("Fire3")) {
                    playerState.setStepSpeed = playerState.runningStepingSpeed; ;
                    return new States.Running();
                }
                if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d") && (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0))
                    return new States.Idle();

                if (Input.GetButton("Fire1") && playerState.rateOfFire <= 0)
                    return new States.ShootingAttack();

                if (playerState.playerHealth.health <= 0) {
                    SoundEffectBoard.PlayerDeath();
                    return new States.DeathAnim();
                }

                return null;
            }
        }

        public class Running : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk(playerState.playerWalkSpeed + 7);
                playerState.PlayerRunningWaddleAnim();
                playerState.hipRing.localPosition = Vector3.zero;
                // transitions:
                if (playerState.playerHealthAmt <= 0) return new States.DeathAnim();

                if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d") && (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)) {
                    playerState.setStepSpeed = playerState.steppingSpeed;
                    return new States.Idle();
                }

                if (!Input.GetButton("Fire3")) {
                    playerState.setStepSpeed = playerState.steppingSpeed;
                    return new States.Walking();
                }

                if (playerState.playerHealth.health <= 0) {
                    SoundEffectBoard.PlayerDeath();
                    return new States.DeathAnim();
                }

                return null;
            }
        }

        public class ShootingAttack : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerGun();

                // transition:
                
                
                if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d") || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                    return new States.Walking();
                }

                return new States.Idle();
            }
        }

        public class DeathAnim : State {
            public override State Update() {
                // behaviour
                playerState.DeathAnimation();
                playerState.currentState = 2;

                return null;
            }
        }

    }

    private States.State state;

    CharacterController player;

    private Camera cam;

    [Header("Player movement variables:")]
    [HideInInspector] public float verticalVelocity = 0;

    public float gravityMultiplier = 10;

    public float playerWalkSpeed = 10;

    private float timeLeftOnGround = 0;

    public float jumpImpulse = 10;

    public float steppingSpeed = 5;
    public float runningStepingSpeed = 17;
    [HideInInspector] public float setStepSpeed;

    public Vector3 walkScale = Vector3.one;

    public Vector3 move { get; private set; }

    [Header("Player weapon variables:")]
    public Transform muzzle;

    public Transform gunModel;

    private Vector3 gunStartingLocation;

    public float roundsPerSecond = 10f;

    private float rateOfFire = 0;

    public PlayerProjectile bullet;

    public Transform playerRig;

    Rigidbody[] ragDoll;

    public GameObject walkingAnim;

    public int currentState = 0;

    public Transform hipRing;

    public float playerHealthAmt = 10;

    RigBuilder letBonesGo;
    Collider[] turningOnRagdollColliders;

    private Health playerHealth;

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
        if (state == null) StateSwitcher(new States.Idle());
        if (state != null) StateSwitcher(state.Update());

        if (timeLeftOnGround > 0) timeLeftOnGround -= Time.deltaTime;
        if (rateOfFire > 0) rateOfFire -= Time.deltaTime;

        AttackAnimation();
        print(verticalVelocity);
    }

    void StateSwitcher(States.State switchState) {
        if (switchState == null) return;
        if (state != null) state.OnEnd();

        state = switchState;
        state.OnStart(this);
        
    }

    void IdleAnimation() {
        hipRing.localPosition = Vector3.down * 0.005f * Mathf.Sin(Time.time);
        hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.identity, .001f);
    }

    void PlayerWalkWaddleAnim() {
        if (player.isGrounded)
            hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.Euler(0, 20f * Mathf.Sin(Time.time * 3), 90f * Mathf.Cos(Time.time * 4)), .001f);
        if (!player.isGrounded)
            hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.identity, .001f);
    }

    void PlayerRunningWaddleAnim() {
        if (player.isGrounded)
            hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.Euler(165, 1f * Mathf.Sin(Time.time * 3), 35f * Mathf.Cos(Time.time * 6)), .001f);
        if (!player.isGrounded)
            hipRing.localRotation = AnimMath.Slide(hipRing.localRotation, Quaternion.identity, .001f);
    }

    void PlayerWalk(float walkSpeed = 10) {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool heldJump = Input.GetButton("Jump");

        float cameraYaw = cam.transform.eulerAngles.y;
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, cameraYaw, 0), .01f);

        move = transform.right * h + v * transform.forward;
        if (move.sqrMagnitude > 1) move.Normalize();

        verticalVelocity += gravityMultiplier * Time.deltaTime;

        Vector3 moveDeltaWithGravity = move * walkSpeed + verticalVelocity * Vector3.down;

        
        player.Move(moveDeltaWithGravity * Time.deltaTime);

        if (player.isGrounded) {
            verticalVelocity = 0f;
            timeLeftOnGround = .2f;
        }

        if (isGrounded && heldJump) {
            verticalVelocity -= 10;
            timeLeftOnGround = 0;
            SoundEffectBoard.JumpSound();
        }
    }

    void PlayerGun() {

        Instantiate(bullet, muzzle.position, muzzle.rotation);

        rateOfFire = 1 / roundsPerSecond;
        SoundEffectBoard.PlayerShooting();

        gunModel.localPosition = AnimMath.Slide(gunModel.localPosition, gunModel.localPosition - (Vector3.forward * 3), .0001f);
    }

    void AttackAnimation() {
        gunModel.localPosition = AnimMath.Slide(gunModel.localPosition, gunStartingLocation, .001f);
    }

    void DeathAnimation() {
        playerRig.parent = null;
        gunModel.gameObject.SetActive(false);
        player.enabled = false;
        walkingAnim.SetActive(false);

        foreach (Rigidbody ragdollAnim in ragDoll) {
            ragdollAnim.useGravity = true;
            ragdollAnim.isKinematic = false;
        }
        foreach (Collider collider in turningOnRagdollColliders) {
            collider.isTrigger = false;
        }

        letBonesGo.enabled = false;
    }
}
