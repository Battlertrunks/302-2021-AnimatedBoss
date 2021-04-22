using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
                    return new States.Walking();

                if (Input.GetButton("Fire1") && playerState.rateOfFire <= 0)
                    return new States.ShootingAttack();

                return null;
            }
        }

        public class Walking : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk(playerState.playerWalkSpeed);
                playerState.currentState = 1;

                // transition:
                if (Input.GetButton("Fire3")) {
                    playerState.setStepSpeed = playerState.runningStepingSpeed; ;
                    return new States.Running();
                }
                if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d"))
                    return new States.Idle();

                if (Input.GetButton("Fire1") && playerState.rateOfFire <= 0)
                    return new States.ShootingAttack();

                if (Input.GetButton("Fire2"))
                    return new States.DeathAnim();

                return null;
            }
        }

        public class Running : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk(playerState.playerWalkSpeed + 7);

                // transitions:
                if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d")) {
                    playerState.setStepSpeed = playerState.steppingSpeed;
                    return new States.Idle();
                }

                if (!Input.GetButton("Fire3")) {
                    playerState.setStepSpeed = playerState.steppingSpeed;
                    return new States.Walking();
                }

                return null;
            }
        }

        public class ShootingAttack : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerGun();

                // transition:
                
                
                if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")) {
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
    private float verticalVelocity = 0;

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
    }


    void Update() {
        if (state == null) StateSwitcher(new States.Idle());
        if (state != null) StateSwitcher(state.Update());

        if (timeLeftOnGround > 0) timeLeftOnGround -= Time.deltaTime;
        if (rateOfFire > 0) rateOfFire -= Time.deltaTime;

        AttackAnimation();
    }

    void StateSwitcher(States.State switchState) {
        if (switchState == null) return;
        if (state != null) state.OnEnd();

        state = switchState;
        state.OnStart(this);
        
    }

    void IdleAnimation() {
        hipRing.localPosition = Vector3.down * 0.05f * Mathf.Sin(Time.time);
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
            verticalVelocity = 0;
            timeLeftOnGround = .2f;
        }

        if (isGrounded && heldJump) {
            verticalVelocity -= 10;
            timeLeftOnGround = 0;
        }
    }

    void PlayerGun() {

        Instantiate(bullet, muzzle.position, muzzle.rotation);

        rateOfFire = 1 / roundsPerSecond;

        gunModel.localPosition = AnimMath.Slide(gunModel.localPosition, gunModel.localPosition - (Vector3.forward * 3), .0001f);
    }

    void AttackAnimation() {
        gunModel.localPosition = AnimMath.Slide(gunModel.localPosition, gunStartingLocation, .001f);
    }

    void DeathAnimation() {
        playerRig.parent = null;
        player.enabled = false;
        walkingAnim.SetActive(false);

        foreach (Rigidbody ragdollAnim in ragDoll) {
            ragdollAnim.useGravity = true;
            ragdollAnim.isKinematic = false;
        }
    }
}
