using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This controls everything that the boss does.
/// </summary>
public class BossStateMachine : MonoBehaviour {

    /// <summary>
    /// Sets up the State Machine.
    /// </summary>
    static class States {
        public class State {

            /// <summary>
            /// The way to accesss everything outside the state machine.
            /// </summary>
            protected BossStateMachine bossState;

            virtual public State Update() {
                return null;
            }

            /// <summary>
            /// When the state starts
            /// </summary>
            /// <param name="bossState"></param>
            virtual public void OnStart(BossStateMachine bossState) {
                this.bossState = bossState;
            }

            virtual public void OnEnd() {

            }
        }

        ///////// Child Classes (Boss States):


        /// <summary>
        /// When the boss is idling and not moving or attacking.
        /// </summary>
        public class Idle : State {

            /// <summary>
            /// How long the boss will idle for.
            /// </summary>
            float idleTime = 5;

            public Idle() {
                idleTime = 5;
            }

            public override State Update() {
                // behaviour:
                bossState.IdleAnim(); // Runs the idle animation function.

                // Sets velocity to zero
                Vector3 velocity = bossState.transform.forward * 0;
                Vector3 localVelocity = bossState.groundRing.InverseTransformDirection(velocity);

                // Slides boss to 'stable' position
                bossState.groundRing.localPosition = AnimMath.Slide(bossState.groundRing.localPosition, localVelocity * 2, .0001f);

                // transition: 
                if (bossState.bossHealth.health <= 0) { // if boss has no health
                    bossState.TurretDeathAnim(); // runs turret death animation function
                    bossState.bossNav.enabled = false; // turns off the boss's navigation 
                    SoundEffectBoard.BossDeathSound(); // plays the death sound
                    return new States.DeathAnim(); // goes to the death state of the boss.
                }

                if (bossState.TargetDistance(bossState.targetMeterDistance)) { // if the boss can see the player
                    bossState.bossNav.isStopped = false; // resumes the navigation to focus on attacking the player
                    return new States.Attack(); // Goes to the attack state
                }

                idleTime -= Time.deltaTime; // counts the idle time when the boss should patrol
                if (!bossState.TargetDistance(bossState.targetMeterDistance) && idleTime <= 0) { // when the boss cannot see the player and the idle timer is at 0 or below.
                    bossState.bossNav.isStopped = false; // resumes naviagation mesh
                    return new States.Walk_Patrol(true); // Starts to patrol
                }

                return null;
            }
        }

        /// <summary>
        /// For when the boss is on patrol to specific points.
        /// </summary>
        public class Walk_Patrol : State {

            /// <summary>
            /// When the boss is patrolling or not
            /// </summary>
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

                Vector3 velocity = bossState.transform.forward * 1.5f; // Moves the hip ring to what ever direction the boss is going.
                Vector3 localVelocity = bossState.groundRing.InverseTransformDirection(velocity); 

                // Slides the hip ring 
                bossState.groundRing.localPosition = AnimMath.Slide(bossState.groundRing.localPosition, localVelocity * 2, .0001f);
                bossState.MoveLegsInDirection(); // Function to move the legs in the direction it is going

                // transitions:
                if (bossState.bossHealth.health <= 0) { // if boss has no health
                    bossState.TurretDeathAnim(); // runs turret death animation function
                    bossState.bossNav.enabled = false; // turns off the boss's navigation 
                    SoundEffectBoard.BossDeathSound(); // plays the death sound
                    return new States.DeathAnim(); // goes to the death state of the boss.
                }

                if (!bossState.bossNav.pathPending && bossState.bossNav.remainingDistance <= 2f) //if the boss has reached his patroll point
                    return new States.Idle(); // goes back to idle and send idleTimer value

                if (bossState.TargetDistance(bossState.targetMeterDistance)) { // if the boss can see the player/target
                    bossState.bossNav.isStopped = false; // resumes navigation mesh
                    return new States.Attack(); // starts the attack state
                }

                return null; // reruns the state again
            }
        }

        /// <summary>
        /// When the boss is attacking the target
        /// </summary>
        public class Attack : State {

            public override State Update() {
                // behaviour:
                if (bossState.TargetDistance(bossState.targetMeterDistance - 50)) { // if the target is close enough to the target.
                    bossState.bossNav.isStopped = true; // stops the boss from moving to the target
                } else {
                    bossState.bossNav.isStopped = false; // resumes the navigation mesh
                    bossState.bossNav.SetDestination(bossState.targetPlayer.position); // Sets the target the boss to go.
                }
                bossState.LookAtTarget(); // Function makes the boss face the target
                bossState.MoveLegsInDirection(); // moves the legs in the direction the boss is going.

                // Draws the ray
                Debug.DrawRay(bossState.barrel.position, bossState.barrel.TransformDirection(Vector3.forward) * 35);
                // if Ray is touching the player
                if (bossState.TargetDistance(bossState.targetMeterDistance - 10) && Physics.Raycast(bossState.barrel.position, bossState.barrel.TransformDirection(Vector3.forward), out RaycastHit hit, bossState.AttackDistance)) {

                    if (hit.collider.gameObject.CompareTag("Player"))
                        bossState.AttackAction(); // starts shooting the player
                }

                // transition:
                if (bossState.bossHealth.health <= 0) { // if boss has no health
                    bossState.TurretDeathAnim(); // runs turret death animation function
                    bossState.bossNav.enabled = false; // turns off the boss's navigation 
                    SoundEffectBoard.BossDeathSound(); // plays the death sound
                    return new States.DeathAnim(); // goes to the death state of the boss.
                }

                if (!bossState.TargetDistance(bossState.targetMeterDistance)) { // if the boss can not see the target
                    bossState.bossNav.isStopped = true; // stops the boss from moving 
                    return new States.Idle(); // goes to the idle state
                }



                return null; // runs method again
            }
        }

