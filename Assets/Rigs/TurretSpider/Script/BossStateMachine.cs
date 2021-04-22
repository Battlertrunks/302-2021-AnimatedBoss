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

            float idleTime = 5;

            public Idle() {
                idleTime = 5;
            }

            public override State Update() {
                // behaviour:
                bossState.IdleAnim();

                Vector3 velocity = bossState.transform.forward * 0;
                Vector3 localVelocity = bossState.groundRing.InverseTransformDirection(velocity);

                bossState.groundRing.localPosition = AnimMath.Slide(bossState.groundRing.localPosition, localVelocity * 2, .0001f);

                // transition: 
                if (bossState.health <= 0) {
                    bossState.TurretDeathAnim();
                    bossState.bossNav.enabled = false;
                    return new States.DeathAnim();
                }

                if (bossState.TargetDistance(bossState.targetMeterDistance)) {
                    bossState.bossNav.isStopped = false;
                    return new States.Attack();
                }

                idleTime -= Time.deltaTime;
                if (!bossState.TargetDistance(bossState.targetMeterDistance) && idleTime <= 0) {
                    bossState.bossNav.isStopped = false;
                    return new States.Walk_Patrol(true); // Starts to patrol
                }

                return null;
            }
        }

        public class Walk_Patrol : State {

            bool patrollingPoint = true;

            public Walk_Patrol(bool patrolling) {
                patrollingPoint = patrolling;
            }

            public override State Update() {
                // behaviour:
                if (patrollingPoint) { // if runPatrolOnce is true
                    bossState.PatrolingPointsSetter(); // goes to randomly selected patrol points
                    patrollingPoint = false; // turning to false to run only once
                }

                Vector3 velocity = bossState.transform.forward * 1.5f;
                Vector3 localVelocity = bossState.groundRing.InverseTransformDirection(velocity);

                bossState.groundRing.localPosition = AnimMath.Slide(bossState.groundRing.localPosition, localVelocity * 2, .0001f);
                bossState.MoveLegsInDirection();

                // transitions:
                if (bossState.health <= 0) {
                    bossState.TurretDeathAnim();
                    bossState.bossNav.enabled = false;
                    return new States.DeathAnim();
                }

                if (!bossState.bossNav.pathPending && bossState.bossNav.remainingDistance <= 2f) //if the boss has reached his patroll point
                    return new States.Idle(); // goes back to idle and send idleTimer value

                if (bossState.TargetDistance(bossState.targetMeterDistance)) {
                    bossState.bossNav.isStopped = false;
                    return new States.Attack();
                }

                return null;
            }
        }

        public class Attack : State {

            public override State Update() {
                // behaviour:
                if (bossState.TargetDistance(bossState.targetMeterDistance - 20)) {
                    bossState.bossNav.isStopped = true;
                } else {
                    bossState.bossNav.isStopped = false;
                    bossState.bossNav.SetDestination(bossState.targetPlayer.position);
                }
                bossState.LookAtTarget();
                bossState.MoveLegsInDirection();


                Debug.DrawRay(bossState.barrel.position, bossState.barrel.TransformDirection(Vector3.forward) * 35);
                if (bossState.TargetDistance(bossState.targetMeterDistance - 5) && Physics.Raycast(bossState.barrel.position, bossState.barrel.TransformDirection(Vector3.forward), out RaycastHit hit, bossState.AttackDistance)) {

                    if (hit.collider.gameObject.CompareTag("Player"))
                        bossState.AttackAction();
                }

                // transition:
                if (bossState.health <= 0) {
                    bossState.TurretDeathAnim();
                    bossState.bossNav.enabled = false;
                    return new States.DeathAnim();
                }

                if (!bossState.TargetDistance(bossState.targetMeterDistance)) {
                    bossState.bossNav.isStopped = true;
                    return new States.Idle();
                }



                return null;
            }
        }

        public class DeathAnim : State {

            public override State Update() {
                // behaviour:
                bossState.DeathAnimation();

                return null;
            }
        }


    }

    private States.State state;

    public Transform hoverBody;

    private NavMeshAgent bossNav;

    public Transform[] patrolingPoints;

    int currentPointPatrolling;

    public Transform targetPlayer;

    public Transform mainCannon;

    public Transform mainCannonMesh;

    private Rigidbody cannonRigidBody;

    public Transform barrel;

    public Transform cannonProjectile;

    private float rateOfFire = 1;

    private float timeCannonSpawns = 0;

    public float targetMeterDistance = 10;

    public float AttackDistance = 35;

    public Transform groundRing;

    bool deathAnimLiftLegs = false;


    public float health = 10;

    public List<StickyFoot> feet = new List<StickyFoot>();

    void Start() {
        bossNav = GetComponent<NavMeshAgent>();
        cannonRigidBody = mainCannonMesh.GetComponent<Rigidbody>();
    }


    void Update() {
        if (state == null) SwitchStates(new States.Idle());

        if (state != null) SwitchStates(state.Update());

        if (timeCannonSpawns > 0) timeCannonSpawns -= Time.deltaTime;

        if (health > 0) BarrelResetAnim();

        print(state);
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

        BarrelRecoil();
        Instantiate(cannonProjectile, barrel.position, barrel.rotation);
        timeCannonSpawns = 1 / rateOfFire;
    }

    void BarrelRecoil() {
        mainCannon.localPosition = Vector3.up * .01f;
    }

    void BarrelResetAnim() {
        if (mainCannon.localPosition.y > 0f) {
            float cannonRecoilRest = AnimMath.Slide(mainCannon.localPosition.y, Vector3.zero.y, .001f);
            Vector3 recoilOver = new Vector3(0, cannonRecoilRest, 0.0175f);
            mainCannon.localPosition = recoilOver;
        }
    }

    void DeathAnimation() {

        if (!deathAnimLiftLegs) {
            hoverBody.localPosition = Vector3.Slerp(hoverBody.localPosition, Vector3.up * 1f, 1f * Time.deltaTime);

            if (hoverBody.localPosition.y >= .95f) deathAnimLiftLegs = true;

            return;
        }

        if (hoverBody.localPosition.y > -2.5f) {
            hoverBody.localPosition = Vector3.Slerp(hoverBody.localPosition, Vector3.down * 2.8f, 1f * Time.deltaTime);
            hoverBody.localRotation = Quaternion.Euler(1f * Mathf.Sin(5 * Time.time), 1f * Mathf.Sin(5 * Time.time), .8f * Mathf.Cos(5 * Time.time));
        }

    }

    void TurretDeathAnim() {
        cannonRigidBody.isKinematic = false;
        cannonRigidBody.useGravity = true;

        mainCannonMesh.parent = null;

        cannonRigidBody.AddForce(0, 20, 4, ForceMode.Impulse);
    }

    void PatrolingPointsSetter() {
        bossNav.updatePosition = true; // sets to true
        bossNav.destination = patrolingPoints[currentPointPatrolling].position; // sets point to go to 
        currentPointPatrolling = (currentPointPatrolling + 1) % patrolingPoints.Length; // assigns the next point to go to
    }

    void MoveLegsInDirection() {
        int feetStepping = 0;
        int feetMoved = 0;
        foreach (StickyFoot foot in feet) {
            if (foot.isAnimating) feetStepping++;
            if (foot.footHasMoved) feetMoved++;
        }

        if (feetMoved >= 6) {
            foreach (StickyFoot foot in feet) {
                foot.footHasMoved = false;
            }
        }

        foreach (StickyFoot foot in feet) {
            if (feetStepping < 3) {
                if (foot.TryToMove()) {
                    feetStepping++;
                }
            }
        }
    }
}
