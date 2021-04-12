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
                if (bossState.health <= 0)
                    return new States.DeathAnim();

                if (bossState.TargetDistance(bossState.targetMeterDistance)) {
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
                // behaviour:
                if (bossState.TargetDistance(bossState.targetMeterDistance - 20)) {
                    bossState.bossNav.isStopped = true;
                } else {
                    bossState.bossNav.SetDestination(bossState.targetPlayer.position);
                }
                bossState.LookAtTarget();


                Debug.DrawRay(bossState.barrel.position, bossState.barrel.TransformDirection(Vector3.forward) * 35);
                if (bossState.TargetDistance(bossState.targetMeterDistance - 5) && Physics.Raycast(bossState.barrel.position, bossState.barrel.TransformDirection(Vector3.forward), out RaycastHit hit, bossState.AttackDistance)) {

                    if (hit.collider.gameObject.CompareTag("Player"))
                        bossState.AttackAction();
                }

                // transition:
                if (!bossState.TargetDistance(bossState.targetMeterDistance)) {
                    bossState.bossNav.isStopped = true;
                    return new States.Idle();
                }



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

    public Transform hoverBody;

    private NavMeshAgent bossNav;

    public Transform targetPlayer;

    public Transform barrel;

    public Transform cannonProjectile;

    private float rateOfFire = 1;

    private float timeCannonSpawns = 0;

    public float targetMeterDistance = 10;

    public float AttackDistance = 35;


    private float health = 10;

    void Start() {
        bossNav = GetComponent<NavMeshAgent>();
    }


    void Update() {
        if (state == null) SwitchStates(new States.Idle());

        if (state != null) SwitchStates(state.Update());

        if (timeCannonSpawns > 0) timeCannonSpawns -= Time.deltaTime;
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

    private bool TargetDistance(float targetDis) {

        if (!targetPlayer) return false;

        Vector3 disToTarget = targetPlayer.position - transform.position;

        if (disToTarget.sqrMagnitude > targetDis * targetDis) {
            return false;
        }

        return true;
    }

    void LookAtTarget() {
        if (!targetPlayer) return;

        print("trying to look");
        Quaternion rotateTowards = Quaternion.LookRotation((targetPlayer.position - hoverBody.position) - new Vector3(0, 6, 0));

        Quaternion constricting = rotateTowards;
        constricting.x = Mathf.Clamp(constricting.x, -.10f, .15f);

        print(constricting.x);

        hoverBody.rotation = Quaternion.Slerp(hoverBody.rotation, constricting, 1 * Time.deltaTime); //hoverBody.LookAt(targetPlayer.position - new Vector3(0, 5, 0));
    }

    void AttackAction() {
        if (timeCannonSpawns > 0) return;

        Instantiate(cannonProjectile, barrel.position, barrel.rotation);
        timeCannonSpawns = 1 / rateOfFire;
    }
}