        /// <summary>
        /// When the boss dies, runs this state.
        /// </summary>
        public class DeathAnim : State {

            public override State Update() {
                // behaviour:
                bossState.DeathAnimation(); // runs the death animation

                return null;
            }
        }


    }

    /// <summary>
    /// To manage the state
    /// </summary>
    private States.State state;

    /// <summary>
    /// The part to hover the boss in idle.
    /// </summary>
    public Transform hoverBody;

    /// <summary>
    /// Manages the nav mesh to move the boss where he needs to be and go to.
    /// </summary>
    private NavMeshAgent bossNav;

    /// <summary>
    /// Points to move the boss to
    /// </summary>
    public Transform[] patrolingPoints;

    /// <summary>
    /// Sets the point in the partrolingPoints where to go.
    /// </summary>
    int currentPointPatrolling;

    /// <summary>
    /// Gets the transform of the player to follow and attack
    /// </summary>
    public Transform targetPlayer;

    /// <summary>
    /// Gets cannon to add recoil to it when shooting
    /// </summary>
    public Transform mainCannon;

    /// <summary>
    /// Gets the mesh of the cannon to shoot out of the boss when the player kills it.
    /// </summary>
    public Transform mainCannonMesh;

    /// <summary>
    /// Added a rigidbody to the cannon to to add gravity to it when the cannon is shoot off
    /// </summary>
    private Rigidbody cannonRigidBody;

    /// <summary>
    /// Gets barrel transform to shoot out the projectile at
    /// </summary>
    public Transform barrel;

    /// <summary>
    /// Gets the projectile to shoot out from the barrel.
    /// </summary>
    public Transform cannonProjectile;

    /// <summary>
    /// Rate of fire the boss can shoot
    /// </summary>
    private float rateOfFire = 1;

    /// <summary>
    /// When the projectile can shoot
    /// </summary>
    private float WhenToShoot = 0;

    /// <summary>
    /// How far the boss can see the player
    /// </summary>
    public float targetMeterDistance = 10;

    /// <summary>
    /// How far the boss can shoot at the player
    /// </summary>
    public float AttackDistance = 35;

    /// <summary>
    /// Controls the hip of the boss to give its idle animation and lean when attacking the target.
    /// </summary>
    public Transform groundRing;

    /// <summary>
    /// When the boss dies, commences the legs animaiton
    /// </summary>
    bool deathAnimLiftLegs = false;

    /// <summary>
    /// Gets the feet of the boss to know when to walk.
    /// </summary>
    public List<StickyFoot> feet = new List<StickyFoot>();

    /// <summary>
    /// Gets the boss health to track.
    /// </summary>
    private Health bossHealth;

    void Start() {
        bossNav = GetComponent<NavMeshAgent>(); // Gets the boss nav mesh
        cannonRigidBody = mainCannonMesh.GetComponent<Rigidbody>(); // Gets the cannon's rigidbody
        bossHealth = GetComponent<Health>(); // gets the boss health
    }


    void Update() {
        if (state == null) SwitchStates(new States.Idle()); // sets state to be idle if it is null

        if (state != null) SwitchStates(state.Update()); // runs the current state's update

        if (WhenToShoot > 0) WhenToShoot -= Time.deltaTime; // rate of fire for the cannon

        if (bossHealth.health > 0) BarrelResetAnim(); // Recoil effect

        print(state);
    }

    /// <summary>
    /// Switches the states
    /// </summary>
    /// <param name="stateSwitched"></param>
    void SwitchStates(States.State stateSwitched) { 
        if (stateSwitched == null) return; // doesn't run the rest if the states is null

        if (state != null) state.OnEnd(); // if stats is not null, run its end
        state = stateSwitched; // state switched 
        state.OnStart(this); // run OnStart
    }

    /// <summary>
    /// The bosses animation when it idles
    /// </summary>
    void IdleAnim() {
        hoverBody.localPosition = Vector3.down * .25f * Mathf.Cos(Time.time); // hovers its up and down position
        hoverBody.localRotation = Quaternion.Euler(2f * Mathf.Sin(Time.time), 4f * Mathf.Sin(Time.time), 2f * Mathf.Cos(Time.time)); // rotates the boss itself for idle
    }

    /// <summary>
    /// Determines if hte player is in distance to pursue
    /// </summary>
    /// <param name="targetDis"></param>
    /// <returns></returns>
    private bool TargetDistance(float targetDis) {

        if (!targetPlayer) return false; // if no target

        Vector3 disToTarget = targetPlayer.position - transform.position; // gets distance

        if (disToTarget.sqrMagnitude > targetDis * targetDis) { // if too far
            return false;
        }

        return true; // if the boss can see the player
    }

    /// <summary>
    /// Turns the boss towards the player
    /// </summary>
    void LookAtTarget() {
        if (!targetPlayer) return; // if no target
        
        Quaternion rotateTowards = Quaternion.LookRotation((targetPlayer.position - hoverBody.position) - new Vector3(0, 6, 0));

        Quaternion constricting = rotateTowards;
        constricting.x = Mathf.Clamp(constricting.x, -.10f, .15f);

        // Rotate to the target
        hoverBody.rotation = Quaternion.Slerp(hoverBody.rotation, constricting, 2 * Time.deltaTime); //hoverBody.LookAt(targetPlayer.position - new Vector3(0, 5, 0));
    }

    /// <summary>
    /// When the boss is attacking and shooting
    /// </summary>
    void AttackAction() {
        if (WhenToShoot > 0) return; // not ready to shoot from rate of fire

        BarrelRecoil(); // adds recoil to the barrel
        Instantiate(cannonProjectile, barrel.position, barrel.rotation); // spawns the projectile 
        SoundEffectBoard.BossShooting(); // plays shooting sound
        WhenToShoot = 1 / rateOfFire;
    }

    /// <summary>
    /// Adds recoil to the barrel
    /// </summary>
    void BarrelRecoil() {
        mainCannon.localPosition = Vector3.up * .01f; // pushes barrel
    }

    /// <summary>
    /// Move barrel back into position from recoil
    /// </summary>
    void BarrelResetAnim() {
        if (mainCannon.localPosition.y > 0f) {
            float cannonRecoilRest = AnimMath.Slide(mainCannon.localPosition.y, Vector3.zero.y, .001f); // slides the barrel back
            Vector3 recoilOver = new Vector3(0, cannonRecoilRest, 0.0175f);
            mainCannon.localPosition = recoilOver; // keeps barrel where it is supposed to be
        }
    }

    /// <summary>
    /// The animation when the boss dies
    /// </summary>
    void DeathAnimation() {

        // lifts legs up
        if (!deathAnimLiftLegs) {
            hoverBody.localPosition = Vector3.Slerp(hoverBody.localPosition, Vector3.up * 1f, 1f * Time.deltaTime); // lifts legs up smoothly

            if (hoverBody.localPosition.y >= .95f) deathAnimLiftLegs = true;

            return;
        }

        // puts legs down
        if (hoverBody.localPosition.y > -2.5f) {
            hoverBody.localPosition = Vector3.Slerp(hoverBody.localPosition, Vector3.down * 2.8f, 1f * Time.deltaTime); // drops legs smoothly
            hoverBody.localRotation = Quaternion.Euler(1f * Mathf.Sin(5 * Time.time), 1f * Mathf.Sin(5 * Time.time), .8f * Mathf.Cos(5 * Time.time)); // adds shacking effect
        }

    }

    /// <summary>
    /// Animation to blow turret off the body
    /// </summary>
    void TurretDeathAnim() {
        // changes the rigidbody from using kinematic to gravity
        cannonRigidBody.isKinematic = false;
        cannonRigidBody.useGravity = true;

        mainCannonMesh.parent = null; // kicks the cannon mesh off of the boss's group

        cannonRigidBody.AddForce(0, 20, 4, ForceMode.Impulse); // adds the force to pop off turret
    }

    /// <summary>
    /// Sets the patrol point to go to for the boss to move around when not attacking
    /// </summary>
    void PatrolingPointsSetter() {
        bossNav.updatePosition = true; // sets to true
        bossNav.destination = patrolingPoints[currentPointPatrolling].position; // sets point to go to 
        currentPointPatrolling = (currentPointPatrolling + 1) % patrolingPoints.Length; // assigns the next point to go to
    }

    /// <summary>
    /// Manages what legs can move and what legs cannot
    /// </summary>
    void MoveLegsInDirection() {
        int feetStepping = 0;
        int feetMoved = 0;
        foreach (StickyFoot foot in feet) {
            if (foot.isAnimating) feetStepping++; // when foot is stepping
            if (foot.footHasMoved) feetMoved++; // when foot has moved
        }

        if (feetMoved >= 6) {
            foreach (StickyFoot foot in feet) {
                foot.footHasMoved = false; // reset foots that are not moving
            }
        }

        foreach (StickyFoot foot in feet) {
            if (feetStepping < 3) { // if feet going to step
                if (foot.TryToMove()) { // moves the foot and gets a bool back
                    feetStepping++; // increments the feet steping
                }
            }
        }
    }
}
