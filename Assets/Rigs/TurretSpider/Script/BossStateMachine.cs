using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

                // transition: 
                if (bossState.targetPlayer) {
                    bossState.bossNav.isStopped = false;
                    return new States.Attack();
                }

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
                // behaviour
                bossState.bossNav.SetDestination(bossState.targetPlayer.position);

                if (!bossState.targetPlayer) {
                    bossState.bossNav.isStopped = true;
                    return new States.Idle();
                }

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

    private NavMeshAgent bossNav;

    public Transform targetPlayer;

    void Start() {
        bossNav = GetComponent<NavMeshAgent>();
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
        hoverBody.localPosition = Vector3.down * .25f * Mathf.Cos(Time.time);
        hoverBody.localRotation = Quaternion.Euler(2f * Mathf.Sin(Time.time), 4f * Mathf.Sin(Time.time), 2f * Mathf.Cos(Time.time));
    }
}
