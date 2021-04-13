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

                if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
                    return new States.Walking();

                return null;
            }
        }

        public class Walking : State {
            public override State Update() {
                // behaviour:
                playerState.PlayerWalk();

                // transition:
                if (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d"))
                    return new States.Idle();

                return null;
            }
        }

        public class ShootingAttack : State {
            public override State Update() {
                return null;
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

    void Start() {
        player = GetComponent<CharacterController>();
        cam = Camera.main;
    }


    void Update() {
        if (state == null) StateSwitcher(new States.Idle());
        if (state != null) StateSwitcher(state.Update());
    }

    void StateSwitcher(States.State switchState) {
        if (switchState == null) return;
        if (state != null) state.OnEnd();

        state = switchState;
        state.OnStart(this);
        
    }

    void PlayerWalk() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float cameraYaw = cam.transform.eulerAngles.y;
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, cameraYaw, 0), .01f);

        Vector3 move = transform.right * h + v * transform.forward;
        if (move.sqrMagnitude > 1) move.Normalize();


        player.Move(move * 10 * Time.deltaTime);
    }
}
