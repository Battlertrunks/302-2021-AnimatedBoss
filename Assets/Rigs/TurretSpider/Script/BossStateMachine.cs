using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour {

    static class States {
        public class State {

            protected BossStateMachine bossState;

            virtual public State Update() {
                return null;
            }

            virtual public void OnStart(BossStateMachine bossState) {
                this.bossState = bossState;
            }

            virtual public void OnEnd() {

            }
        }

        ///////// Child Classes (Boss States):
        ///

        public class Idle : State {

            public override State Update() {
                // behaviour:
                bossState.IdleAnim();


                return null;
            }
        }

        public class Walk_Patrol : State {

            public override State Update() {

                return null;
            }
        }

        public class Attack : State {

            public override State Update() {

                return null;
            }
        }

        public class deathAnim : State {

            public override State Update() {

                return null;
            }
        }


    }

    private States.State state;

    public Transform hoverBody;

    void Start() {
        
    }


    void Update() {
        if (state == null) SwitchStates(new States.Idle());

        if (state != null) SwitchStates(state.Update());
    }

    void SwitchStates(States.State stateSwitched) {
        if (stateSwitched == null) return;

        if (state != null) state.OnEnd();
        state = stateSwitched;
        state.OnStart(this);
    }

    void IdleAnim() {
        hoverBody.position = Vector3.up * Mathf.Cos(Time.time);
    }
}
