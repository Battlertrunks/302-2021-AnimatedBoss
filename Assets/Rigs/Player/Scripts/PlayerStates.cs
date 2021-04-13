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

                // transition:
                if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d"))
                    return new States.Idle();

                if (Input.GetButton("Fire1") && playerState.rateOfFire <= 0)
                    return new States.ShootingAttack();

                return null;
            }
        }

        public class ShootingAttack : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerGun();

                // transition:
                
                
                return new States.Idle();
            }
        }

        public class DeathAnim : State {
            public override State Update() {
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

    [Header("Player weapon variables:")]
    public Transform muzzle;

    public float roundsPerSecond = 10f;

    private float rateOfFire = 0;

    public PlayerProjectile bullet;

    public bool isGrounded {
        get {
            return player.isGrounded || timeLeftOnGround > 0;
        }
    }

    void Start() {
        player = GetComponent<CharacterController>();
        cam = Camera.main;
    }


    void Update() {
        if (state == null) StateSwitcher(new States.Idle());
        if (state != null) StateSwitcher(state.Update());

        if (timeLeftOnGround > 0) timeLeftOnGround -= Time.deltaTime;
        if (rateOfFire > 0) rateOfFire -= Time.deltaTime;
    }

    void StateSwitcher(States.State switchState) {
        if (switchState == null) return;
        if (state != null) state.OnEnd();

        state = switchState;
        state.OnStart(this);
        
    }

    void PlayerWalk(float walkSpeed = 10) {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool heldJump = Input.GetButton("Jump");

        float cameraYaw = cam.transform.eulerAngles.y;
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, cameraYaw, 0), .01f);

        Vector3 move = transform.right * h + v * transform.forward;
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
    }
}
